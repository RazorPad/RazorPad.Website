$(function () {
    var f, doc = document;
    if ((f = doc.loginForm) != null) {
        var validate = function () {
            var retVal = false;
            $(f).find(".errorMsg").addClass("hide");
            if (!$(f.UserName).val()) {
                $(f.UserName).focus().next(".errorMsg").removeClass("hide");
            }
            else if (!$(f.Password).val()) {
                $(f.Password).focus().next(".errorMsg").removeClass("hide");
            }
            else {
                $("#loginBtn").prop("disabled", true);
                retVal = true;
            }
            return retVal;
        };

        $(f).submit(function () {
            return validate();
        });

        $(f.UserName).focus();
    }
    else if ((f = doc.signUpForm) != null) {
        var validate = function () {
            var retVal = false,
            emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

            $(f).find(".errorMsg").addClass("hide");

            if (!$(f.Email).val()) {
                $(f.Email).focus().next(".errorMsg").text("Enter your email").removeClass("hide");
            }
            else if (!emailPattern.test($(f.Email).val())) {
                $(f.Email).focus().next(".errorMsg").text("Enter valid email").removeClass("hide");
            }
            else if (!$(f.UserName).val()) {
                $(f.UserName).focus().next(".errorMsg").removeClass("hide");
            }
            else if (!$(f.Password).val()) {
                $(f.Password).focus().next(".errorMsg").removeClass("hide");
            }
            else if (!$(f.ConfirmPassword).val()) {
                $(f.ConfirmPassword).focus().next(".errorMsg").text("Confirm password").removeClass("hide");
            }
            else if ($(f.ConfirmPassword).val() != $(f.Password).val()) {
                $(f.ConfirmPassword).focus().next(".errorMsg").text("Password do no match").removeClass("hide");
            }
            else {
                $("#signUpBtn").prop("disabled", true);
                retVal = true;
            }
            return retVal;
        };

        $(f).submit(function () {
            return validate();
        });

        $(f.Email).focus();
    }
    else if ((f = doc.forgotPasswordForm) != null) {
        var validate = function () {
            var retVal = false,
                emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

            $(f).find(".errorMsg").addClass("hide");

            if (!$(f.Email).val()) {
                $(f.Email).focus().next(".errorMsg").text("Enter your email").removeClass("hide");
            }
            else if (!emailPattern.test($(f.Email).val())) {
                $(f.Email).focus().next(".errorMsg").text("Enter valid email").removeClass("hide");
            }
            else {
                $("#sendBtn").prop("disabled", true);
                retVal = true;
            }
            return retVal;
        };

        $(f).submit(function () {
            return validate();
        });

        $(f.Email).focus();
    }
    else if ((f = doc.resetPasswordForm) != null) {
        var validate = function () {
            var retVal = false;

            if (!$(f.Password).val()) {
                $(f.Password).focus().next(".errorMsg").removeClass("hide");
            }
            else if (!$(f.ConfirmPassword).val()) {
                $(f.ConfirmPassword).focus().next(".errorMsg").text("Confirm password").removeClass("hide");
            }
            else if ($(f.ConfirmPassword).val() != $(f.Password).val()) {
                $(f.ConfirmPassword).focus().next(".errorMsg").text("Password do no match").removeClass("hide");
            }
            else {
                $("#resetBtn").prop("disabled", true);
                retVal = true;
            }
            return retVal;
        };

        $(f).submit(function () {
            return validate();
        });

        $(f.Password).focus();
    }
});