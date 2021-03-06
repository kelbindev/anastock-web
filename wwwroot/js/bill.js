var mydate = new Date();

$(document).ready(function () {
    $('#paymentTermValueDiv').hide();

    if ($('.productline').hasClass('productline')) {
        LoadProduct($('#productline'));
    }
    $('.quantityline,.unitpriceline,.discountline,.amountline').keypress(function (event) {
        return isNumber(event, this)
    });
    //gst = $("#gstval").val();
    if ($('.tbBill').hasClass('tbBill')) {
        let connection = new signalR.HubConnectionBuilder().withUrl("/signalServer").build();

        connection.start();

        connection.on("refreshBill", function () {
            loadData();
        });
        loadData();
    }

    if ($('#PaymentTerm').val() == "Days") {
        $('#paymentTermValueDiv').show();
        $('#DueDate').attr('disabled', true);
    }
    else {
        if ($('#PaymentTerm').val() != "0") {
            $('#DueDate').attr('disabled', true);
        }
    }

    if ($('#DiscountType').val() == "0") {
        $('#DiscountValue').attr('disabled', true);
    }
    else {
        $('#DiscountValue').attr('disabled', false);
        if ($('#DiscountType').val() == "Percent") {
            $('.totalDiscount').css('visibility', 'visible')
            $("#discountTotal").html(parseFloat($("#discountTotal").text()).toFixed(2))
        }
        else {
            $('.totalDiscount').css('visibility', 'hidden')
        }
    }

});
var Products = [];
//var gst;
function LoadProduct(element) {
    if (Products.length == 0) {
        $.ajax({
            type: "GET",
            url: '/Bill/GetProducts',
            success: function (data) {
                Products = data;
                renderProduct(element);
            }
        })
    }
    else {
        //render catagory to the element
        renderProduct(element);
    }
}

function renderProduct(element) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option />').val('0').text('Select').attr("data-suom", "").attr("data-puom", "").attr("data-pprice", 0).attr("data-sprice", 0));
    $.each(Products, function (i, val) {
        $ele.append($('<option/>').val(val.id).text(val.name).attr("data-suom", val.sellUOM).attr("data-puom", val.purchaseUOM).attr("data-pprice", val.purchasePrice).attr("data-sprice", val.sellPrice));
    })
}

function isNumber(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode != 46 || $(element).val().indexOf('.') != -1) && (charCode < 48 || charCode > 57))
        return false;
    return true;
}


function formatDate(date) {
    //MM-DD-YYYY FORMAT
    //var result = ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '/'
    //    + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()))
    //    + '/' + date.getFullYear();

    //YYYY-MM-DD FORMAT
    var result = date.getFullYear() + "-" +
        ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + "-" +
        ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));

    return result;
}

function paymenttermChanged(paymentTerm) {
    var date = new Date();
    var formattedDate = formatDate(date);

    $('#DueDate').val(formattedDate);

    if ($(paymentTerm).val() != "0") {
        $('#DueDate').attr('disabled', true);
    }
    else {
        $('#DueDate').attr('disabled', false);
    }

    if ($(paymentTerm).val() != "Days") {
        $('#paymentTermValueDiv').hide();
    }
    else {
        $('#paymentTermValueDiv').show();
        $('#paymentTermValue').val(0);
    }
}

function calculateDueDate(days) {
    var date = new Date();
    var formattedDate = formatDate(date);
    var day = $(days).val();

    $('#paymentTermValue').val(day);

    if (day.toString() == "") {
        $('#DueDate').val(formattedDate);
    }
    else {
        date.setDate(date.getDate() + parseInt(day));
        formattedDate = formatDate(date);
        $('#DueDate').val(formattedDate);
    }
}

function LoadProductData(product) {
    console.log($(product).val());
    console.log($(product).find(":selected").data('puom'));
    if ($(product).val() != "0") {
        $(product).closest('tr').find('.uomline').val($(product).find(":selected").data('puom'));
        $(product).closest('tr').find('.unitpriceline').val($(product).find(":selected").data('pprice'));
        updateTempAmounts();
        updateFinalAmounts();
    }
    else {
        $(product).closest('tr').find('.descriptionline').val("");
        $(product).closest('tr').find('.quantityline').val(0);
        $(product).closest('tr').find('.uomline').val("");
        $(product).closest('tr').find('.unitpriceline').val(0.00);
        $(product).closest('tr').find('.discountline').val(0);
        $(product).closest('tr').find('.amountline').val(0.00);
    }
}

