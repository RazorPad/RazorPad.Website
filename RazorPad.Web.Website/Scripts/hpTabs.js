(function ($) {
    $.fn.extend({
        hpTabs: function (options) {
            
            var ShowTab = function (lnk) {
                var $lnk = $(lnk),
    	            $panel = $($lnk.attr("href"));
                $lnk.addClass("selected").siblings("a").removeClass("selected");
                $panel.show().siblings("div").hide();
            };

            if (arguments.length == 2) {
                if (arguments[0] == "select" && !isNaN(arguments[1])) {
                    ShowTab(this.find(".tabBar > a").get(arguments[1]));
                }
                return;
            }
            
            var defaults = {},
                options = $.extend(defaults, options);

            return this.each(function () {
                var o = options;
                var $this = $(this);
                $this.find(".tabBar > a").click(function () {
                    ShowTab(this);
                    return false;
                });
                ShowTab($this.find(".tabBar > a")[0]);
            });
        }
    });
} (jQuery))