function DragDropInit(LeftMenu, RightMenu) {
    if ($(LeftMenu).height() > $(RightMenu).height()) {
        $(RightMenu).css('min-height', $(LeftMenu).height());
        $(LeftMenu).css('min-height', $(RightMenu).height());
    }
    else {
        $(LeftMenu).css('min-height', $(RightMenu).height());
        $(RightMenu).css('min-height', $(LeftMenu).height());
    }


    $(LeftMenu + ' .row').draggable({
        classes: {
            "ui-draggable": "highlight"
        },
        helper: "clone",
        snap: true,
        appendTo: RightMenu
    });

    $(RightMenu).droppable({
        accept: LeftMenu + " .row",
        drop: function (event, ui) {
            $(ui.draggable).appendTo(RightMenu).removeClass('selectedRow');
        }
    });

    $(RightMenu + ' .row').draggable({
        classes: {
            "ui-draggable": "highlight"
        },
        helper: "clone",
        snap: true,
        appendTo: LeftMenu
    });


    $(LeftMenu).droppable({
        accept: RightMenu + " .row",
        drop: function (event, ui) {
            $(ui.draggable).appendTo(LeftMenu).removeClass('selectedRow');
        }
    });

    $(LeftMenu + ' .row, ' + RightMenu + ' .row').click(function (e) {
        if (!$(this).hasClass('selectedRow')) {
            $(this).addClass('selectedRow');
        } else {
            $(this).removeClass('selectedRow');
        }
    });


}

function MoveRight(LeftMenu, RightMenu) {
    $(LeftMenu + ' .row.selectedRow').appendTo(RightMenu).removeClass('selectedRow');
}

function MoveLeft(LeftMenu, RightMenu) {
    $(RightMenu + ' .row.selectedRow').appendTo(LeftMenu).removeClass('selectedRow');
}

function DateTimeInit(element) {
    $('.datetime').datetimepicker({
        format: "YmdHis"
    });
}

function generatePassword() {
    var length = 8,
        charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
        retVal = "";
    for (var i = 0, n = charset.length; i < length; ++i) {
        retVal += charset.charAt(Math.floor(Math.random() * n));
    }
    return retVal;
}

function ShowAlert(message, type) {
    if (type === 'success') {
        return ShowSuccessAlert(message);
    }
    else if (type === 'info') {
        return ShowInfoAlert(message);
    }
    else if (type === 'warning') {
        return ShowWarningAlert(message);
    }
    else if (type === 'danger') {
        return ShowDangerAlert(message);
    }
}


function ShowSuccessAlert(message) {
    return Alert(message, 'fa fa-thumbs-up', 'Success:', 'success');
}

function ShowInfoAlert(message) {
    return Alert(message, 'fa fa-question-circle', '', 'info');
}

function ShowWarningAlert(message) {
    return Alert(message, 'fa fa-exclamation-triangle', 'Sorry:', 'warning');
}

function ShowDangerAlert(message) {
    return Alert(message, 'fa fa-exclamation-circle', 'Error:', 'danger');
}

function ShowLoadingAlert(message) {
    return Alert(message, 'fa fa-spinner fa-spin', '', 'info', false);
}


function Alert(message, icon, title, type, showProgressbar) {

    if (!showProgressbar)
        showProgressbar = false;
    return $.notify({
        // options
        icon: icon,
        title: title,
        message: message
    }, {
        // settings
        element: 'body',
        position: null,
        type: type,
        allow_dismiss: true,
        newest_on_top: true,
        showProgressbar: showProgressbar,
        placement: {
            from: "top",
            align: "right"
        },
        offset: 20,
        spacing: 10,
        z_index: 10000,
        delay: 1000,
        timer: type === 'info' || type === 'danger' ? 0 : 1000,
        url_target: '_blank',
        mouse_over: null,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
        },
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        icon_type: 'class',
        allow_duplicates: true
    });
}

function ReturnMessage(result) {
    if (result.Message) {
        ShowAlert(result.Message, result.AlertType);
    }
}

function SetPlainDatatable() {
    $('#dataTable').DataTable({
    });
}

function SetDatatable(fn, appendId) {
    $('#dataTable').DataTable({
        "initComplete": function (settings, json) {
            CreateSearchButton(fn, appendId);
        }
    });
}

function SetFilterDatatable(fn, appendId, options, label, SelectedID) {
    $('#dataTable').DataTable({
        "initComplete": function (settings, json) {
            CreateFilterButton(fn, appendId, options, label, SelectedID);
        }
    });
}

function CreateSearchButton(fn, appendId) {
    var button = document.createElement("button");
    button.id = "datatable-search";
    button.onclick = fn;
    button.className = "btn btn-primary";
    button.innerText = "Search";

    $(appendId ? appendId : '#dataTable_filter').append(button);
    return button;
}

function CreateFilterButton(fn, appendId, options, labeltext, SelectedID) {
    var select = document.createElement("select");
    select.id = "datatable-filter";
    select.onchange = fn;
    select.className = "form-control form-control-sm";
    $.each(options, function (key, value) {
        var $option = $("<option></option>");
        $option.attr("value", key)
            .text(value);

        if (SelectedID === key) {
            $option.attr("value", key)
                .attr("selected", "selected");
        }
        else if (!SelectedID) {
            $option.attr("value", key)
                .attr("selected", "selected");
        }
        $(select)
            .append($option);
    });


    var label = document.createElement("label");
    label.className = "";
    label.textContent = labeltext;
    var $div12 = $('<div class="col-md-2" style="padding-left: 0;"></div>');
    $div12.append(label);
    $div12.append(select);

    $(appendId ? appendId : '#dataTable_wrapper > .row:first-child').append($div12);


    return select;
}


