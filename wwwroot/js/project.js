$(document).ready(function () {
    //if ($('.handoverdate').hasClass('handoverdate')) {
    //    $('#Handoverdate').datetimepicker({
    //        format: 'dd/mm/yyyy'
    //    });
    //}
    //if ($('.dismantledate').hasClass('dismantledate')) {
    //    $('#Dismantledate').datetimepicker({
    //        format: 'dd/mm/yyyy'
    //    });
    //}
    //if ($('.installationdate').hasClass('installationdate')) {
    //    $('#Installationdate').datetimepicker({
    //        format: 'dd/mm/yyyy'
    //    });
    //}
    $('.projectbudget,.targetsales').keypress(function (event) {
        return isNumber(event, this)
    });
});

function isNumber(evt, element) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if ((charCode != 46 || $(element).val().indexOf('.') != -1) && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function CustomerOnChange() {
    var _text = $("#Customer option:selected").text();

    if (_text == "Create New Customer") {
        modalCreateEdit();
    }
}

function modalCreateEdit() {
    $('#hdfCustomerId').val('0');
    $('#modalCreateEditCustomer').modal("show");
};

function hideModalProject() {
    $('#modalCreateEditProject').modal("hide");
}

function submitProject() {
    var isValid = true;

    if ($('#title').val().trim() == '') {
        $('#title').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#title').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#projectno').val().trim() == '') {
        $('#projectno').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#projectno').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('.projectCustomer').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('.projectCustomer').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('.projectCustomer').siblings('span.error').css('visibility', 'hidden');
    }

    //if ($('#installationtime').val().trim() == '') {
    //    $('#installationtime').siblings('span.error').css('visibility', 'visible');
    //    isValid = false;
    //}
    //else {
    //    $('#installationtime').siblings('span.error').css('visibility', 'hidden');
    //}

    if ($('#installationdate').val().trim() == '') {
        $('#installationdate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#installationdate').siblings('span.error').css('visibility', 'hidden');
    }

    //if ($('#targetsales').val().trim() == '') {
    //    $('#targetsales').siblings('span.error').css('visibility', 'visible');
    //    isValid = false;
    //}
    //else {
    //    $('#targetsales').siblings('span.error').css('visibility', 'hidden');
    //}

    if ($('#projectbudget').val().trim() == '') {
        $('#projectbudget').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#projectbudget').siblings('span.error').css('visibility', 'hidden');
    }

    if (isValid) {
        var data = {
            Title: $('#title').val().trim(),
            CustomerId: $('.projectCustomer').val(),
            ProjectNo: $('#projectno').val().trim(),
            InstallationDate: $('#installationdate').val(),
            InstallationTime: '-',//$('#installationtime').val(),
            Status: 'New', //$('#status').val(),
            TargetSales: parseFloat(0),//parseFloat($('#targetsales').val()),
            ProjectBudget: parseFloat($('#projectbudget').val()),
            Remarks: $('#remarks').val(),
            HandoverDate: $('#handoverdate').val() == "" ? null : $('#handoverdate').val(),
            DismantleDate: $('#dismantledate').val() == "" ? null : $('#dismantledate').val()
        }

        console.log(JSON.stringify(data));
        $(this).val('Please wait...');

        var npi, npt;

        $.ajax({
            type: 'POST',
            url: '/Project/save',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                npi = result.newProjectId;
                npt = result.newProjectName;
                if (reloadProject == "true") {
                    console.log(result);
                    $('#submitProject').val('Save');
                    if (result.success == true) {
                        window.location = '/Project/Index';
                    }
                }
                else {
                    $('#cancelProject').click();
                    var project = document.getElementById("project");

                    project.options.add(new Option(npt, npi), project.options[project.options.length - 1]);
                    project.value = npi;
                }
            },
            error: function (error) {
                console.log(error);
                $('#submitProject').val('Save');
            }
        });
    }
}

function updateProject() {
    var isValid = true;

    if ($('#title').val().trim() == '') {
        $('#title').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#title').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('#projectno').val().trim() == '') {
        $('#projectno').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#projectno').siblings('span.error').css('visibility', 'hidden');
    }

    if ($('.projectCustomer').val().trim() == '00000000-0000-0000-0000-000000000000') {
        $('.projectCustomer').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('.projectCustomer').siblings('span.error').css('visibility', 'hidden');
    }

    //if ($('#installationtime').val().trim() == '') {
    //    $('#installationtime').siblings('span.error').css('visibility', 'visible');
    //    isValid = false;
    //}
    //else {
    //    $('#installationtime').siblings('span.error').css('visibility', 'hidden');
    //}

    if ($('#installationdate').val().trim() == '') {
        $('#installationdate').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#installationdate').siblings('span.error').css('visibility', 'hidden');
    }

    //if ($('#targetsales').val() == '') {
    //    $('#targetsales').siblings('span.error').css('visibility', 'visible');
    //    isValid = false;
    //}
    //else {
    //    $('#targetsales').siblings('span.error').css('visibility', 'hidden');
    //}

    if ($('#projectbudget').val().trim() == '') {
        $('#projectbudget').siblings('span.error').css('visibility', 'visible');
        isValid = false;
    }
    else {
        $('#projectbudget').siblings('span.error').css('visibility', 'hidden');
    }

    if (isValid) {
        var data = {
            ProjectId: $("#projectid").val(),
            Title: $('#title').val().trim(),
            CustomerId: $('.projectCustomer').val(),
            ProjectNo: $('#projectno').val().trim(),
            InstallationDate: $('#installationdate').val(),
            //InstallationTime: $('#installationtime').val(),
            //Status: $('#status').val(),
            //TargetSales: parseFloat($('#targetsales').val()),
            ProjectBudget: parseFloat($('#projectbudget').val()),
            Remarks: $('#remarks').val(),
            HandoverDate: $('#handoverdate').val() == "" ? null : $('#handoverdate').val(),
            DismantleDate: $('#dismantledate').val() == "" ? null : $('#dismantledate').val()
        }

        console.log(JSON.stringify(data));
        $(this).val('Please wait...');
        $.ajax({
            type: 'POST',
            url: '/Project/update',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (result) {
                console.log(result);
                $('#submitProject').val('Save');
                if (result.success == true) {
                    if (result.success == true) {
                        bootbox.alert(result.message, function () {
                            window.location.reload();
                        })

                    }
                }
            },
            error: function (error) {
                console.log(error);
                $('#submitProject').val('Save');
            }
        });
    }
}