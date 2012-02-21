$(function () {
    var validate = function () {
        retVal = false;
        $("#loginForm").find(".errormsg").addClass("hide");
        if (!$("#UserName").val()) {
            $("#UserName").focus().next(".errormsg").removeClass("hide");
        }
        else if (!$("#Password").val()) {
            $("#Password").focus().next(".errormsg").removeClass("hide");
        }
        else {
            $("#loginBtn").prop("disabled", true);
            retVal = true;
        }
        return retVal;
    };

    $("#loginForm").submit(function () {
        return validate();
    });

    $("#UserName").focus();
});