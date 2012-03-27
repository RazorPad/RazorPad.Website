﻿$(function () {

    RazorPad.razorEditor = null;

    function resizeRazorEditor() {
        $(RazorPad.razorEditor.getScrollerElement()).css({
            height: $("#razorPane").height() - $("#viewHeader").height(),
            width: $("#razorPane").width()
        });
        RazorPad.razorEditor.refresh();
        resizeModelProps();
    }

    function resizeModelProps() {
        var $modelPane = $('#modelPane');
        $modelPane.find('.modelProps')
                         .height(
                            $modelPane.height()
                            - $modelPane.find('.paneheader').outerHeight(true)
                            - $modelPane.find('.propinput').outerHeight(true)
                            - 10//modelProps top and bottom padding
                            );
    }

    var outerLayout = $('body').layout({
        enableCursorHotkey: false,
        north__paneSelector: "#toolbar",
        north__resizable: false,
        west__paneSelector: "#sidebar",
        west__size: Math.floor((screen.width / 1.67) - 10),
        west__resizable: false,
        west__closable: true,
        west__maxSize: 230,
        center__paneSelector: "#panes",
        resizeWhileDragging: true,
        closable: false
    });

    var innerLayout = $('#panes').layout({
        closable: false,
        center__paneSelector: "#razorPane",
        east__paneSelector: "#modelPane",
        east__maxSize: 600,
        east__minSize: 400,
        east__size: 400,
        south__paneSelector: "#resultsPane",
        south__maxSize: "70%",
        south__size: Math.floor((screen.height / 2) - 100),
        south__minSize: 200,
        resizeWhileDragging: true,
        onresize: resizeRazorEditor
    });


    RazorPad.razorEditor = CodeMirror.fromTextArea(document.getElementById("razorEditor"), {
        mode: "text/html",
        lineNumbers: true,
        tabMode: "indent",
        matchBrackets: true,
        tabIndex: -1,
        lineWrapping: true,
        extraKeys: {
            "Ctrl-S": function () { RazorPad.saveTemplate(); },
            "Ctrl-E": function () { RazorPad.executeTemplate(); },
            "Ctrl-C": function () { RazorPad.saveTemplate(true); }
        }
    });

    $("#mainContainer").css('visibility', 'visible');

    $(window).resize(resizeRazorEditor).resize();

    if ($("#snippetId").val()) {
        //Execute the template (adding timeout to load the layout before ajax calls)
        setTimeout(function () {
            RazorPad.executeTemplate();
        }, 200);
    }

    $('#savedSnippets').height($("#sidebar").height() / 2);

    $('#accordion').accordion({
        autoHeight: false,
        change: function () {
            $('#savedSnippets').height($("#sidebar").height() / 2);
        }
    });
});

RazorPad.saveTemplate = function (clone) {
    RazorPad.showLoading();
    var data = {
        Template: RazorPad.razorEditor.getValue(),
        Model: JSON.stringify(RazorPad.getModel()),
        SnippetId: (clone ? '' : ($('#snippetId').val() || '')),
        Title: $('#snippetTitle').val() || '',
        Notes: $('#snippetNotes').val() || ''
    };
    $.ajax({
        url: RazorPad.siteRoot + 'RazorPad/Save',
        cache: false,
        data: JSON.stringify(data),
        success: function (response) {
            if (!response.Messages) {
                if (clone || !$('#snippetId').val()) {
                    location.href = RazorPad.siteRoot + response;
                }
                else {
                    RazorPad.hideLoading();
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
        Template: RazorPad.razorEditor.getValue(),
        Model: JSON.stringify(RazorPad.getModel())
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
    alert(evt); return;
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
    $('#loading').fadeOut('slow');
};

RazorPad.getModel = function() {
    var model = { }, $this, name, val, type;
    $('#modelGrid > tbody > tr').each(function() {
        $this = $(this);
        name = $this.find('span.name').text();
        val = $this.find('span.value').text();
        type = $this.data('type') || "string"; //If type is not specified consider it as string
        if (name && val) {
            model[$.trim(name)] = RazorPad.parsePropValue($.trim(val), $.trim(type));
        }
    });
    return model;
};

RazorPad.parsePropValue = function(val, type) {
    if (type && val) {
        switch (type.toLowerCase()) {
        case "int":
            return parseInt(val);
        case "float":
        case "double":
            return parseFloat(val);
        case "string":
            return val;
        }
    }
    return val;
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
    .bind('keydown', 'ctrl+e', RazorPad.handleExecuteKey)
    .bind('keydown', 'ctrl+c', RazorPad.handleCloneKey);

    RazorPad.razorEditor.focus();

    if (RazorPad.isUserAuthenticated) {
        $('#savedSnippets').delegate('.snippet', 'click', function () {
            location.href = RazorPad.siteRoot + $(this).data('key');
            return false;
        });
    }
});