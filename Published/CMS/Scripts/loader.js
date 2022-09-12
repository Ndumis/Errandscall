class LoaderOptions {
    constructor() {
        this.type = LoaderType.container
    }
}


const LoaderType = {
    right: 'right',
    full: 'full',
    container: 'container',
}



/**
* Shows a loader for specific elements.
* @constructor
* @param {Options} options - options for the loader.
*/
$.fn.loader = function (options = new LoaderOptions()) {

    this.X = 0;
    this.Y = 0;
    this.Current = this;
    Current = this;
    this.Options = options;
    this.each(function () {
        var $element = null;
        var $this = null;
        if (options.type === LoaderType.right) {


            $element = $(this).find('.Loadingcontainer')
            $this = $(this);
            if (!$this.next().hasClass("input-group-btn")) {
                $(`<div class="input-group">
                         <div id="replacement"></div>
                        <span class="input-group-btn">
                            <img id="" class="" src="https://cdn.mscdirect.com/global/media/images/icons/error.png" title="" data-original-title="Author is required">
                        </span>
                </div>`).insertAfter($this)

                $('#replacement').replaceWith($this)
                //$(error).attr('src', config.errorSymbolLocation)
                //$('#replacementError').replaceWith(error)
            }
        }
        else {

            $(this).append($('.LoadingTemplate').html())
            $element = $(this).find('.Loadingcontainer')
            $this = $(this);
            $element.outerHeight($this.outerHeight(), true)
            //$element.css('height', $this.outerHeight() + 'px');
            $element.outerWidth($this.outerWidth(), true)
            $element.css('position', 'fixed')
            $element.css('top', $this.position().top)
            $element.css('left', $this.position().left)
            Current.X = $this.position().left;
            Current.Y = $this.position().top;
        }
    });

    return this;

};

$.fn.loaderUnload = function () {

    this.each(function () {
        var $this = $(this);
        setTimeout(function () {
            var $element = $this.find('.Loadingcontainer')
            if ($element) {
                $element.remove();
            }
        }, 10000)
    });

    return this;

};
