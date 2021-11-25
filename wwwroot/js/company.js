$('#attc').on('change', function (e) {
    var fileName = e.target.files[0].name;
    $(this).next('.custom-file-label').html(fileName);
})

$('#submit').click(function (e) {

    e.preventDefault();

    var isValid = true;

    var name = $('#name');
    var address = $("#address");
    var email = $('#email');
    var website = $('#website');
    var phone = $('#phone');
    var fax = $('#fax');

    if (name.val().trim() == '') {
        name.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        name.siblings('span').css('display', 'none');
    }

    if (address.val().trim() == '') {
        address.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        address.siblings('span').css('display', 'none');
    }

    //if (email.val().trim() == '') {
    //    email.siblings('span').css('display', 'none');
    //}
    //else {
    var emailExpr = /(?:((?:[\w-]+(?:\.[\w-]+)*)@(?:(?:[\w-]+\.)*\w[\w-]{0,66})\.(?:[a-z]{2,6}(?:\.[a-z]{2})?));*)/g;
    if (!emailExpr.test(email.val())) {
        email.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        email.siblings('span').css('display', 'none');
    }
    //}

    if (website.val().trim() == '') {
        website.siblings('span').css('display', 'none');
    }
    else {
        //var websiteExpr = /^(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])+(.[a-z])?/;
        var websiteExpr = /^[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])+(.[a-z])?/;
        if (!websiteExpr.test(website.val())) {
            website.siblings('span').css('display', 'inline-block');
            isValid = false;
        }
        else {
            website.siblings('span').css('display', 'none');
        }
    }

    //if (phone.val().trim() == '') {
    //    phone.siblings('span').css('display', 'none');
    //}
    //else {
    var phoneExpr = /^((\+[1-9]{1,4}[ \-]*)|(\([0-9]{2,3}\)[ \-]*)|([0-9]{2,4})[ \-]*)*?[0-9]{3,4}?[ \-]*[0-9]{3,4}?$/;
    if (!phoneExpr.test(phone.val())) {
        phone.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        phone.siblings('span').css('display', 'none');
    }
    //}

    if (fax.val().trim() == '') {
        fax.siblings('span').css('display', 'none');
    }
    else {
        var faxExpr = /^\+?[0-9]+$/;
        if (!faxExpr.test(fax.val())) {
            fax.siblings('span').css('display', 'inline-block');
            isValid = false;
        }
        else {
            fax.siblings('span').css('display', 'none');
        }
    }

    var fileupload = $("#attc")[0].files;
    var imageType = ["image/gif", "image/jpeg", "image/png"];
    if (fileupload.length > 0) {

        var fileType = fileupload[0].type;
        console.log(fileType);
        if ($.inArray(fileType, imageType) < 0) {
            $("#attc").siblings('span').css('display', 'inline-block');
            isValid = false;
        }
        else {
            $("#attc").siblings('span').css('display', 'none');
        }
    }
    else {
        $("#attc").siblings('span').css('display', 'none');
    }

    if (isValid) {
        var data = {
            Name: name.val(),
            Address: $('#address').val(),
            Email: email.val(),
            Website: website.val(),
            Phone: phone.val(),
            Fax: $('#fax').val(),
            IsGSTEnable: $("#isgstenable").is(":checked") ? true : false,
            GSTRegNo: $('#gstregno').val(),
            GST: $('#gst').val(),
        };

        var formData = new FormData();

        formData.append("data", JSON.stringify(data));


        if (fileupload.length > 0) {
            formData.append('file', fileupload[0]);
        }

        $.ajax({
            type: "POST",
            url: '/Company/Save',
            data: formData,
            dataType: 'json',
            processData: false,
            contentType: false,
            success: function (data) {
                bootbox.alert(data.message, function () {
                    location.reload();
                });
            },
            error: function (error) {
                console.log("error");
            }
        });
    }
    return false;
});