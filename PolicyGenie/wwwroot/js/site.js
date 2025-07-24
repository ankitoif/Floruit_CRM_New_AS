function AlertMessage(msg, timeout, eventType) {
    var arr = msg.split('//');
    var message = arr.length > 1 ? arr[1] : msg;
    var alertType = 'success', title = 'Success!';

    if (eventType === 'fail' || arr[0] === 'R') {
        alertType = 'danger';
        title = 'Error!';
    } else if (eventType === 'warn' || arr[0] === 'W') {
        alertType = 'warning';
        title = 'Warning!';
    }

    $('#divTransactionResult').html(`
        <div class="alert alert-${alertType}" style="z-index:3000;position:fixed;width:100%;bottom:0%; right:-70px;">
            <a class="close" data-dismiss="alert"></a>
            <strong>${title}</strong>
            <p style="font-weight:bold">${message}</p>
        </div>
    `);

    alertTimeout(timeout);
}

function alertTimeout(wait) {
    setTimeout(function () {
        $('#divTransactionResult').children('.alert:first-child').remove();
    }, wait);
}
