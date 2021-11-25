function loadBill(vendor) {
    let bill = document.getElementById('BillNo');
    bill.innerHTML = "";
    $.ajax({
        type: "GET",
        url: "/Bill/bindBill",
        data: { 'vendorId': $(vendor).val() },
        success: function (data) {
            for (let i = 0; i < data.length; i++) {
                option = document.createElement('option');
                option.text = data[i].billNo;
                option.value = data[i].id;
                bill.add(option);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function loadBalance(Bill) {
    $.ajax({
        type: "GET",
        url: "/Bill/bindBalance",
        data: { 'BillId': $(Bill).val() },
        success: function (data) {
            $("#BalanceDue").text("$" + data.toFixed(2));
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function clearBill() {
    $('#BillNo')
        .find('option')
        .remove()
        .end()
        .append('<option value="00000000-0000-0000-0000-000000000000">Select Bill</option>')
        .val('');
}

$('#submit').click(function () {
    var isValid = true;

    if ($('#Vendor').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('#Vendor').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#Vendor').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#PaymentMethod').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('#PaymentMethod').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#PaymentMethod').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#BillNo').val() == '00000000-0000-0000-0000-000000000000' || $('#BillNo').val() == '' || $('#BillNo').val() === null) {
        $('#BillNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#BillNo').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#PaymentDate').val().trim() == '') {
        $('#PaymentDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#PaymentDate').siblings('span.error').css('visibility', 'hidden');
    }

    //if ($('#ReferenceNo').val().trim() == '') {
    //    $('#ReferenceNo').siblings('span.error').css('visibility', 'visible');
    //    isValid = false;
    //}
    //else {
    //    $('#ReferenceNo').siblings('span.error').css('visibility', 'hidden');
    //}

    if ($('#AmountPaid').val().trim() == '' || $('#AmountPaid').val().trim() == '0.00' || $('#AmountPaid').val().trim() == NaN || $('#AmountPaid').val().trim() <= 0) {
        $('#AmountPaid').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#AmountPaid').siblings('span.error').css('visibility', 'hidden');
    }

    if (isValid) {
        var data = {
            VendorId: $('#Vendor').val().trim(),
            BillId: $('#BillNo').val(),
            PaymentMethodId: $('#PaymentMethod').val().trim(),
            PaymentDate: $('#PaymentDate').val().trim(),
            ReferenceNo: $('#ReferenceNo').val().trim(),
            AmountPaid: parseFloat($('#AmountPaid').val()),
            Description: $("#Description").val()
        }

        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/Bill/savePayment',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                $('#submit').val('Save');
                if (result.success == true) {
                    window.location = '/Bill/IndexPayment';
                }

            },
            error: function (error) {
                console.log(error);
                $('#submit').val('Save');
            }
        });
    }

});

function isNumber(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode != 46 || $(element).val().indexOf('.') != -1) && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

$(document).ready(function () {
    $('.amountpaid').keypress(function (event) {

        return isNumber(event, this)
    });
    document.getElementsByClassName('amountpaid')[0].oninput = function () {
        var max = parseFloat(this.max);

        if (parseFloat(this.value) > max) {
            this.value = max;
        }
    }
});