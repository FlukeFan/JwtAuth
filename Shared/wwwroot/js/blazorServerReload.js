(function () {
    var reload = function () {
        $.ajax({
            url: document.location.href,
            error: function () { window.setTimeout(reload, 300); },
            success: function () { document.location.reload(); }
        });
    }

    Blazor.defaultReconnectionHandler._reconnectCallback = reload;
}());
