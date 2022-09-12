//      ============================================         Varialable        =====================================================

var protocal = "http";
var domainName = "localhost";
var portNo = "9020";

var deviceInfos;
var selectedDeviceIndex;
var listOfObjects = [];
var result = [];

var g_isCapturing = false;
var g_isSensorOn = false;
var g_isFingerOn = false;
var g_hasFinger = false;

var urlString = protocal + "://" + domainName + ":" + portNo;
var userTemplates = [];
var gToastTimeout = 0;

//      ============================================         Function        =====================================================

function IsIEbrowser() {
    var browser = navigator.userAgent.toLowerCase();

    if ((browser.indexOf("chrome") !== -1 || browser.indexOf("firefox") !== -1) && browser.indexOf("edge") === -1) {
        return 0;
    }
    else {
        if (browser.indexOf("msie") !== -1 || (navigator.appName === 'Netscape' && navigator.userAgent.search('Trident') !== -1) || browser.indexOf("edge") !== -1) {
            return 1;
        }
    }
}

function initializeAPI() {

    $("#Fpimg").removeAttr();

    var DeviceIndex = $('#dd_devicelist').val();

    jQuery.ajax(
        {
            type: "POST",
            url: urlString + "/bm/initializeAPI",
            dataType: "json",
            data: {
                DeviceIndex: DeviceIndex
            },
            success: function (data) {
                //alert(data.ScannerCount);
                if (data === null) {
                    //alert('An error occurred: \nPlease start the neaBioMini API and continue.');
                    ShowDangerAlert(`Please connect the scanner and try again<br> <a class="btn btn-primary" href="/Login/Login">
                                                <i class="fas fa-sync-alt"></i>
                                            </a>`);
                    return;
                }

                if (data.ScannerCount > 0) {

                    if (data.retValue === "0") {
                        deviceInfos = data.ScannerInfos;
                        //alert(data.retValue);
                        AddScannerList(deviceInfos);
                        //CheckStatus();
                    }
                    else {
                        // alert('An error occurred: \nReconnect the scanner and try again.');

                        ShowDangerAlert(`Please connect the scanner and try again<br> <a class="btn btn-primary" href="/Login/Login">
                                                <i class="fas fa-sync-alt"></i>
                                            </a>`);

                    }
                }
                else {
                    //alert('Please connect the scanner and try again');                    
                }
            },
            error: function (request, status, error) {
                ShowDangerAlert("Cannot initiate API", gToastTimeout);
                //ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                //ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                //ShowDangerAlert(JSON.stringify(error), gToastTimeout);
            }
        });
}

function GetDeviceList() {

    $("#Fpimg").removeAttr();

    jQuery.ajax(
        {
            type: "GET",
            url: urlString + "/bm/getDeviceList",
            dataType: "json",
            success: function (data) {
                //debugger
                if (data === null) {
                    ShowDangerAlert('An error occurred: \nPlease start the neaBioMini API and continue.');
                    return;
                }


                if (data.ScannerCount > 0) {

                    if (data.retValue === "0") {
                        deviceInfos = data.ScannerInfos;
                        AddScannerList(deviceInfos);
                        //CheckStatus();
                    }
                }
                else {

                    //ShowDangerAlert('Device not detected');

                }
            },
            error: function (request, status, error) {
                //ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                //ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                //ShowDangerAlert(JSON.stringify(error), gToastTimeout);
            }
        });
}

function AddScannerList(ScannerInfos) {
    var count = -1;
    $("#dd_devicelist").attr("innerHTML", "");

    $.each(ScannerInfos, function (key) {
        strBuffer = "[" + ScannerInfos[key].DeviceIndex + "]" + ScannerInfos[key].DeviceType + " (" + ScannerInfos[key].ScannerName + ")";
        $("#dd_devicelist").append("<option value='" + key + "' class='ScannerListOptions' >" + strBuffer + "</option>");
        count = key;
    });
    count = count + 1;

    if ($("#dd_devicelist option:selected").attr("value") === undefined)
        selectedDeviceIndex = 0;
    else
        selectedDeviceIndex = $("#dd_devicelist")[0].selectedIndex;

    jQuery.ajax
        ({
            type: "GET",
            url: urlString + "/bm/getScannerParameters?deviceIndex=" + selectedDeviceIndex,
            dataType: "json",
            success: function (data) {
                if (data !== null) {
                    $("#dd_brightness").attr("value", data.Brightness);
                    $("#cb_fastmode").attr("checked", data.Fastmode);
                    $("#dd_securitylevel > option[value=" + data.SecurityLevel + "]").attr("selected", true);
                    $("#dd_sensitivity").attr("value", data.Sensitivity);
                    $("#dd_timeout > option[value=" + data.Timeout / 1000 + "]").attr("selected", true);
                    $("#dd_templatetype  > option[value=" + data.TemplateType + "]").attr("selected", true);
                    //$("#Tb_BrightnessValue").attr("value" , data.FakeLevel);
                    $("#cb_detectcode").attr("checked", data.DetectFakeAdvancedMode);
                }
            }
        });
}

