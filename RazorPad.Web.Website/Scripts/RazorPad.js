var RazorPad = typeof(RazorPad) == 'undefined' ? {} : RazorPad;

RazorPad.saveTemplate = function (clone) {
    RazorPad.showLoading();
    var data = {
        Template: RazorPad.razorEditor.getValue(),
        Model: JSON.stringify(RazorPad.getModel()),
        FiddleId: (clone ? '' : ($('#fiddleId').val() || '')),
        Title: $('#fiddleTitle').val() || '',
        Notes: $('#fiddleNotes').val() || ''
    };
    $.ajax({
        url: RazorPad.siteRoot + 'RazorPad/Save',
        cache: false,
        data: JSON.stringify(data),
        success: function (response) {
            if (!response.Messages) {
                if (clone || !$('#fiddleId').val()) {
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
        Model:  JSON.stringify(RazorPad.getModel())
    };

    $.ajax({
        url: RazorPad.siteRoot + 'RazorPad/Execute',
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
    var model = {}, $this, name, val, type;
    $('#modelGrid > tbody > tr').each(function () {
        $this = $(this);
        name = $this.find('span.name').text();
        val = $this.find('span.value').text();
        type = $this.data('type') || "string";//If type is not specified consider it as string
        if (name && val) {
            model[$.trim(name)] = RazorPad.parsePropValue($.trim(val), $.trim(type));
        }
    });
    return model;
}

RazorPad.parsePropValue = function (val, type) {
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

    if ($("#fiddleId").val()) {
        $('#clone')
        .click(function (e) {
            e.preventDefault();
            RazorPad.saveTemplate(true);
        }).parent().show();
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

    if (RazorPad.isUserAuthenticated) {
        $('#savedFiddles').delegate('.fiddle', 'click', function () {
            location.href = RazorPad.siteRoot + $(this).data('key');
            return false;
        });
    }
});