$('#tablebody td .quantityline, .unitpriceline, .discountline').change(function () {
    updateTempAmounts();
});

$('#tablebody td .amountline').change(function () {
    updateTempDiscounts();
});


function updateTempDiscounts() {
    $('#tablebody').each(function () {
        var qty = $(this).find('.quantityline').val();
        var price = $(this).find('.unitpriceline').val();
        var amount = $(this).find('.amountline').val();
        var total = (qty * price);
        var discount = 100 - ((amount / total) * 100);
        $(this).find('.discountline').val(discount.toFixed(2));
    });
}

function updateFinalDiscounts() {
    var grandtotal = 0.0;
    $('#orderdetailsItems tr').each(function () {
        var qty = $(this).find('.quantityline').val();
        var price = $(this).find('.unitpriceline').val();
        var amount = $(this).find('.amountline').val();
        var total = (qty * price);
        var discount = 100 - ((amount / total) * 100);
        grandtotal += parseFloat(parseFloat(amount).toFixed(2));
        $(this).find('.discountline').val(discount.toFixed(2));
        $("#subtotal").html(grandtotal);
        $("#total").html(parseFloat(grandtotal).toFixed(2));
        $("#grandtotal").val(parseFloat(grandtotal).toFixed(2));
    });
}

function updateFinalAmounts() {
    var grandtotal = 0.0;
    $('#orderdetailsItems tr').each(function () {
        var qty = $(this).find('.quantityline').val();
        var price = $(this).find('.unitpriceline').val();
        var discount = $(this).find('.discountline').val();
        var total = (qty * price);
        var amount = (total - ((total * discount) / 100));
        //alert(amount);
        grandtotal += amount;
        $(this).find('.amountline').val(parseFloat(amount.toFixed(0)).toFixed(2));
        $("#subtotal").html(grandtotal);
        $("#total").html(parseFloat(grandtotal).toFixed(2));
        $("#grandtotal").val(parseFloat(grandtotal).toFixed(2));
    });

    //Calculate Discount
    if (grandtotal > 0.0) {
        var discType = $('#DiscountType').val();
        var discValue = parseFloat($('#DiscountValue').val());

        if (discType != "0") {
            if (discValue > 0.0) {
                if (discType == "Percent") {
                    discValue = (discValue / 100) * grandtotal;
                }
                $("#discountTotal").html(parseFloat(discValue).toFixed(2))
                grandtotal -= discValue;
                $("#total").html(parseFloat(grandtotal).toFixed(2));
                $("#grandtotal").val(parseFloat(grandtotal).toFixed(2));
            }
        }
    }

}

$('#orderdetailsItems').on('change', 'input', function () {
    var cls = $(this).attr('class');
    console.log(cls);
    if ($(this).hasClass('amountline')) {
        console.log('2');
        updateFinalDiscounts();
    }
    else {
        console.log('1');
        updateFinalAmounts();
    }
});

function updateTempAmounts() {
    $('#tablebody').each(function () {
        var qty = $(this).find('.quantityline').val();
        var price = $(this).find('.unitpriceline').val();
        var discount = $(this).find('.discountline').val();
        var total = (qty * price);
        var amount = total - ((total * discount) / 100);
        $(this).find('.amountline').val(amount);
    });
}

