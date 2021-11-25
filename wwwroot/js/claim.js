$('#attc').on('change', function (e) {
    var fileName = e.target.files[0].name;
    $(this).next('.custom-file-label').html(fileName);
})
$('#submit').click(function () {
    var isValid = true;
    var fileupload = $("#attc")[0].files;
    var formData = new FormData();
    if (fileupload.length > 0) {
        formData.append('file', fileupload[0]);
    }
    if (isValid) {
        var data = {
            ExpenseDateFrom: $('#expenseDateFrom').val().trim(),
            ExpenseDateTo: $('#expenseDateTo').val().trim(),
            ExpenseType: $('#expenseType').val().trim(),
            SubTotal: parseFloat($('#subTotal').text()),
            Tax: parseFloat($('#tax').text()),
            Total: parseFloat($('#total').text()),
            Remarks: $('#remarks').val()
        }

        console.log(JSON.stringify(data));
        //for (var pair of formData.entries()) {
        //    console.log(pair[0] + ', ' + pair[1].name);
        //}
        $(this).val('Please wait...');
        //$.ajax({
        //    type: 'POST',
        //    url: '/PurchaseOrder/save',
        //    data: JSON.stringify(data),
        //    contentType: 'application/json',
        //    success: function (result) {
        //        console.log(result);
        //        $('#submit').val('Save');

        //        var poId = result.poId;

        //        bootbox.alert(result.message, function () {
        //            if (result.success) {
        //                window.location = '/PurchaseOrder/Edit/' + poId;
        //            }
        //        });

        //    },
        //    error: function (error) {
        //        console.log(error);
        //        $('#submit').val('Save');
        //    }
        //});
    }

});