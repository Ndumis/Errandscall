/// <reference path="loader.js" />



class Config {

    constructor() {
        this.type = ValidationTypes.default; //default, append, prepend, symbols
        this.validSymbolLocation = "";
        this.errorSymbolLocation = "";
        this.valid = false;
        this.error = true;
        this.confirm = false;
        this.warning = false;
        this.warningFunction = function () {
            return "";
        };
        this.confirmationModalMessage = "Are you sure you would like submit";
        this.errorModalMessage = "";
        this.customSubmit = function () {
        };
        this.customSubmit = null;
    }

}


const ValidationTypes = {
    default: 'default',
    append: 'append',
    prepend: 'prepend',
    symbols: 'symbols'
};
class Validation {


    config = new Config();

    init = function (config = new Config(), formID) {
        //$.extend(this.config, config);



        jQuery.extend(jQuery.validator.messages, {
            required: function (e, element) {
                return ($(element).attr("placeholder") === undefined ? $(element).attr("name") : $(element).attr("placeholder")) + " required";
            },
            remote: "Please fix this field.",
            email: "Please enter a valid email address.",
            url: "Please enter a valid URL.",
            date: "Please enter a valid date.",
            dateISO: "Please enter a valid date (ISO).",
            number: "Please enter a valid number.",
            digits: "Please enter only digits.",
            creditcard: "Please enter a valid credit card number.",
            equalTo: "Please enter the same value again.",
            accept: "Please enter a value with a valid extension.",
            maxlength: jQuery.validator.format("Please enter no more than {0} characters."),
            minlength: jQuery.validator.format("Please enter at least {0} characters."),
            rangelength: jQuery.validator.format("Please enter a value between {0} and {1} characters long."),
            range: jQuery.validator.format("Please enter a value between {0} and {1}."),
            max: jQuery.validator.format("Please enter a value less than or equal to {0}."),
            min: jQuery.validator.format("Please enter a value greater than or equal to {0}.")
        });


        this.RefreshForm(config, formID);

    };
    RefreshForm = function (configs = new Config(), formID) {

        $((formID ? formID : "form") + ':not(.noblock) button, ' + (formID ? formID : "form") + ':not(.noblock) input[type="submit"], ' + (formID ? formID : "form") + ':not(.noblock) input[type="button"]').attr('disabled', 'disabled');

        $(formID ? formID : "form" + ':not(.noblock)').on('change keyup paste', ':input', function (e) {

            var keycode = e.which;

            if (e.type === 'paste' || e.type === 'change' || (
                (keycode === 46 || keycode === 8) || // delete & backspace
                (keycode > 47 && keycode < 58) || // number keys
                keycode === 32 || keycode === 13 || // spacebar & return key(s) (if you want to allow carriage returns)
                (keycode > 64 && keycode < 91) || // letter keys
                (keycode > 95 && keycode < 112) || // numpad keys
                (keycode > 185 && keycode < 193) || // ;=,-./` (in order)
                (keycode > 218 && keycode < 223))) { // [\]' (in order))

                $(this.form).find('button').removeAttr('disabled');
                $(this.form).find('input[type="submit"]').removeAttr('disabled');
                $(this.form).find('input[type="button"]').removeAttr('disabled');
            }

        });

        $(formID ? formID : "form" + ':not(.noblock) a').on('click', function (e) {
            $(this).find('button').removeAttr('disabled');
            $(this).find('input[type="submit"]').removeAttr('disabled');
            $(this).find('input[type="button"]').removeAttr('disabled');

        });

        $((formID ? formID : "form") + ':not(.noblock) ').on('drop', function (e) {
            $(this).find('button').removeAttr('disabled');
            $(this).find('input[type="submit"]').removeAttr('disabled');
            $(this).find('input[type="button"]').removeAttr('disabled');

        });
        $(formID ? formID : "form").each(function () {
            if (formID !== "form") {
                $(this).validate().destroy();
            }
            var config = configs;
            $(this).validate({
                rules: {
                },
                submitHandler: function (form, event) {
                    if (!$(form).valid()) {
                        event.preventDefault();
                        return;
                    }

                    if (config.confirm && !$('#exampleModal').hasClass('show')) {
                        event.preventDefault();
                        if (config.warning) {
                            $('#exampleModalLabel').html(config.warningFunction());

                        } else {
                            $('#exampleModalLabel').html(config.confirmationModalMessage);

                        }
                        $('#exampleModal').modal();
                        $('#btnSaveChanges').unbind('click');
                        $('#btnSaveChanges').click(function () {
                            if (config.customSubmit !== null) {
                                config.customSubmit();
                            }
                            else {
                                $(form).submit();
                            }
                            $('#exampleModal').modal('hide');
                        });
                    }

                    if (config.customSubmit !== null && !config.confirm) {
                        $('#exampleModal').modal('hide');
                        config.customSubmit();
                    }

                    if (config.customSubmit !== null || (config.confirm && !$('#exampleModal').hasClass('show'))) {
                        return false;
                    }
                    return true;
                    //$(form).submit();
                },
                invalidHandler: function (_event, validator) {
                    // 'this' refers to the form
                    if (!$('#' + this.id).hasClass('ErrorAlert')) {
                        if (config.type === ValidationTypes.append)
                            $('#' + this.id).append($('.erorrTemplate .ErrorAlert')[0]);
                        else if (config.type === ValidationTypes.prepend)
                            $('#' + this.id).prepend($('.erorrTemplate .ErrorAlert')[0]);
                    }
                    var errors = validator.numberOfInvalids();
                },
                messages: {
                    name: "Please specify your name",
                    email: {
                        required: "We need your email address to contact you",
                        email: "Your email address must be in the format of name@domain.com"
                    }
                },
                errorClass: "invalid",
                validClass: "success",
                errorElement: config.type === ValidationTypes.symbols ? 'img' : 'label',
                errorPlacement: function (error, element) {
                    if (config.type === ValidationTypes.symbols) {
                        if (!$(element).next().hasClass("input-group-btn")) {
                            $(`<div class="input-group">
                         <div id="replacement"></div>
                        <span class="input-group-btn">
                                <div id="replacementError"></div>
                        </span>
                </div>`).insertAfter(element);

                            $('#replacement').replaceWith(element);
                            $(error).attr('src', config.errorSymbolLocation);
                            $('#replacementError').replaceWith(error);
                            $(error).attr('title', $(error).text());
                            $(error).tooltip();
                        }

                    }
                    if (config.type !== ValidationTypes.default) {
                        $(element).parents('form').find('.ErrorAlert').append(error);
                    }
                    else {
                        $(error).insertAfter(element);
                    }
                    $(error).mouseenter(function () {
                        $(element).addClass("onFocus");
                    });
                    $(error).mouseleave(function () {
                        $(element).removeClass("onFocus");
                    });
                }
            });

            //$(this).removeAttr('novalidate');

        });
    };
}