$('#addline').click(function () {
    var isValid = true;
    if ($('#productline').val() == "0") {
        isValid = false;
        $('#productline').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#productline').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#quantityline').val().trim() != '' && (parseInt($('#quantityline').val()) || 0))) {
        isValid = false;
        $('#quantityline').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#quantityline').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#uomline').val().trim() != '')) {
        isValid = false;
        $('#uomline').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#uomline').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#unitpriceline').val().trim() != '' && (parseInt($('#unitpriceline').val()) || 0) && !isNaN($('#unitpriceline').val().trim()))) {
        isValid = false;
        $('#unitpriceline').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#unitpriceline').siblings('span.error').css('visibility', 'hidden');
    }

    if (!($('#amountline').val().trim() != '' && (parseInt($('#amountline').val()) || 0) && !isNaN($('#amountline').val().trim()))) {
        isValid = false;
        $('#amountline').siblings('span.error').css('visibility', 'visible');
    }
    else {
        $('#amountline').siblings('span.error').css('visibility', 'hidden');
    }

    if (isValid) {
        var $newRow = $('#tablebody').clone().removeAttr('id');
        $('.productline', $newRow).val($('#productline').val());

        $('#addline', $newRow).addClass('remove').val('Remove').removeClass('btn-info').addClass('btn-danger');
        $('#falineIcon', $newRow).removeClass('fa-plus-circle').addClass('fa-times-circle').attr('data-icon', 'times-circle');

        $('#productline, #descriptionline,#quantityline, #uomline, #unitpriceline, #discountline, #amountline, #addline', $newRow).removeAttr('id');
        $('span.error', $newRow).remove();

        $('#orderdetailsItems').append($newRow);

        $('#productline').val('0');
        $('#quantityline, #discountline').val('0');
        $('#unitpriceline, #amountline').val('0.00');
        $('#orderItemError').empty();
        $('#uomline').val('');
        $('#descriptionline').val('');

        $('.quantityline,.unitpriceline,.discountline,.amountline').keypress(function (event) {
            return isNumber(event, this)
        });

        updateFinalAmounts();
    }
});

$('#orderdetailsItems').on('click', '.remove', function () {
    $(this).parents('tr').remove();
    updateFinalAmounts();
});