function SetNavActive(navActive = 'Dasboard') {
    $('li[data-navactive="' + navActive + '"]').addClass('active');
}

$('.cardSelectable').click(function () {
    $('.cardSelectable').removeClass('selected');
    $(this).addClass('selected');
});

var SelectedLink = null;
function LookupShow(UrL, func, element) {
    $('#LookupModal').modal({
        show: true
    });

    $.ajax(
        {
            url: UrL,
            type: 'GET',
            success: function (result) {
                $('#LookupBody').html(result);
            }
        }
    );
    SelectedLink = element;
}


function SelectLookup(Value, Description) {
    $('#LookupModal').modal('hide');

    var parent = $(SelectedLink).parent('div');
    $(parent.find('label')[1]).text(Description);
    parent.find('input:hidden').val(Value);
}

function HelpdeskShow(UrL, func, element) {

    $('#HelpdeskModal').modal({
        backdrop: true,
        show: true,
        focus: true
    });
    $('#HelpdeskModal').css('left', $('#ui-id-1').offset().left - $('#HelpdeskModal').width());
    $('#HelpdeskModal').css('top', $('#ui-id-1').offset().top);
    $('#HelpdeskModal .modal-content, #HelpdeskModal .modal-body').css('min-height', $('#ui-id-1').outerHeight(true));

    $.ajax(
        {
            url: UrL,
            type: 'GET',
            success: function (result) {
                $('#HelpdeskBody').html(result);
            }
        }
    );
    SelectedLink = element;
}


function HelpdeskLookup(Value, Description) {
    $('#LookupModal').modal('hide');

    var parent = $(SelectedLink).parent('div');
    $(parent.find('label')[1]).text(Description);
    parent.find('input:hidden').val(Value);
}

if (jQuery.ui && jQuery.ui.autocomplete) {
    //override the autocomplete widget
    jQuery.widget("ui.autocomplete", jQuery.ui.autocomplete, {
        _close: function (event) {
            if (event !== undefined && event.keepOpen === true) {
                //trigger new search with current value
                this.search(null, event);
                return true;
            }
            //otherwise invoke the original
            return this._super(event);
        }
    });

    $("#search-input").autocomplete({
        open: function (event, ui) {
            $('#RenderBody').addClass('blur');
        },
        close: function (event, ui) {
            $('#RenderBody').removeClass('blur');
        },
        source: function (request, response) {
            $.ajax({
                url: "/Lookup/CustomerProfileSearch",
                dataType: "json",
                data: {
                    Search: request.term
                },
                success: function (data) {
                    response(data);
                }
            });
        },
        appendTo: '.navbar-nav.ml-auto',
        minLength: 3,
        select: function (event, ui) {
            var URL = '/Helpdesk/_CustomerView?CustomerID=' + ui.item.ID;
            HelpdeskShow(URL, null, this);
            return false;
        }
    })
        .autocomplete("instance")._renderItem = function (ul, item) {
            var result = Template.replace("{SRC}", item.Data);
            result = result.replace("{Firstname}", item.FirstNames ? item.FirstNames : "");
            result = result.replace("{Surname}", item.Surname ? item.Surname : "");
            result = result.replace("{Email}", item.EmailAddress ? item.EmailAddress : "");
            result = result.replace("{ID}", item.MobileNumber ? item.MobileNumber : "");
            return $("<li>")
                .append(result)
                .appendTo(ul);
        };

    var canClose = false;
    var originalCloseMethod = $("#search-input").autocomplete("instance").close;
    $("#search-input").autocomplete("instance").close = function (event, ui) {
        //event.keepOpen = false;
        if (canClose) {
            //close requested by someone else, let it pass
            originalCloseMethod.apply(this, arguments);
        }
        canClose = false;
        return false;
    };


    //    return Json(Customer.Select(n => new { n.FirstNames , n.Surname, n.EmailAddress, n.MobileNumber}), JsonRequestBehavior.AllowGet);
    var Template = `<div class="row">
    <div class="col-md-2">
        <img style="height: 72px;padding:5px 0 0 5px;" src="{SRC}">
    </div>
    <div class="col-md-9">
        <div class="row">
            <div class="col-md-6">
                Firstname  <br /> {Firstname}
            </div>
            <div class="col-md-6">
                ID  <br /> {ID}
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                Surname  <br /> {Surname}
            </div>
            <div class="col-md-6">
                Email  <br /> {Email}
            </div>
        </div>
    </div>
</div>`;
}






function expand() {
    if (!$('#search-btn').hasClass('close')) {
        $('#search-input').focus();
    } else {
        canClose = true;
        $('#HelpdeskModal').modal('hide');
        $("#search-input").autocomplete('close');
        $('#RenderBody').removeClass('blur');
    }
    $('#search-btn').toggleClass('close');
    $('#search-input').toggleClass('square');
}

