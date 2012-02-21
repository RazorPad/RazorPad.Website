var RazorPad = typeof(RazorPad) == 'undefined' ? {} : RazorPad;

RazorPad.saveTemplate = function (clone) {
    RazorPad.showLoading();
    var data = {
        Template: RazorPad.razorEditor.getValue(), //ToDo: Need to find a way to find the editor instance on the page
        Model: JSON.stringify(RazorPad.getModel()),
        FiddleId: (clone ? '' : ($('#fiddleId').val() || ''))
    };
    $.ajax({
        url: RazorPad.siteRoot + 'Save',
        cache: false,
        data: JSON.stringify(data),
        success: function (response) {
            if (!response.Messages) {
                if (clone || !$('#fiddleId').val()) {
                    location.href = RazorPad.siteRoot + "Index/" + response;
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

RazorPad.executeTemplate = function(clone) {
    RazorPad.showLoading();
    var data = {
        Template: RazorPad.razorEditor.getValue(),
        Model:  JSON.stringify(RazorPad.getModel())
    };

    $.ajax({
        url: RazorPad.siteRoot + 'Execute',
        data: JSON.stringify(data),
        success: function (resp) {
            RazorPad.onParseSuccess(resp);
            RazorPad.showRenderedTemplateOutput(resp.TemplateOutput);
            $('#template-output').text(resp.TemplateOutput);
            $('div.tabContainer').hpTabs("select", resp.Success ? 0 : 4); //Select Output tab
        },
        error: function (resp) {
            RazorPad.onParseError(resp);
            var message = ' [[**** EXECUTION ERROR ****]] \r\n' + JSON.stringify(resp);
            $('#rendered-output-container').empty().text(message);
            $('#template-output').text(message);
            $('div.tabContainer').hpTabs("select", 4); //Select Message tab
        },
        complete: function () {
            RazorPad.hideLoading();
        }
    });
}

RazorPad.handleSaveKey = function(evt) {
    RazorPad.saveTemplate();
    evt.stopPropagation();
    return false;
}

RazorPad.handleExecuteKey = function(evt) {
    RazorPad.executeTemplate();
    evt.stopPropagation();
    return false;
}

RazorPad.showLoading = function() {
    var $loading = $('#loading');
    $loading.css({
        left: ($(document).width() - $loading.width()) / 2,
        top: ($(document).height() - $loading.height()) / 2
    }).fadeIn('fast');
}

RazorPad.hideLoading = function() {
    $('#loading').fadeOut('slow');
}



RazorPad.getModel = function () {
    var model = {}, name, val, type;
    $('#modelGrid > tbody > tr').each(function () {
        name = $(this).find('span.name').text();
        val = $(this).find('span.value').text();
        type = $(this).data('type');
        if (name && val) {
            model[$.trim(name)] = $.trim(val);
        }
    });
    return model;
}

RazorPad.onParseError = function(err) {
    RazorPad.updateStatus('fail');
    RazorPad.showMessages([{ Kind: 'Error', Text: JSON.stringify(err)}]);
    $('#generated-code').html(' [[**** PARSE ERROR ****]] ');
} // END onParseError()

RazorPad.onParseSuccess = function(resp) {
    if (resp.Success) { RazorPad.updateStatus('success'); }
    else { RazorPad.updateStatus('fail'); }

    RazorPad.showMessages(resp.Messages);
    $('#generated-code-container').empty().append($('<pre id="generated-code" class="brush: csharp"></pre>').text(resp.GeneratedCode));

    $('#parser-result-container').empty().append($('<pre id="parser-results" class="brush: html"></pre>').text(resp.ParsedDocument));
    SyntaxHighlighter.highlight({ toolbar: false });
} // END onParseSuccess()


RazorPad.showMessages = function(messages) {
    var messagesList = $('#messages');
    messagesList.html('');

    $.each(messages, function (idx, message) {
        $('<li/>')
			.addClass(message.Kind)
			.html($('<pre/>').html(message.Text))
			.appendTo(messagesList);
    });
} // END showMessages()

RazorPad.showRenderedTemplateOutput = function(templateOutput) {
    var iframe = $('iframe', '#rendered-output-container');

    if (!iframe.get(0))
        iframe = $('<iframe>').appendTo('#rendered-output-container');

    iframe.contents().find('body').html(templateOutput);
}

RazorPad.updateStatus = function (status) {
    $('#template-container').attr('class', status);
}



$(function () {
    $('#execute')
	.click(RazorPad.executeTemplate)
	.ajaxStart(function () {
	    RazorPad.updateStatus('waiting');
	    $('#template-output').text('');
	    $('#generated-code').text('');
	});

    $('#save').click(function () { RazorPad.saveTemplate(); });
    if ($("#fiddleId").val()) {
        $('#clone').click(function () { RazorPad.saveTemplate(true); }).parent().show();
    }

    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        type: 'post'
    });

    $.fn.bindHotkeys = function () {
        this
        .bind('keydown', 'ctrl+s', RazorPad.handleSaveKey)
        .bind('keydown', 'ctrl+e', RazorPad.handleExecuteKey);
    };

    $(document).bindHotkeys();
    RazorPad.razorEditor.focus();
});