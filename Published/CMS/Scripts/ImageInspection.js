class ICAOAttribute {
    constructor(heading, icaoCompliant, range, score, dependancyStatus) {
        this.heading = heading;
        this.icaoCompliant = icaoCompliant;
        this.range = range;
        this.score = score;
        this.dependancyStatus = dependancyStatus;
    }
}

class CropData {
    constructor(jsonData) {
        if (jsonData != null || jsonData != undefined) {
            this.left = jsonData.StartX;
            this.top = jsonData.StartY;
            this.width = jsonData.Width;
            this.height = jsonData.Height;
        }
        else {
            this.left = 0;
            this.top = 4;
            this.width = 300;
            this.height = 400;
        }
    }
}

class ExtraOptions {
    constructor(jsonSettings) {
        this.minRatio = jsonSettings.MinPortraitRatio;
        this.maxRatio = jsonSettings.MaxPortraitRatio;
        this.minPixelHeight = jsonSettings.MinPixelHeight;
        this.minPixelWidth = jsonSettings.MinPixelWidth;
        this.ghostImageFilter = jsonSettings.GhostImageFilter / 10;
    }
}

class ImageInspection {
    constructor(imageFormat) {
        this.allICAOAttributes = [];
        this.imageStages = [];
        this.imageFormat = imageFormat;
        this.el = 'canvas';
        this.dimension = '2d';
        this.targetImage;
        this.tagetCanvas;
        this.targetCanvasId = '#main-image-canvas'
        this.defaultImageWidth = '300px';
        this.defaultImageHeight = 'auto';

        this.badStrokeColor = 'red';
        this.goodStrokeColor = 'lightskyblue';
        this.fillColor = 'white';
        this.font = '16px Arial';
        this.lineWidth = '3';
        this.bcuCanvas;

        this.cropper;
        this.onceOff = true;
        this.isSettingUp = true;
        this.cropperAspectRatio = 0.75
        this.cropperInitialAspectRatio = 0.75;
        this.aspectRatio = 0.75;
        this.cropData = new CropData(null);
        this.options;
        this.jsonData;
        this.jsonSettings;
        this.minBoxWidth = 0;
        this.minBoxHeight = 0;
        this.imageRGBArray = [];
        this.originalImage;
        this.noBgImgControlId;
        this.initData;

        Caman.DEBUG = ('Console' in window);
    }

    static imageCollection = [null, null, null]
    static finalImageSource = null;
    static finalGhostImage = null;
    static ihn = null;

    icaoAttribute = function (heading, icaoCompliant, range, score, dependancyStatus) {
        return new ICAOAttribute(heading, icaoCompliant, range, score, dependancyStatus);
    }

    convertToGrayScale = function (sourceElement, width = 65, height = 80) {
        var result = null;

        try {
            let canvas = document.createElement(this.el);
            canvas.width = width; canvas.height = height;
            let context = canvas.getContext(this.dimension);
            let x = 0, y = 0;

            context.drawImage(sourceElement, x, y, width, height);

            var imageData = context.getImageData(x, y, width, height);
            var data = imageData.data;

            for (var i = 0; i < data.length; i += 4) {
                var brightness = 0.34 * data[i] + 0.5 * data[i + 1] + 0.16 * data[i + 2];
                data[i] = brightness;
                data[i + 1] = brightness;
                data[i + 2] = brightness;
            }

            context.putImageData(imageData, x, y);

            result = canvas.toDataURL(this.imageFormat);
        }
        catch (e) { console.log(e); }

        return result;
    }

    drawCardImage = function (sourceElement, gBeforeCallback, gAfterCallback) {
        try {
            this.prepareGhostImage(sourceElement, gBeforeCallback, gAfterCallback);
        }
        catch (e) { console.log(e); }
    }

