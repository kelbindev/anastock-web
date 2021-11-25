function loadInvoice(customer) {
    let inv = document.getElementById('InvoiceNo');
    $.ajax({
        type: "GET",
        url: "/InvoiceReceivable/bindInvoice",
        data: { 'customerId': $(customer).val() },
        success: function (data) {
            $("#InvoiceNo").empty();
            for (let i = 0; i < data.length; i++) {
                option = document.createElement('option');
                option.text = data[i].invoiceNo;
                option.value = data[i].invoiceId;
                inv.add(option);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function loadBalance(invoice) {
    $.ajax({
        type: "GET",
        url: "/InvoiceReceivable/bindBalance",
        data: { 'invoiceId': $(invoice).val() },
        success: function (data) {
            $("#BalanceDue").text("$" + data.toFixed(2));
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function clearInvoice() {
    $('#InvoiceNo')
        .find('option')
        .remove()
        .end()
        .append('<option value="00000000-0000-0000-0000-000000000000">Select Invoice</option>')
        .val('');
}

$('#submit').click(function () {
    var isValid = true;

    if ($('#Customer').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('#Customer').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#Customer').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#PaymentMethod').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('#PaymentMethod').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#PaymentMethod').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#InvoiceNo').val() == '00000000-0000-0000-0000-000000000000' || $('#InvoiceNo').val() == '' || $('#InvoiceNo').val() === null) {
        $('#InvoiceNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#InvoiceNo').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#PaymentDate').val().trim() == '') {
        $('#PaymentDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#PaymentDate').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#ReferenceNo').val().trim() == '') {
        $('#ReferenceNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#ReferenceNo').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#AmountPaid').val().trim() == '' || $('#AmountPaid').val().trim() == '0.00' || $('#AmountPaid').val().trim() == NaN || $('#AmountPaid').val().trim() <= 0) {
        $('#AmountPaid').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#AmountPaid').siblings('span.error').css('visibility', 'hidden');
    }

    if (isValid) {
        var data = {
            CustomerId: $('#Customer').val().trim(),
            InvoiceId: $('#InvoiceNo').val(),
            PaymentMethodId: $('#PaymentMethod').val().trim(),
            PaymentDate: $('#PaymentDate').val().trim(),
            ReferenceNo: $('#ReferenceNo').val().trim(),
            AmountReceived: parseFloat($('#AmountPaid').val()),
            Description: $("#Description").val()
        }

        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/InvoiceReceivable/save',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                $('#submit').val('Save');
                if (result.success == true) {
                    window.location = '/InvoiceReceivable/Index';
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