function getScannerParameters() {
    if ($("#dd_devicelist option:selected").attr("value") === undefined)
        selectedDeviceIndex = 0;
    else
        selectedDeviceIndex = $("#dd_devicelist")[0].selectedIndex;

    jQuery.ajax
        ({
            type: "GET",
            url: urlString + "/bm/getScannerParameters?deviceIndex=" + selectedDeviceIndex,
            dataType: "json",
            success: function (data) {
                if (data !== null) {
                    $("#dd_brightness").attr("value", data.Brightness);
                    $("#cb_fastmode").attr("checked", data.Fastmode);
                    $("#dd_securitylevel > option[value=" + data.SecurityLevel + "]").attr("selected", true);
                    $("#dd_sensitivity").attr("value", data.Sensitivity);
                    $("#dd_timeout > option[value=" + data.Timeout / 1000 + "]").attr("selected", true);
                    $("#dd_templatetype  > option[value=" + data.TemplateType + "]").attr("selected", true);
                    //$("#Tb_BrightnessValue").attr("value" , data.FakeLevel);
                    $("#cb_detectcode").attr("checked", data.DetectFakeAdvancedMode);
                }
            }
        });
}

function GetScannerStatus() {
    selectedDeviceIndex = $("#dd_devicelist").attr("value");

    jQuery.ajax({
        type: "GET",
        url: urlStr + "/bm/GetScannerStatus?deviceIndex=" + selectedDeviceIndex,
        dataType: "json",
        success: function (data) {
            if (data !== null) {
                CheckStatus();
            }
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function uninitializeAPI() {
    jQuery.ajax(
        {
            type: "POST",
            url: urlString + "/bm/uninitializeAPI",
            dataType: "json",
            data: {
                DeviceIndex: $("#dd_devicelist").attr("value")
            },
            success: function (data) {
                if (data === 0) {
                    $("#dd_devicelist").empty();
                }
                else if (data === -1) {
                    alert('Error occurred');
                }
            },
            error: function (request, status, error) {
                ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                ShowDangerAlert(JSON.stringify(error), gToastTimeout);
            }
        });
}

function StartCapture(_userTemplates) {
    userTemplates = _userTemplates;
    g_hasFinger = false;
    var DeviceIndex = $('#dd_devicelist').val();

    jQuery.ajax({
        type: "POST",
        url: urlString + "/bm/startCapture",
        dataType: "json",
        data: {
            DeviceIndex: DeviceIndex
        },
        success: function (data) {
            if (data.retValue === -1) {
                ShowDangerAlert('Failed to get the connected scanner, please try again');
            }
            else {
                document.getElementById("ta_log").value = data.template;
                GetBiominiImageLivePreviewer();
                IdentifyWithTemplate();
                //VerifyWithTemplate();
            }
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function SingleCapture() {
    var delayVal = 30000;

    jQuery.ajax(
        {
            type: "POST",
            url: urlString + "/bm/singleCapture",
            dataType: "json",
            data: {
                DeviceIndex: $("#dd_devicelist").attr("value")
            },
            success: function (data) {
                if (data === -1) {
                    alert('Error occurred');
                }
                else {
                    document.getElementById("ta_log").value = "Template: " + "\n" + data.template + "\n\n" + "Image: " + "\n" + data.image;
                    GetBiominiImageLivePreviewer();
                    $("#ta_log").attr('src', "data:image/bmp;base64," + data.image);
                }
            },
            error: function (request, status, error) {
                ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                ShowDangerAlert(JSON.stringify(error), gToastTimeout);
            }
        });
}

function AbortCapture() {

    jQuery.ajax({
        type: "POST",
        url: urlString + "/bm/abortCapture",
        dataType: "json",
        data: {
            DeviceIndex: $("#dd_devicelist").attr("value")
        },
        success: function (data) {
            if (data === -1) {
                alert('Failed to abort capture');
            }
            else {
                alert('Abort capture success');
            }
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function GetBiominiImageLivePreviewer() {
    if (g_hasFinger) {
        return;
    }
    //var sessionData = "&shandle=" + deviceInfos[selectedDeviceIndex].DeviceHandle + "&id=" + pageID;
    var getImageFromData = "";

    var IEflag = IsIEbrowser();

    if (IEflag === 1) {
        if (g_isCapturing === false && g_isSensorOn === false && g_isFingerOn === false) {
            $("#Fpimg").removeAttr();

            jQuery.ajax({
                type: "GET",
                url: urlString + "/bm/getBiominiImageLivePreviewer",
                dataType: "json",
                success: function (data) {
                    if (data !== null) {
                        getImageFromData = data;
                        //var imgUrl = urlStr + "img/img.bmp?dummy=";
                        $("#Fpimg").attr('src', "data:image/bmp;base64," + getImageFromData);


                    }
                },
                error: function (request, status, error) {
                    ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                    ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                    ShowDangerAlert(JSON.stringify(error), gToastTimeout);
                }
            });

            pLoopflag = setTimeout(GetBiominiImageLivePreviewer, 150);
            gPreviewFaileCount = 0;
        }
        else if (gPreviewFaileCount < 60) {
            pLoopflag = setTimeout(GetBiominiImageLivePreviewer, 1000);
            gPreviewFaileCount++;
        }
        else {
            gPreviewFaileCount = 0;
            $('#Fpimg').attr('src', 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==');
        }
    }
    else {
        if (g_isCapturing === false && g_isSensorOn === false && g_isFingerOn === false) {
            $("#Fpimg").removeAttr();

            jQuery.ajax({
                type: "GET",
                url: urlString + "/bm/getBiominiImageLivePreviewer",
                dataType: "json",
                success: function (data) {
                    if (data !== null) {
                        getImageFromData = data;
                        //var imgUrl = urlStr + "img/img.bmp?dummy=";
                        $("#Fpimg").attr('src', "data:image/bmp;base64," + getImageFromData);


                    }
                },
                error: function (request, status, error) {
                    ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                    ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                    ShowDangerAlert(JSON.stringify(error), gToastTimeout);
                }
            });

            pLoopflag = setTimeout(GetBiominiImageLivePreviewer, 150);
            gPreviewFaileCount = 0;
        }
        else if (gPreviewFaileCount < 60) {
            pLoopflag = setTimeout(GetBiominiImageLivePreviewer, 1000);
            gPreviewFaileCount++;
        }
        else {
            gPreviewFaileCount = 0;
            $('#Fpimg').attr('src', 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==');
        }
    }
}

function GetScannerStatus2() {
    if ($("#dd_devicelist option:selected").attr("value") === undefined)
        selectedDeviceIndex = 0;
    else
        selectedDeviceIndex = $("#dd_devicelist")[0].selectedIndex;

    jQuery.ajax(
        {
            type: "GET",
            url: urlString + "/bm/getScannerStatus?deviceIndex=" + selectedDeviceIndex,
            dataType: "json",
            success: function (data) {
                if (data !== null) {
                    g_isCapturing = data.IsCapturing;
                    g_isSensorOn = data.SensorOn;
                    g_isFingerOn = data.IsFingeOn;
                }

                getParameters();
            },
            error: function (request, status, error) {
                ShowDangerAlert(JSON.stringify(request), gToastTimeout);
                ShowDangerAlert(JSON.stringify(status), gToastTimeout);
                ShowDangerAlert(JSON.stringify(error), gToastTimeout);
            }
        }
    );
}

var params = null;
function SetScannerParameters(_ScannerParameters) {
    //debugger
    params = _ScannerParameters;

    jQuery.ajax({
        type: "POST",
        url: urlString + "/bm/setScannerParameters",
        dataType: "json",
        data: _ScannerParameters,
        success: function (data) {
            //debugger
            if (data === 0) {
                //alert('The parameters were successfully changed.');
            }
            else if (data === -1) {
                ShowAlert('Failed to get scanner.');

                GetDeviceList();
                SetScannerParameters(params);
            }
            else if (data === -2) {
                ShowAlert('Sensitivity value is invalid.');
            }
            else {
                ShowAlert('The parameters were not changed.');
            }
        },
        error: function (request, status, error) {
            //ShowDangerAlert("Could not connect to service", gToastTimeout);
            //ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            //ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            //ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function GetTemplate() {
    jQuery.ajax({
        type: "GET",
        url: urlString + "/bm/getTemplate",
        dataType: "json",
        success: function (data) {
            if (data !== null) {

                var sampleArr = base64ToArrayBuffer(data);
                saveByteArray("Template", sampleArr);

                AppendLogData(data);
            } else {
                ShowDangerAlert('Failed to get template');
            }
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function GetImage() {
    jQuery.ajax({
        type: "GET",
        url: urlString + "/bm/getImage",
        dataType: "json",
        success: function (data) {
            if (data !== null) {
                AppendLogData(data);
            } else {
                ShowDangerAlert('Failed to get image');
            }
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function VerifyWithTemplate() {

    var template1 = $('#ta_log').val();
    var Templates = userTemplates;
    debugger

    jQuery.ajax({
        type: "POST",
        url: urlString + "/bm/verifyWithTemplate",
        dataType: "json",
        data: {
            template1: template1,
            Templates: Templates
        },
        success: function (data) {

            if (data === 0) {
                ShowSuccessAlert("Success");
                //Direct to Home page   
                //window.location.href = '@Url.Action("Index", "Home", new { Success=1 })';
                window.location = 'Home/Index';

                return;
            }

            if (data === 1) {
                ShowDangerAlert("Templates did not match");
                return;
            }

            if (data === -1 || data === -2) {
                ShowDangerAlert("Failed");
            }

        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

window.onload = function () {

    document.getElementById('fileInput').addEventListener('change', function (event) {
        var files = event.target.files;

        // Initialize an instance of the `FileReader`
        var reader = new FileReader();

        // Specify the handler for the `load` event
        reader.onload = (function (theFile) {

            return function (e) {

                var binaryData = e.target.result;
                var base64String = window.btoa(binaryData);

                listOfObjects.push(base64String);

                console.log(e.target.result);
            };
        })(files);
        // Read the file     

        reader.readAsBinaryString(files[0]);



    }, false);
};

function IdentifyWithTemplate() {
    var template1 = $('#ta_log').val();

    var Templates = userTemplates;

    if (template1 === null || template1 === "") {
        ShowDangerAlert('Could not find template');
        return;
    }

    //alert(Templates);

    jQuery.ajax({
        type: "POST",
        url: urlString + "/bm/identifyWithTemplate",
        dataType: "json",
        data: {
            template1: template1,
            Templates: Templates
        },
        success: function (data) {
            debugger
            if (data[0] === -1 || data[0] === -2 ) {
                ShowDangerAlert("Templates did not match");
                g_hasFinger = true;
                $('#Fpimg').removeAttr('src');
            }
            else {

                if (data.length !== 0) {
                    for (var i = 0; i < data.length; i++) {
                        //ShowDangerAlert("Template match is " + data[i]);

                        $.ajax({
                            url: '/Login/SignIn',
                            type: 'POST',
                            data: { UserName: $('#UserName').val() },
                            success: function (result) {
                                window.location = '/Dashboard/Index';
                            },
                            error: function (result) {
                                window.location = '/Login/Login';
                            }
                        });
                    }
                    return;
                }
                else {
                    ShowDangerAlert('Templates did not match');
                    g_hasFinger = true;
                    $('#Fpimg').removeAttr('src');


                }
            }
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function verfiyWithTemplate(currentTemplate) {
    jQuery.ajax({
        type: "POST",
        url: urlString + "/bm/verifyWithTemplate",
        dataType: "json",
        data: {
            temp: currentTemplate,
            status: 0
        },
        success: function (data) {
            if (data === 0)
                alert("Success");
            else
                alert("Failed");
        },
        error: function (request, status, error) {
            ShowDangerAlert(JSON.stringify(request), gToastTimeout);
            ShowDangerAlert(JSON.stringify(status), gToastTimeout);
            ShowDangerAlert(JSON.stringify(error), gToastTimeout);
        }
    });
}

function AppendLogData(text) {
    //var originText = document.getElementById("Tb_DisplayLog").value;
    document.getElementById("ta_log").value = text + "\n";
    var ta = document.getElementById('ta_log');
    ta.scrollTop = ta.scrollHeight;
}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}

function saveByteArray(reportName, byte) {
    var blob = new Blob([byte], { type: "text/tmp" });
    var link = document.createElement('a');
    //link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.innerHTML = "Save Template File";

    if (window.webkitURL !== null) {
        // Chrome allows the link to be clicked
        // without actually adding it to the DOM.
        link.href = window.webkitURL.createObjectURL(blob);
    }
    else {
        // Firefox requires the link to be added to the DOM
        // before it can be clicked.
        link.href = window.URL.createObjectURL(blob);
        link.onclick = destroyClickedElement;
        link.style.display = "none";
        document.body.appendChild(link);
    }

    link.click();
}

function destroyClickedElement(event) {
    document.body.removeChild(event.target);
}

