$(function () {

    RazorPad.razorEditor = null;

    function onLayoutResize() {
        $(RazorPad.razorEditor.getScrollerElement()).css({
            height: $("#razorPane").height() - $("#viewHeader").height(),
            width: $("#razorPane").width()
        });
        RazorPad.razorEditor.refresh();

        var $resultsPane = $('#resultsPane');
        var $tabContainer = $resultsPane.children('.tabContainer');
        $tabContainer.children('.tabPanels').height($resultsPane.outerHeight(true)
                                                    - $tabContainer.children('.tabBar').outerHeight(true));
    }

    $('body').layout({
        enableCursorHotkey: false,
        name: 'bodyLayout',
        north__paneSelector: "header",
        north__closable: false,
        east__paneSelector: "#sidebar",
        east__size: 260,
        east__closable: true,
        center__paneSelector: "#panes",
        center__closable: false,
        resizeWhileDragging: true,
        resizable: false,
        onresize: onLayoutResize,
        useStateCookie: true
    });

    $('#panes').layout({
        enableCursorHotkey: false,
        closable: false,
        name: 'panesLayout',
        center__paneSelector: "#razorPane",
        south__paneSelector: "#resultsPane",
        south__maxSize: "70%",
        south__size: Math.floor((screen.height / 2) - 100),
        south__minSize: 200,
        resizeWhileDragging: true,
        onresize: onLayoutResize,
        useStateCookie: true
    });

    var editorConfig = {
        mode: "text/html",
        lineNumbers: true,
        tabMode: "indent",
        matchBrackets: true,
        tabIndex: -1,
        lineWrapping: true,
        extraKeys: {
            "Ctrl-S": function () { RazorPad.saveTemplate(); },
            "Ctrl-E": function () { RazorPad.executeTemplate(); }
        }
    };
    if ($('#snippetId').val()) {
        editorConfig.extraKeys["Ctrl-L"] = function () { RazorPad.saveTemplate(true); };
    }

    RazorPad.razorEditor = CodeMirror.fromTextArea(document.getElementById("razorEditor"), editorConfig);

    $("#mainContainer").css('visibility', 'visible');

    $(window).resize(onLayoutResize).resize();

    if ($("#snippetId").val()) {
        //Execute the template (adding timeout to load the layout before ajax calls)
        setTimeout(function () {
            RazorPad.executeTemplate();
        }, 200);
    }

    $('#savedSnippets').height(($("#sidebar").height() / 2) - $('#exampleSnippets').height());

    $('#sideBarTabs').delegate('a', 'click', function (e) {
        e.preventDefault();
        var $this = $(this), $span = $this.prev();
        $this.parent().next().slideToggle('fast', function () {
            if ($(this).is(':visible')) {
                $span.addClass('expanded').removeClass('collapsed');
            }
            else {
                $span.addClass('collapsed').removeClass('expanded');
            }
        });
    });

    //Watermark text for Info
    $('#snippetTitle').waterMark({ waterMarkClass: 'watermark', waterMarkText: "Title" });
    $('#snippetNotes').waterMark({ waterMarkClass: 'watermark', waterMarkText: "Notes" });

});

RazorPad.saveTemplate = function (clone) {
    RazorPad.showLoading();

    var snippetId = $('#snippetId').val();

    var data = {
        Template: RazorPad.razorEditor.getValue(),
        SnippetId: snippetId,
        Title: $('#snippetTitle').val(),
        Notes: $('#snippetNotes').val()
    };

    $.ajax({
        url: RazorPad.siteRoot + 'Snippets/' + (clone ? 'Clone' : 'Save'),
        cache: false,
        data: JSON.stringify(data),
        success: function (response) {
            if (response && !response.Messages) {
                var newSnippetId = response.Key;
                if (snippetId === newSnippetId) {
                    RazorPad.hideLoading();
                }
                else {
                    location.href = RazorPad.siteRoot + newSnippetId;
                }
            }
            else {
                alert("Save failed, please try again later");
                RazorPad.hideLoading();
            }
        },
        error: function (error) {
            alert("Save failed, please try again later");
        }
    });
};

