$(function () {
    var validate = function () {
        var retVal = false,
            emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

        $("#signUpForm").find(".errormsg").addClass("hide");

        if (!$("#Email").val()) {
            $("#Email").focus().next(".errormsg").text("Enter your email").removeClass("hide");
        }
        else if (!emailPattern.test($("#Email").val())) {
            $("#Email").focus().next(".errormsg").text("Enter valid email").removeClass("hide");
        }
        else if (!$("#UserName").val()) {
            $("#UserName").focus().next(".errormsg").removeClass("hide");
        }
        else if (!$("#Password").val()) {
            $("#Password").focus().next(".errormsg").removeClass("hide");
        }
        else if (!$("#ConfirmPassword").val()) {
            $("#ConfirmPassword").focus().next(".errormsg").text("Confirm password").removeClass("hide");
        }
        else if ($("#ConfirmPassword").val() != $("#Password").val()) {
            $("#ConfirmPassword").focus().next(".errormsg").text("Password do no match").removeClass("hide");
        }
        else {
            $("#signUpBtn").prop("disabled", true);
            retVal = true;
        }
        return retVal;
    };

    $("#signUpForm").submit(function () {
        return validate();
    });

    $("#Email").focus();
});