var myHub = null;

(function () {
    // Defining a connection to the server hub.
    myHub = $.connection.myHub;
    // Setting logging to true so that we can see whats happening in the browser console log. [OPTIONAL]
    $.connection.hub.logging = true;
    // Start the hub
    $.connection.hub.start().done(function () {
        myHub.server.checkExclusivePage(location.pathname);

        setInterval(function () {
            myHub.server.checkNotification();
        }, 3000);
    });

    // This is the client method which is being called inside the MyHub constructor method every 3 seconds
    myHub.client.SendNotification = function (Notifications) {
        // Set the received serverTime in the span to show in browser
        $("#bellCounter").text(Notifications);
    };

    // This is the client method which is being called inside the MyHub constructor method every 3 seconds
    myHub.client.SendNotificationMessages = function (Notifications) {
        $('#dropdownMessages *:not(.dropdown-header, .dropdown-header *)').remove();
        $(Notifications).each(function () {
            var notificationTemplate = NotificationTemplate;
            notificationTemplate = notificationTemplate.replace('{icon}', this.Icon);
            notificationTemplate = notificationTemplate.replace('{Message}', this.Messge);
            notificationTemplate = notificationTemplate.replace('{Date}', this.Date);
            
            $('#dropdownMessages').append(notificationTemplate);
        });
    };

    // Client method to broadcast the message
    myHub.client.hello = function (message) {
        $("#message").text(message);
    };

    //Button click jquery handler
    $("#btnClick").click(function () {
        // Call SignalR hub method
        myHub.server.helloServer();
    });

    myHub.client.IsExlcusive = function (IsExlcusive) {
        if (IsExlcusive) {
            $("#SessionEndModal").modal("show");
            $("#btnSessionsEnd").attr(href);
        } else {
            //debugger
            myHub.server.disconnect();
            location.href = href;
        }
    };
    $("#btnSessionsEnd").on('click', function () {
            //debugger
        myHub.server.disconnect();

    });


    myHub.client.ShowMessage = function (messsage) {
        ShowSuccessAlert(messsage);
    };

    myHub.client.logoutcomplete = function (logout) {
        //$('#Logout').click();
        if (logout) {
            window.location = "/Login/Login";

        } else {
            window.location = "/Home/Index";
            browser.history.deleteAll();
        }
    };
})();

function Logout(Logout) {
    Logout = Logout ? true : false;
    myHub.server.logout(Logout);
}

var tries = 0;
function CheckLink(url) {
    try {
        myHub.server.checkLink(url);
    } catch (e) {
        tries++;
        if (tries === 2) {
            window.location = "/Login/Login";
        }
        return;
    }
    tries = 0;
}

function NotificationGet() {

}

var NotificationTemplate =
    `<a class="dropdown-item d-flex align-items-center" href="#">
                                    <div class="mr-3">
                                        <div class="icon-circle bg-primary">
                                            <i class="fas {icon} text-white"></i>
                                        </div>
                                    </div>
                                    <div>
                                        <div class="small text-gray-500">{Date}</div>
                                        <span class="font-weight-bold">{Message}</span>
                                    </div>
                                </a>`;