RazorPad.executeTemplate = function() {
    RazorPad.showLoading();
    var data = {
        Template: RazorPad.razorEditor.getValue()
    };

    $.ajax({
        url: RazorPad.siteRoot + 'RazorPad/Execute',
        data: JSON.stringify(data),
        success: function(resp) {
            RazorPad.onParseSuccess(resp);
            RazorPad.showRenderedTemplateOutput(resp.TemplateOutput);
            $('#template-output').text(resp.TemplateOutput);
            $('div.tabContainer').hpTabs("select", resp.Success ? 0 : 4); //Select Output tab
        },
        error: function(resp) {
            RazorPad.onParseError(resp);
            var message = ' [[**** EXECUTION ERROR ****]] \r\n' + JSON.stringify(resp);
            $('#rendered-output-container').empty().text(message);
            $('#template-output').text(message);
            $('div.tabContainer').hpTabs("select", 4); //Select Message tab
        },
        complete: function() {
            RazorPad.hideLoading();
        }
    });
};

RazorPad.handleSaveKey = function(evt) {
    RazorPad.saveTemplate();
    evt.stopPropagation();
    return false;
};

RazorPad.handleExecuteKey = function(evt) {
    RazorPad.executeTemplate();
    evt.stopPropagation();
    return false;
};

RazorPad.handleCloneKey = function (evt) {
    RazorPad.saveTemplate(true);
    evt.stopPropagation();
    return false;
};

RazorPad.showLoading = function() {
    var $loading = $('#loading');
    $loading.css({
        left: ($(document).width() - $loading.width()) / 2,
        top: ($(document).height() - $loading.height()) / 2
    }).fadeIn('fast');
};

RazorPad.hideLoading = function() {
    $('#loading').fadeOut('fast');
};

RazorPad.onParseError = function(err) {
    RazorPad.updateStatus('fail');
    RazorPad.showMessages([{ Kind: 'Error', Text: JSON.stringify(err) }]);
    $('#generated-code').html(' [[**** PARSE ERROR ****]] ');
};

RazorPad.onParseSuccess = function(resp) {
    if (resp.Success) {
        RazorPad.updateStatus('success');
    } else {
        RazorPad.updateStatus('fail');
    }

    RazorPad.showMessages(resp.Messages);
    $('#generated-code-container').empty().append($('<pre id="generated-code" class="brush: csharp"></pre>').text(resp.GeneratedCode));

    $('#parser-result-container').empty().append($('<pre id="parser-results" class="brush: html"></pre>').text(resp.ParsedDocument));
    SyntaxHighlighter.highlight({ toolbar: false });
};


RazorPad.showMessages = function(messages) {
    var messagesList = $('#messages');
    messagesList.html('');

    $.each(messages, function(idx, message) {
        $('<li/>')
            .addClass(message.Kind)
            .html($('<pre/>').html(message.Text))
            .appendTo(messagesList);
    });
};

RazorPad.showRenderedTemplateOutput = function(templateOutput) {
    var iframe = $('iframe', '#rendered-output-container');

    if (!iframe.get(0))
        iframe = $('<iframe>').appendTo('#rendered-output-container');

    iframe.contents().find('body').html(templateOutput);
};

RazorPad.updateStatus = function(status) {
    $('#template-container').attr('class', status);
};



$(function () {
    $('#execute')
	.click(function (e) {
	    e.preventDefault();
	    RazorPad.executeTemplate();
	})
	.ajaxStart(function () {
	    RazorPad.updateStatus('waiting');
	    $('#template-output').text('');
	    $('#generated-code').text('');
	});

    $('#save').click(function (e) {
        e.preventDefault();
        RazorPad.saveTemplate();
    });

    if ($("#snippetId").val()) {
        $('#clone').click(function (e) {
            e.preventDefault();
            RazorPad.saveTemplate(true);
        }).parent().show();
    }

    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        type: 'post'
    });

    //Bind keyboard shorts
    $(document)
    .bind('keydown', 'ctrl+s', RazorPad.handleSaveKey)
    .bind('keydown', 'ctrl+e', RazorPad.handleExecuteKey);
    if ($('#snippetId').val()) {
        $(document).bind('keydown', 'ctrl+l', RazorPad.handleCloneKey);
    }

    RazorPad.razorEditor.focus();

    if (RazorPad.isUserAuthenticated) {
        $('#savedSnippets').delegate('.snippet', 'click', function () {
            location.href = RazorPad.siteRoot + $(this).data('key');
            return false;
        });
    }
});