function LoadBillingAddress(vendor) {
    console.log($(vendor).val());
    if ($(vendor).val() != "00000000-0000-0000-0000-000000000000") {
        $.ajax({
            type: "GET",
            url: "/Bill/bindvendorAddressById",
            data: { 'vendorId': $(vendor).val() },
            success: function (data) {
                console.log(data);
                $("#BillingAddress").show();
                $("#billAddrId").val(data[0]['vendorAddressId']);
                $("#billAddrHdr").html("Billing Address");
                $("#billAddrVal").html(data[0]['billingAddress'] + "<br />" + data[0]['billingCountry']);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    else {
        $("#billAddrId").val('');
        $("#billAddrHdr").empty();
        $("#billAddrVal").empty();
        $("#BillingAddress").hide();

        var _text = $("#Vendor option:selected").text();

        if (_text == "Create New Vendor") {
            modalCreateEdit();
        }
    }
};

function modalCreateEdit() {
    $('#hdfVendorId').val('0');
    $('#modalCreateEditVendor').modal("show");
};


$('#cancel').click(function () {
    window.location = '/Bill/Index';
});

$('#submit').click(function () {
    var isValid = true;

    $('#orderItemError').text('');
    var list = [];
    var errorItemCount = 0;
    $('#orderdetailsItems tbody tr').each(function (index, ele) {
        if (
            $('select.productline', this).val() == "0" ||
            (parseInt($('.quantityline', this).val()) || 0) == 0 ||
            $('.unitpriceline', this).val() == "" ||
            isNaN($('.unitpriceline', this).val()) ||
            $('.amountline', this).val() == "" ||
            isNaN($('.amountline', this).val()) || $('.uomline', this).val() == ""
        ) {
            errorItemCount++;
            $(this).addClass('error');
        } else {
            var orderItem = {
                ProductAndServiceId: $('select.productline', this).val(),
                Qty: parseInt($('.quantityline', this).val()),
                UnitPrice: parseFloat($('.unitpriceline', this).val()),
                DiscountPercent: parseFloat($('.discountline', this).val()),
                Total: parseFloat($('.amountline', this).val()),
                UOM: $('.uomline', this).val(),
                Description: $('.descriptionline', this).val()
            }
            list.push(orderItem);
        }
    })

    if (errorItemCount > 0) {
        $('#orderItemError').text(errorItemCount + " invalid entry in order item list.");
        isValid = false;
    }

    if (list.length == 0) {
        $('#orderItemError').text('At least 1 order item required.');
        isValid = false;
    }

    if ($('#Vendor').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('#Vendor').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#Vendor').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#BillNo').val().trim() == '') {
        $('#BillNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#BillNo').siblings('span.error').css('visibility', 'hidden');
    }

    var issueDate = $('#IssueDate').val();
    var dueDate = $('#DueDate').val();
    var paymentTerm = $('#PaymentTerm').val();
    var paymentTermValue = $('#paymentTermValue').val();
    var discType = $('#DiscountType').val();
    var discValue = $('#DiscountValue').val();

    if (issueDate.trim() == '') {
        $('#IssueDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#IssueDate').siblings('span.error').css('visibility', 'hidden');
    }

    if (dueDate.trim() == '') {
        $('#DueDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#DueDate').siblings('span.error').css('visibility', 'hidden');
    }

    if (paymentTerm == "0") {
        $('#PaymentTerm').siblings('span.error').css('visibility', 'hidden');
    }
    else {
        if (!parseInt(paymentTermValue) > 0 && paymentTerm == "Days") {
            $('#paymentTermValue').siblings('span.error').css('visibility', 'visible');
            isValid = false;
        }
        else {
            $('#paymentTermValue').siblings('span.error').css('visibility', 'hidden');
        }
    }

    if (discType != "0") {
        if (!parseFloat(discValue) > 0.0) {
            $('#DiscountValue').siblings('span.error').html("Discount Value Required");
            $('#DiscountValue').siblings('span.error').css('visibility', 'visible');
            isValid = false;
        }
        else {
            var grandTotal = $("#grandtotal").val();

            if (discType == "Percent" && discValue > 100) {
                $('#DiscountValue').siblings('span.error').html("Percent can't be bigger than 100")
                $('#DiscountValue').siblings('span.error').css('visibility', 'visible');
                isValid = false;
            }
            else if (discType == "Value" && discValue > grandTotal) {
                $('#DiscountValue').siblings('span.error').html("Value can't be bigger than Total")
                $('#DiscountValue').siblings('span.error').css('visibility', 'visible');
                isValid = false;
            }
            else {
                $('#DiscountValue').siblings('span.error').css('visibility', 'hidden');
            }
        }
    }
    else {
        $('#DiscountValue').siblings('span.error').css('visibility', 'hidden');
    }

    if (isValid) {
        var data = {
            vendorId: $('#Vendor').val().trim(),
            BillNo: $('#BillNo').val().trim(),
            VendorInvoiceNo: $('#VendorInvoiceNo').val().trim(),
            IssueDate: $('#IssueDate').val().trim(),
            DueDate: $('#DueDate').val().trim(),
            Status: $('#Status').val(),
            VendorNotes: $('#VendorNotes').val(),
            SubTotal: parseFloat($('#subtotal').text()),
            Tax: 0.00,
            vendorAddressId: parseInt($('#billAddrId').val()),
            Total: parseFloat($('#grandtotal').val()),
            PaymentTerm: paymentTerm,
            PaymentTermValue: parseInt(paymentTermValue),
            LinkedPOId: $('#PurchaseOrderId').val(),
            BalanceDue: parseFloat($('#grandtotal').val()),
            DiscountType: discType,
            DiscountValue: parseFloat(discValue),
            BillDetails: list
        }

        console.log(JSON.stringify(data));
        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/Bill/save',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                console.log(result);
                $('#submit').val('Save');

                var billId = result.billId;

                bootbox.alert(result.message, function () {
                    if (result.success) {
                        window.location = '/Bill/Index';
                    }
                });

            },
            error: function (error) {
                console.log(error);
                $('#submit').val('Save');
            }
        });
    }

});

$('#update').click(function () {
    var isValid = true;

    $('#orderItemError').text('');
    var list = [];
    var errorItemCount = 0;
    $('#orderdetailsItems tbody tr').each(function (index, ele) {
        if (
            $('select.productline', this).val() == "0" ||
            (parseInt($('.quantityline', this).val()) || 0) == 0 ||
            $('.unitpriceline', this).val() == "" ||
            isNaN($('.unitpriceline', this).val()) ||
            $('.amountline', this).val() == "" ||
            isNaN($('.amountline', this).val()) ||
            $('.uomline', this).val() == ""
        ) {
            errorItemCount++;
            $(this).addClass('error');
        } else {
            var orderItem = {
                BillId: $("#id").val(),
                BillDetailsId: parseInt($(this).find("input[type='hidden']").val() == null ? 0 : $(this).find("input[type='hidden']").val()),
                ProductAndServiceId: $('select.productline', this).val(),
                Qty: parseInt($('.quantityline', this).val()),
                UnitPrice: parseFloat($('.unitpriceline', this).val()),
                DiscountPercent: parseFloat($('.discountline', this).val()),
                Total: parseFloat($('.amountline', this).val()),
                UOM: $('.uomline', this).val(),
                Description: $('.descriptionline', this).val()
            }
            list.push(orderItem);
        }
    })

    if (errorItemCount > 0) {
        $('#orderItemError').text(errorItemCount + " invalid entry in order item list.");
        isValid = false;
    }

    if (list.length == 0) {
        $('#orderItemError').text('At least 1 order item required.');
        isValid = false;
    }

    if ($('#Vendor').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('#Vendor').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#Vendor').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#BillNo').val().trim() == '') {
        $('#BillNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#BillNo').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#IssueDate').val().trim() == '') {
        $('#IssueDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#IssueDate').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#DueDate').val().trim() == '') {
        $('#DueDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#DueDate').siblings('span.error').css('visibility', 'hidden');
    }

    var paymentTerm = $('#PaymentTerm').val();
    var paymentTermValue = $('#paymentTermValue').val();
    var discType = $('#DiscountType').val();
    var discValue = $('#DiscountValue').val();

    if (paymentTerm == "0") {
        $('#PaymentTerm').siblings('span.error').css('visibility', 'hidden');
    }
    else {
        if (!paymentTermValue > 0 && paymentTerm == "Days") {
            $('#paymentTermValue').siblings('span.error').css('visibility', 'visible');
            isValid = false;
        }
        else {
            $('#paymentTermValue').siblings('span.error').css('visibility', 'hidden');
        }
    }

    if (isValid) {
        console.log($('#grandtotal').val());
        var data = {
            id: $("#id").val().trim(),
            vendorId: $('#Vendor').val().trim(),
            BillNo: $('#BillNo').val().trim(),
            VendorInvoiceNo: $('#VendorInvoiceNo').val().trim(),
            IssueDate: $('#IssueDate').val().trim(),
            DueDate: $('#DueDate').val().trim(),
            Status: $('#Status').val(),
            vendorNotes: $('#VendorNotes').val(),
            SubTotal: parseFloat($('#subtotal').text()),
            Tax: 0.00,
            vendorAddressId: parseInt($('#billAddrId').val()),
            Total: parseFloat($('#grandtotal').val()),
            PaymentTerm: paymentTerm,
            PaymentTermValue: parseInt(paymentTermValue),
            LinkedPOId: $('#PurchaseOrderId').val(),
            BalanceDue: parseFloat($('#grandtotal').val()),
            DiscountType: discType,
            DiscountValue: parseFloat(discValue),
            BillDetails: list
        }

        console.log(JSON.stringify(data));
        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/Bill/update',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                console.log(result);
                $('#update').val('Save');
                bootbox.alert(result.message);
            },
            error: function (error) {
                console.log(error);
                $('#update').val('Save');
            }
        });
    }

});


