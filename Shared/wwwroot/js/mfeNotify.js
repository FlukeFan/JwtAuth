(function () {

    window.mfeNotify = function (message) {
        var inIframe = window.self !== window.top;

        if (inIframe) {
            window.top.$(window.top).trigger('notify', { message: message });
        } else {
            internalNotify(message);
        }
    }

    $(window).on('notify', function (e, d) {
        internalNotify(d.message);
    });

    function internalNotify(message) {
        if (!$.notify) {
            console.log('notify: ' + message);
            return;
        }

        $.notify(message, {
            delay: 3500,
            timer: 100,
            type: 'danger',
            allow_dismiss: true,
            mouse_over: 'pause',
            placement: {
                from: "bottom",
                align: "right"
            }
        });
    }

}());