    drawCropSection = function (canvas, uiCallback) {
        let cropInfo = this.cropData;

        try {
            if (canvas.getContext) {
                let context = canvas.getContext(this.dimension);
                let w = this.originalImage.width;
                let h = this.originalImage.height;
                let x = parseFloat(this.cropData.left = (this.cropData.left / w) * canvas.width);
                let y = parseFloat(this.cropData.top = (this.cropData.top / h) * canvas.height);

                cropInfo.width = parseFloat(w = (cropInfo.width / w) * canvas.width);
                cropInfo.height = parseFloat(h = (cropInfo.height / h) * canvas.height);

                const ratio = Math.round((cropInfo.width / cropInfo.height) * 100);

                if ((cropInfo.top + cropInfo.height) > canvas.height || (cropInfo.left + cropInfo.width) > canvas.width
                    || ratio < this.options.minRatio || ratio > this.options.maxRatio) {
                    context.strokeStyle = this.badStrokeColor;
                    this.cropperAspectRatio = this.cropperInitialAspectRatio = this.aspectRatio;
                }
                else {
                    context.strokeStyle = this.goodStrokeColor;
                    this.cropperAspectRatio = this.cropperInitialAspectRatio = NaN;
                }

                this.minBoxHeight = canvas.height * (this.options.minPixelHeight / 100);
                this.minBoxWidth = this.minBoxHeight * ratio;

                context.beginPath();
                context.fillStyle = this.fillColor;
                context.fillRect(10, 135, 50, 20);

                context.font = this.font;
                context.strokeText(ratio + "%", 20, 150);

                context.beginPath();
                context.rect(x, y, w, h);

                context.lineWidth = this.lineWidth;
                context.stroke();

                if (ratio > 0)
                    uiCallback(canvas);
            }
        }
        catch (e) { console.log(e); }

        return cropInfo;
    }

    onceOffCrop = function (canvas, uiCallback, brBeforeCallback, brAfterCallback) {
        uiCallback(canvas);
        this.updateNoBackgroundImage(canvas.toDataURL(this.imageFormat), true,
            brBeforeCallback, brAfterCallback);
    }

    adjustBrightness = function (source, value, beforeCallback, afterCallback) {
        beforeCallback();
        $.ajax({
            type: 'POST',
            url: "/ImageInspection/_AdjustBrightness",
            data: {
                base64String: source,
                value: value
            },
            success: function (result) {
                afterCallback(result);
            }
        });
    }

