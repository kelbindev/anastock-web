function copyFromShipping() {
    $('#tbBillingContactPerson').val($('#tbShippingContactPerson').val());
    $('#tbBillingContactEmail').val($('#tbShippingContactEmail').val());
    $('#tbBillingContactFax').val($('#tbShippingContactFax').val());
    $('#tbBillingContactPhone').val($('#tbShippingContactPhone').val());
    $('#tbBillingContactCountry').val($('#tbShippingContactCountry').val());
    $('#tbBillingAddress').val($('#tbShippingAddress').val());
    $('#tbBillingTown').val($('#tbShippingTown').val());
    $('#tbBillingState').val($('#tbShippingState').val());
    $('#tbBillingPostalCode').val($('#tbShippingPostalCode').val());
};

function SubmitFormCreateEdit(form) {
    var isValid = true;

    var vendorName = $('#tbVendorName');
    var vendorEmail = $('#tbVendorEmail');

    var billingContactPerson = $('#tbBillingContactPerson');
    var billingContactEmail = $('#tbBillingContactEmail');
    var billingContactFax = $('#tbBillingContactFax');
    var billingContactPhone = $('#tbBillingContactPhone');
    var billingContactCountry = $('#tbBillingContactCountry');
    var billingContactAddr = $('#tbBillingAddress');
    var billingContactTown = $('#tbBillingTown');
    var billingContactState = $('#tbBillingState');
    var billingContactPostal = $('#tbBillingPostalCode');

    var ShippingContactPerson = $('#tbShippingContactPerson');
    var ShippingContactEmail = $('#tbShippingContactEmail');
    var ShippingContactFax = $('#tbShippingContactFax');
    var ShippingContactPhone = $('#tbShippingContactPhone');
    var ShippingContactCountry = $('#tbShippingContactCountry');
    var ShippingContactAddr = $('#tbShippingAddress');
    var ShippingContactTown = $('#tbShippingTown');
    var ShippingContactState = $('#tbShippingState');
    var ShippingContactPostal = $('#tbShippingPostalCode');

    if (vendorName.val().trim() == '') {
        vendorName.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        vendorName.siblings('span').css('display', 'none');
    }

    if (vendorEmail.val().trim() == '') {
        vendorEmail.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        vendorEmail.siblings('span').css('display', 'none');
    }

    //BILLING
    if (billingContactPerson.val().trim() == '') {
        billingContactPerson.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactPerson.siblings('span').css('display', 'none');
    }

    if (billingContactEmail.val().trim() == '') {
        billingContactEmail.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactEmail.siblings('span').css('display', 'none');
    }

    if (billingContactFax.val().trim() == '') {
        billingContactFax.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactFax.siblings('span').css('display', 'none');
    }

    if (billingContactPhone.val().trim() == '') {
        billingContactPhone.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactPhone.siblings('span').css('display', 'none');
    }

    if (billingContactCountry.val().trim() == '') {
        billingContactCountry.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactCountry.siblings('span').css('display', 'none');
    }

    if (billingContactAddr.val().trim() == '') {
        billingContactAddr.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactAddr.siblings('span').css('display', 'none');
    }

    if (billingContactTown.val().trim() == '') {
        billingContactTown.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactTown.siblings('span').css('display', 'none');
    }

    if (billingContactState.val().trim() == '') {
        billingContactState.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactState.siblings('span').css('display', 'none');
    }

    if (billingContactPostal.val().trim() == '') {
        billingContactPostal.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        billingContactPostal.siblings('span').css('display', 'none');
    }

    //SHIPPING
    if (ShippingContactPerson.val().trim() == '') {
        ShippingContactPerson.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactPerson.siblings('span').css('display', 'none');
    }

    if (ShippingContactEmail.val().trim() == '') {
        ShippingContactEmail.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactEmail.siblings('span').css('display', 'none');
    }

    if (ShippingContactFax.val().trim() == '') {
        ShippingContactFax.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactFax.siblings('span').css('display', 'none');
    }

    if (ShippingContactPhone.val().trim() == '') {
        ShippingContactPhone.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactPhone.siblings('span').css('display', 'none');
    }

    if (ShippingContactCountry.val().trim() == '') {
        ShippingContactCountry.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactCountry.siblings('span').css('display', 'none');
    }

    if (ShippingContactAddr.val().trim() == '') {
        ShippingContactAddr.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactAddr.siblings('span').css('display', 'none');
    }

    if (ShippingContactTown.val().trim() == '') {
        ShippingContactTown.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactTown.siblings('span').css('display', 'none');
    }

    if (ShippingContactState.val().trim() == '') {
        ShippingContactState.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactState.siblings('span').css('display', 'none');
    }

    if (ShippingContactPostal.val().trim() == '') {
        ShippingContactPostal.siblings('span').css('display', 'inline-block');
        isValid = false;
    }
    else {
        ShippingContactPostal.siblings('span').css('display', 'none');
    }


    if (isValid) {
        var url = form.action;
        var nvi;
        var nvn;

        $.ajax({
            url: url,
            type: 'post',
            data: $(form).serialize(),
            success: function (data) {
                $('#modalCreateEditVendor').modal("hide");
                console.log(data);
                nvi = data.newVendorId;
                nvn = data.newVendorName;
                bootbox.alert(data.message, function () {
                    //$("#cancel").click();

                    if (reloadVendor == "true") {
                        location.reload();
                    }
                    else {
                        var vendor = document.getElementById("Vendor");

                        vendor.options.add(new Option(nvn, nvi), vendor.options[vendor.options.length - 1]);
                        vendor.value = nvi;
                    }
                });
            }
        });
    }

    return false;
};

function SubmitFormDelete(url, vendorId) {
    $.ajax({
        url: url,
        type: 'post',
        data: { id: vendorId },
        success: function (data) {
            bootbox.alert(data.message, function () {
                
                if (reloadVendor == "true") {
                    location.reload();
                }
            });
        }
    });

    return false;
};