jQuery.validator.addMethod("mobile", function (value, element) {
    //return false;
    var regEx = /^\+[\d ]+$/;
    return regEx.test(value);
}, "Mobile number requires a + and numbers");



const ActionTypes = {
    hide: 1,
    disable: 2,
    show: 3
};
class AccessConfig {

    constructor() {
        this.options = {
            1: 1
        };
        this.options = {};
    }

}


class Access {
    config = new AccessConfig();
    init = function (config = new AccessConfig()) {
        $.extend(this.config, config);

        $.each(config.options, function (key, value) {
            var $element = $('*[data-actionid=' + key + ']');
            switch (value) {
                case ActionTypes.hide:
                    $element.hide();
                    break;
                case ActionTypes.disable:
                    $element.prop('disabled', true);
                    $element.removeAttr('href');
                    $element.removeAttr('src');
                    $element.click(function (e) {
                        e.preventDefault();
                        //do other stuff when a click happens
                    });
                    break;
                case ActionTypes.show:
                    $element.show();
                    break;
                default:
                    break;
            }
        });

    };

}


var idleTime = 0;
var href = null;
$(document).ready(function () {

    $(document).on("click", "a", function (e) {

        if ($(this).attr('id') !== 'btnSessionsEnd') {
            e.preventDefault();

            href = $(this).attr("href");

            CheckLink(href);
            //put the logic to update the href

            //finally relocate to that URL as you would have intended to
        }
        else {
            location.href = href;
        }



    });

    //Increment the idle time counter every minute.
    var idleInterval = setInterval(timerIncrement, 10000); // 1 minute

    //Zero the idle timer on mouse movement.
    $(this).mousemove(function (e) {
        countdown = 10;
        idleTime = 0;
        if (bar !== null) {
            bar.destroy();
            bar = null;
            $('#AutoLogoutModal').modal('hide');
        }
    });
    $(this).keypress(function (e) {
        countdown = 10;
        idleTime = 0;
        if (bar !== null) {
            bar.destroy();
            bar = null;
            $('#AutoLogoutModal').modal('hide');
        }
    });

    SetNavActive();

    $(window).on("blur focus", function (e) {
        var prevType = $(this).data("prevType");

        if (prevType !== e.type) {   //  reduce double fire issues
            switch (e.type) {
                case "blur":
                    HasFocus = false;
                    break;
                case "focus":
                    HasFocus = true;
                    break;
            }
        }

        $(this).data("prevType", e.type);
    });

});

var HasFocus = true;
var countdown = 10;
var bar = null;
function timerIncrement() {
    if (HasFocus) {
        idleTime = idleTime + 1;
    }
    //ShowDangerAlert(idleTime + ' minutes')
    if (idleTime === 50) { // 20 minutes

        bar = new ProgressBar.Circle('#containerTimer', {
            color: '#aaa',
            // This has to be the same size as the maximum width to
            // prevent clipping
            strokeWidth: 4,
            trailWidth: 1,
            easing: 'linear',
            duration: 10000,
            text: {
                autoStyleContainer: false
            },
            from: { color: '#aaa', width: 1 },
            to: { color: '#333', width: 4 },
            // Set default step function for all animate calls
            step: function (state, circle) {
                circle.path.setAttribute('stroke', state.color);
                circle.path.setAttribute('stroke-width', state.width);

                var value = Math.round(10 - (circle.value() * 10));
                if (value === 0) {
                    circle.setText('');
                } else {
                    circle.setText(value);
                }

            }
        });
        bar.text.style.fontFamily = '"Raleway", Helvetica, sans-serif';
        bar.text.style.fontSize = '2rem';

        bar.animate(1.0);  // Number from 0.0 to 1.0

        $('#AutoLogoutModal').modal('show');
    }

    if (idleTime === 51) { // 20 minutes
        Logout(true);
        bar.destroy();
        bar = null;
    }
};