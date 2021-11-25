
$(document).ready(function () {
    //empty customer modal on project
    $('.projectModalCreateEditCustomer').html("");

    if ($('.productline').hasClass('productline')) {
        LoadProduct($('#productline'));
    }
    $('.quantityline,.unitpriceline,.discountline,.amountline').keypress(function (event) {
        return isNumber(event, this)
    });
    gst = $("#gstval").val();
    if ($('.tbQuote').hasClass('tbQuote')) {
        let connection = new signalR.HubConnectionBuilder().withUrl("/signalServer").build();

        connection.start();

        connection.on("refreshQuote", function () {
            loadData();
        });
        loadData();
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
var gst;
function LoadProduct(element) {
    if (Products.length == 0) {
        $.ajax({
            type: "GET",
            url: '/Quote/GetProducts',
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

function LoadProject(customer) {
    if ($(customer).val() != "00000000-0000-0000-0000-000000000000") {
        $.ajax({
            type: "GET",
            url: "/Quote/bindProjectsByCustomer",
            data: { 'customerId': $(customer).val() },
            success: function (data) {
                console.log(data);
                var $ele = $("#project");
                $ele.empty();
                $.each(data, function (i, val) {
                    $ele.append($('<option/>').val(val.projectId).text(val.title));
                })
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
    }
}

function LoadProductData(product) {
    console.log($(product).val());
    console.log($(product).find(":selected").data('puom'));
    if ($(product).val() != "0") {
        $(product).closest('tr').find('.uomline').val($(product).find(":selected").data('suom'));
        $(product).closest('tr').find('.unitpriceline').val($(product).find(":selected").data('sprice'));
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

function isNumber(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode != 46 || $(element).val().indexOf('.') != -1) && (charCode < 48 || charCode > 57))
        return false;
    return true;
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
        $("#total").html((parseFloat(grandtotal) + ((parseFloat(grandtotal) * gst) / 100)).toFixed(2));
        $("#grandtotal").val((parseFloat(grandtotal) + ((parseFloat(grandtotal) * gst) / 100)).toFixed(2));
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
        $("#total").html((parseFloat(grandtotal) + ((parseFloat(grandtotal) * gst) / 100)).toFixed(2));
        $("#grandtotal").val((parseFloat(grandtotal) + ((parseFloat(grandtotal) * gst) / 100)).toFixed(2));
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

                console.log(grandtotal);
                $("#total").html(parseFloat(parseFloat(grandtotal) + ((parseFloat(grandtotal) * gst) / 100)).toFixed(2));
                $("#grandtotal").val(parseFloat(parseFloat(grandtotal) + ((parseFloat(grandtotal) * gst) / 100)).toFixed(2));
            }
        }
    }
}

$('#orderdetailsItems').on('change', 'input', function () {
    var cls = $(this).attr('class');
    if ($(this).hasClass('amountline')) {
        updateFinalDiscounts();
    }
    else {
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

        $('#productline, #descriptionline, #quantityline, #uomline, #unitpriceline, #discountline, #amountline, #addline', $newRow).removeAttr('id');
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
})

$('#orderdetailsItems').on('click', '.remove', function () {
    $(this).parents('tr').remove();
    updateFinalAmounts();
});

function LoadBillingAddress(customer) {
    if ($(customer).val() != "00000000-0000-0000-0000-000000000000") {
        $.ajax({
            type: "GET",
            url: "/Quote/bindCustomerAddressById",
            data: { 'customerId': $(customer).val() },
            success: function (data) {
                $("#BillingAddress").show();
                $("#billAddrId").val(data[0]['customerAddressId']);
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

        var _text = $("#Customer option:selected").text();

        if (_text == "Create New Customer") {
            modalCreateEdit();
        }
    }
}

function onProjectChanged() {
    var _text = $("#project option:selected").text();

    if (_text == "Create New Project") {
        modalCreateEditProject();
    }
}

function modalCreateEdit() {
    $('#hdfCustomerId').val('0');
    $('#modalCreateEditCustomer').modal("show");
};

function modalCreateEditProject() {
    var selCust = $('.quoteCustomer').val();

    if (selCust != "00000000-0000-0000-0000-000000000000") {
        $('#modalCreateEditProject').modal("show");
        var projectCustomer = $('.projectCustomer');
        projectCustomer.val(selCust);
        $('.projectCustomer').attr('disabled', true);

        projectAutoNumber();
        //$('#projectno').val(projectNo.projectno);
    }
    else {
        bootbox.alert("Please Select Customer!");
        $("#project").prop('selectedIndex', 0);
    }

}

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
        $('#orderItemError').text(errorItemCount + " invalid entry in Quote item list.");
        isValid = false;
    }

    if (list.length == 0) {
        $('#orderItemError').text('At least 1 item is required.');
        isValid = false;
    }

    if ($('.quoteCustomer').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('.quoteCustomer').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('.quoteCustomer').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#QuoteNo').val().trim() == '') {
        $('#QuoteNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#QuoteNo').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#IssueDate').val().trim() == '') {
        $('#IssueDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#IssueDate').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#ExpiryDate').val().trim() == '') {
        $('#ExpiryDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#ExpiryDate').siblings('span.error').css('visibility', 'hidden');
    }

    var discType = $('#DiscountType').val();
    var discValue = parseFloat($('#DiscountValue').val());

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
            CustomerId: $('.quoteCustomer').val().trim(),
            QuoteNo: $('#QuoteNo').val().trim(),
            CustomerPONo: $('#CustomerPONo').val().trim(),
            IssueDate: $('#IssueDate').val().trim(),
            ExpiryDate: $('#ExpiryDate').val().trim(),
            Status: $('#Status').val(),
            CustomerNotes: $('#CustomerNotes').val(),
            SubTotal: parseFloat($('#subtotal').text()),
            Tax: parseFloat(gst),
            CustomerAddressId: parseInt($('#billAddrId').val()),
            Total: parseFloat($('#grandtotal').val()),
            LinkedProjectId: $('#project').val() == "00000000-0000-0000-0000-000000000000" ? null : $('#project').val(),
            DiscountType: discType,
            DiscountValue: parseFloat(discValue),
            QuoteDetails: list
        }
        console.log(JSON.stringify(data));
        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/Quote/save',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                $('#submit').val('Save');
                if (result.success == true) {
                    window.location = '/Quote/Index';
                }

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
            isNaN($('.amountline', this).val()) || $('.uomline', this).val() == ""
        ) {
            errorItemCount++;
            $(this).addClass('error');
        } else {
            var orderItem = {
                QuoteId: $("#QuoteId").val(),
                QuoteDetailsId: parseInt($(this).find("input[type='hidden']").val() == null ? 0 : $(this).find("input[type='hidden']").val()),
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
        $('#orderItemError').text(errorItemCount + " invalid entry in Quote item list.");
        isValid = false;
    }

    if (list.length == 0) {
        $('#orderItemError').text('At least 1 item is required.');
        isValid = false;
    }

    if ($('.quoteCustomer').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('.quoteCustomer').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('.quoteCustomer').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#QuoteNo').val().trim() == '') {
        $('#QuoteNo').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#QuoteNo').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#IssueDate').val().trim() == '') {
        $('#IssueDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#IssueDate').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#ExpiryDate').val().trim() == '') {
        $('#ExpiryDate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#ExpiryDate').siblings('span.error').css('visibility', 'hidden');
    }

    var discType = $('#DiscountType').val();
    var discValue = parseFloat($('#DiscountValue').val());

    if (discType != "0") {
        if (!parseFloat(discValue) > 0) {
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
            QuoteId: $("#QuoteId").val().trim(),
            CustomerId: $('.quoteCustomer').val().trim(),
            QuoteNo: $('#QuoteNo').val().trim(),
            CustomerPONo: $('#CustomerPONo').val().trim(),
            IssueDate: $('#IssueDate').val().trim(),
            ExpiryDate: $('#ExpiryDate').val().trim(),
            Status: $('#Status').val(),
            CustomerNotes: $('#CustomerNotes').val(),
            SubTotal: parseFloat($('#subtotal').text()),
            Tax: parseFloat(gst),
            CustomerAddressId: parseInt($('#billAddrId').val()),
            Total: parseFloat($('#grandtotal').val()),
            LinkedProjectId: $('#project').val() == "00000000-0000-0000-0000-000000000000" ? null : $('#project').val(),
            DiscountType: discType,
            DiscountValue: parseFloat(discValue),
            QuoteDetails: list
        }

        console.log(JSON.stringify(data));
        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/Quote/update',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                console.log(result);
                $('#update').val('Save');
                if (result.success == true) {
                    bootbox.alert(result.message, function () {
                        window.location.reload();
                    })
                }
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
        url: '/Quote/Index',
        method: 'GET',
        success: (result) => {
            $.each(result, (k, v) => {
                tr = tr + `<tr>
                        <td>
                            <a href="/Account/Edit/${v.QuoteId}">Edit</a>
                            <a onclick="return confirm ('Are you sure want to delete this quote?');" href="/Account/Delete?quoteId=${v.QuoteId}">Delete</a>
                        </td>
                        <td>${v.QuoteNo}</td>
                        <td>${v.CustomerPONo}</td>
                        <td>${v.IssueDate}</td>
                        <td>${v.ExpiryDate}</td>
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
        $('.totalDiscount').css('visibility', 'hidden');
        updateFinalAmounts();
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

async function projectAutoNumber() {
    var res = await getProjectAutoNumber();
    $('#projectno').val(res.projectno);
}

function getProjectAutoNumber() {
    return $.ajax({
        url: '/Quote/GetNewProjectNumber',
        method: 'GET',
        dataType: 'json',
        //success: function (result) {
        //    console.log(result);
        //    if (result.success == true) {
        //        return result.projectno;
        //    }
        //    else {
        //        console.log(result.message);
        //    }
        //}
    });
}