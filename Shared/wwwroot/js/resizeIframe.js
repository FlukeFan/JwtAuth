(function () {

    window.resizeIframe = function () {
        var inIframe = window.self !== window.top;

        if (inIframe) {
            window.top.$(frameElement).trigger('mfeIframeResize', { size: $(document).height() });
        }
    }

    $(document).on('mfeIframeResize', 'iframe', function (e, d) {
        $(e.target).height(d.size);
    });

}());
