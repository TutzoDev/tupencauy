$(function () {
    $('#createNotificationButton').on('click', function () {
        $.ajax({
            url: '/Notification/CreateSampleNotification',
            type: 'POST',
            success: function () {
                alert("Notificación de ejemplo creada");
            },
            error: function () {
                alert("Error al crear la notificación de ejemplo");
            }
        });
    });

    const connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

    connection.start().then(function () {
        console.log("Conectado a SignalR");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ReceiveNotification", function (message) {
        const notificationCountElem = $('#notificationCount');
        let currentCount = parseInt(notificationCountElem.text());
        notificationCountElem.text(currentCount + 1);

        const notificationList = $('#notificationList');
        notificationList.find('.dropdown-item').remove();
        notificationList.append('<li><a class="dropdown-item" href="#">' + message + '</a></li>');
    });
});