function loadData() {
    var tr = '';

    $.ajax({
        url: '/Bill/Index',
        method: 'GET',
        success: (result) => {
            $.each(result, (k, v) => {
                tr = tr + `<tr>
                        <td>
                            <a href="/Account/Edit/${v.BillId}">Edit</a>
                            <a onclick="return confirm ('Are you sure want to delete this Bill?');" href="/Account/Delete?BillId=${v.BillId}">Delete</a>
                        </td>
                        <td>${v.BillNo}</td>
                        <td>${v.VendorInvoiceNo}</td>
                        <td>${v.IssueDate}</td>
                        <td>${v.DueDate}</td>
                        <td>${v.Total}</td>
                        <td>${v.Status}</td>
                    </tr>`;
            });
            $("#tableBody").html(tr);
        },
        error: (error) => {
            console.log("SignalR : " + error);
        }
    });
}

function discountChange(discount) {
    console.log($(discount).val());
    $('#DiscountValue').val("");

    if ($(discount).val() == "0") {
        $('#DiscountValue').attr('disabled', true);
        $('.totalDiscount').css('visibility', 'hidden')
    }
    else {
        $('#DiscountValue').attr('disabled', false);
        if ($(discount).val() == "Percent") {
            $('.totalDiscount').css('visibility', 'visible')
        }
        else {
            $('.totalDiscount').css('visibility', 'hidden')
        }
    }

}
