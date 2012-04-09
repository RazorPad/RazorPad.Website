(function ($) {
    $.extend($.fn, {
        waterMark: function (options) {
            var $this = null;
            return this.each(function () {
                $this = $(this);
                if ($this.val() === options.waterMarkText || $this.val() === "") {
                    $this.addClass(options.waterMarkClass).val(options.waterMarkText);
                }

                $this.focus(function () {
                    $(this).filter(function () {
                        return $(this).val() === "" || $(this).val() === options.waterMarkText;
                    }).val("").removeClass(options.waterMarkClass);

                })
                .blur(function () {
                    $(this).filter(function () {
                        return $(this).val() === "";
                    })
                    .addClass(options.waterMarkClass)
                    .val(options.waterMarkText);
                });

            });
        }
    });

})(jQuery);