    prepareGhostImage = function (sourceElement, beforeCallback, afterCallback) {
        beforeCallback();
        let source = sourceElement instanceof HTMLCanvasElement ? sourceElement.toDataURL(this.imageFormat)
            : sourceElement.src;

        let parent = this;

        $.ajax({
            type: 'POST',
            url: "/ImageInspection/_GetGhostImage",
            data: { base64String: source },
            success: function (result) {
                ImageInspection.finalImageSource = source;
                ImageInspection.finalGhostImage = result;
                afterCallback(source, result, parent.options);
            }
        });
    }

    
    updateNoBackgroundImage = function (source, updateAll, beforeCallback, afterCallback) {
        beforeCallback(source);
        for (let i in ImageInspection.imageCollection) {
            if (i == 0 && updateAll == false)
                continue;
            ImageInspection.imageCollection[i] = source;
        }

        $.ajax({
            type: 'POST',
            url: "/ImageInspection/_GetNoBackground",
            data: {
                images: ImageInspection.imageCollection,
                updateAll: updateAll
            },
            success: function (result) {
                afterCallback(result, ImageInspection.imageCollection);
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    }

    updateICAOInfo = function (beforeCallback, afterCallback) {
        beforeCallback();
        $.ajax({
            type: 'POST',
            url: "/ImageInspection/_ICAOUpdate",
            data: {
                images: ImageInspection.imageCollection
            },
            success: function (result) {
                afterCallback(result);
            }
        });
    }

    updateInspectionImages = function (beforeCallback, afterCallback) {
        beforeCallback();
        let inspectionInstance = this;
        $.ajax({
            type: 'POST',
            url: "/ImageInspection/_InspectionImages",
            data: {},
            success: function (result) {
                afterCallback(result);
                inspectionInstance.cropper.reset();
                inspectionInstance._init_(inspectionInstance.initData);
            }
        });
    }

    confirmRejection = function (reasonId, additionalText, beforeCallback, afterCallback) {
        beforeCallback();
        $.ajax({
            type: 'POST',
            url: "/ImageInspection/RejectImage",
            data: {
                handleNo: ImageInspection.ihn,
                reasonId: reasonId,
                additionalText: additionalText
            },
            success: function (result) {
                afterCallback(result);
            }
        });
    }

    getFailedICAOInfo = function (index, afterCallback) {
        $.ajax(
            {
                type: 'POST',
                url: "/ImageInspection/_FailedICAO",
                data: {
                    index: index
                },
                success: function (result) {
                    afterCallback(result);
                }
            }
        );
    }

    saveChanges = function (beforeCallback, successCallback, failCallback) {
        beforeCallback();
        $.ajax({
            type: 'POST',
            url: "/ImageInspection/UpdateInfo",
            data: {
                base64String: ImageInspection.finalImageSource,
                ghostBase64String: ImageInspection.finalGhostImage,
                imageHandleNo: ImageInspection.ihn
            },
            success: function (result) {
                if (Boolean(result) == true)
                    successCallback();
                else
                    failCallback('Failed to save Images');
            },
            error: function (result) {
                failCallback(result.responseText);
            }
        });
    }

    resetFilters = function (beforeCallback, afterCallback) {
        beforeCallback();
        $.ajax(
            {
                type: 'POST',
                url: "/ImageInspection/_Filters",
                data: {},
                success: function (result) {
                    afterCallback(result);
                }
            }
        );
    }

    resetChanges = function (resetFilters = false, resetCrop = false, uiCallback, ocUICallback,
        brBeforeCallback, brAfterCallback, rfCallback, beforeCallback, afterCallback) {
        if (resetCrop)
            this.updateInspectionImages(uiCallback, ocUICallback,
                brBeforeCallback, brAfterCallback, beforeCallback, afterCallback);

        if (resetFilters)
            this.resetFilters(rfCallback, null);
    }

    applyFilterChanges = function (beforeCallback, afterCallback) {
        this.targetCanvas = document.getElementById(this.targetCanvasId.substr(1));
        let source = this.targetCanvas instanceof HTMLCanvasElement ? this.targetCanvas.toDataURL(this.imageFormat)
            : this.targetCanvas.src;
        this.updateNoBackgroundImage(source, false, beforeCallback, afterCallback);
    }

    _init_ = function (initData) {
        this.options = new ExtraOptions(JSON.parse(document.getElementById(initData.jsonSettingsControlId).value));
        this.bcuCanvas = document.getElementById(initData.bcuCanvasControlId);
        this.originalImage = document.getElementById(initData.originalImageControlId);
        this.targetImage = document.getElementById(initData.mainImageControlId);
        this.targetCanvas = document.getElementById(initData.mainCanvasControlId);
        this.noBgImgControlId = initData.noBgImageControlId;
        this.jsonData = JSON.parse(document.getElementById(initData.jsonCropDataControlId).value);
        this.jsonSettings = JSON.parse(document.getElementById(initData.jsonSettingsControlId).value);
        this.initData = initData;
        this.onceOff = true;

        ImageInspection.ihn = document.getElementById(initData.ihnControlId).value;

        if (this.jsonData != null && this.jsonData != undefined) {
            this.cropData = new CropData(this.jsonData);

            let context = this.bcuCanvas.getContext(this.dimension);
            context.drawImage(this.targetImage, 0, 0, 300, 169);

            this.cropData = this.jsonData = JSON.stringify(this.drawCropSection(this.bcuCanvas,
                initData.bcuCropSectionCallback));
        }

        this.targetImage.style.width = this.defaultImageWidth;
        this.targetImage.style.height = this.defaultImageHeight;

        let inspectionInstance = this;

        this.cropper = new Cropper(this.targetImage, {
            autoCrop: false,
            minCropBoxWidth: inspectionInstance.minBoxWidth,
            minCropBoxHeight: inspectionInstance.minBoxHeight,
            initialAspectRatio: inspectionInstance.cropperInitialAspectRatio,
            aspectRatio: inspectionInstance.cropperAspectRatio,
            ready: function () {
                this.cropper.crop();
                if (isNaN(inspectionInstance.cropperAspectRatio) && isNaN(inspectionInstance.cropperInitialAspectRatio)) {
                    inspectionInstance.cropper.setCropBoxData(JSON.parse(inspectionInstance.cropData));
                }
            },
            crop: function (event) {
                let croppedCanvas = this.cropper.getCroppedCanvas(event.detail);

                if (inspectionInstance.isSettingUp == true && isNaN(inspectionInstance.cropperAspectRatio)) {
                    inspectionInstance.isSettingUp = false;
                    return;
                }

                initData.cropperUICallback(croppedCanvas.toDataURL(inspectionInstance.imageFormat), null);

                if (inspectionInstance.onceOff) {
                    for (let i in inspectionInstance.imageCollection)
                        inspectionInstance.imageCollection[i] =
                            croppedCanvas.toDataURL(inspectionInstance.imageFormat);

                    inspectionInstance.onceOffCrop(croppedCanvas, initData.onceOffCropCallback, initData.bgRemoverBeforeCallback,
                        initData.bgRemoverAfterCallback);
                    inspectionInstance.onceOff = false;
                }
            },
            cropend: function (event) {
                let croppedCanvas = this.cropper.getCroppedCanvas(event.detail);

                inspectionInstance.updateNoBackgroundImage(croppedCanvas.toDataURL(this.imageFormat), true,
                    initData.bgRemoverBeforeCallback, initData.bgRemoverAfterCallback);
            }
        });

        this.isSettingUp = inspectionInstance.isSettingUp;
        this.onceOff = inspectionInstance.onceOff;

        this.cropper.setAspectRatio(this.aspectRatio);

        if (initData.isReloading == true)
            this.cropper.crop();
    }

    filterImage = function (r, g, b, brightness, contrast, saturation, vibrance, exposure, hue,
        sepia, sharpness) {
        Caman(this.targetCanvasId, function () {
            this.revert(false);

            this.brightness(brightness);
            this.contrast(contrast);
            this.saturation(saturation);
            this.vibrance(vibrance);
            this.exposure(exposure);
            this.hue(hue);
            this.sepia(sepia);
            this.sharpen(sharpness);

            this.channels({
                red: r,
                green: g,
                blue: b
            }).render();
        });
    }

    drawMiniGhostImage = function (sourceElement, uiCallback) {
        uiCallback(sourceElement);
    }

    openCloseCrop = function (isDone = false, uiCallback, iiCallback, ocUICallback,
        brBeforeCallback, brAfterCallback, beforeCallback, afterCallback) {
        if (isDone)
            this.cropper.clear();
        else
            updateInspectionImages(iiCallback, ocUICallback,
                brBeforeCallback, brAfterCallback, beforeCallback, afterCallback)

        uiCallback(isDone);
    }

    drawNewCanvas = function (filtersId, rgbControlId, inputClass, valueClass, uiCallback) {
        this.targetCanvas.src = ImageInspection.imageCollection[0];

        Caman(this.targetCanvasId, function () {
            this.revert(false);
            this.resize({
                width: 300,
                height: 400
            });
            this.render();
        });

        this.prepareFilters(filtersId, rgbControlId, inputClass, valueClass);

        let canvas = document.createElement(this.el);
        canvas.width = 300; canvas.height = 400;

        let context = canvas.getContext(this.dimension);

        context.drawImage(this.targetCanvas, 0, 0, 300, 400);

        let data = context.getImageData(0, 0, 300, 400).data;
        var rgb = [data[0], data[1], data[2]];

        for (let i = 0; i < rgb.length; i++)
            rgb[i] = Math.round(rgb[i] / 255 * 100 * .25);

        this.imageRGBArray = rgb;

        uiCallback(this.imageRGBArray);

        Caman(inspection.targetCanvasId, function () {
            this.revert(false);
            this.render();
        });
    }

    prepareFilters = function (filtersId, rgbControlId, inputClass, valueClass) {
        var busy, caman, changed, filters, render,
            __hasProp = {}.hasOwnProperty;

        caman = null;
        filters = {};
        busy = false;
        changed = false;
        render = _.throttle(function () {
            var filter, value;
            if (busy) {
                changed = true;
                return;
            } else {
                changed = false;
            }
            busy = true;
            caman.revert(false);
            for (filter in filters) {
                if (!__hasProp.call(filters, filter)) continue;
                value = filters[filter];
                value = parseFloat(value, 10);
                if (value === 0) {
                    continue;
                }

                caman[filter](value);
            }
            return caman.render(function () {
                busy = false;
                if (changed) {
                    return render();
                }
            });
        }, 300);

        if (!($(this.targetCanvasId).length > 0)) {
            return;
        }

        caman = Caman(this.targetCanvasId);

        $(inputClass).each(function () {
            var filter;
            filter = $(this).data('filter');
            return filters[filter] = $(this).val();
        });

        $(`${filtersId},${rgbControlId}`).on('input', inputClass, function () {
            var filter, value;
            filter = $(this).data('filter');
            value = $(this).val();
            filters[filter] = value;
            $(this).find(`~ ${valueClass}`).html(value);
        });
    }
}

class PageUI {
    constructor(container, inspection) {
        this.container = container;
        this.inspection = inspection;
    }

    _init_ = function (ocCropCallback, pRGBCallback, resetCallback) {
        this.ocCropCallback = ocCropCallback;
        this.resetCallback = resetCallback;
        this.pRGBCallback = pRGBCallback;
    }

    showFilters = function (filtersId, rgbControlId, inputClass, valueClass) {
        inspection.drawNewCanvas(filtersId, rgbControlId, inputClass, valueClass,
            this.pRGBCallback);
        $(this.inspection.targetCanvas).css('display', 'block');
    }

    showLoader = function (elementId = null) {
        $(elementId || this.container).LoadingOverlay("show", {
            background: "rgba(255, 255, 255, 0.8)"
        });
    }
    hideLoader = function (elementId = null) {
        $(elementId || this.container).LoadingOverlay("hide");
    }
}