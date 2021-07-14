$(document).ready(function () {
    try {
        $('#divTaskDetails').hide();
        $('#popoverOption').hide();
        $('#popoverOption').popover({ trigger: "hover" });
        $('#PersonListSelected').multiselect({
            enableFiltering: true,
            //buttonWidth: '400px',
            filterPlaceholder: 'Search for Name',
            enableCaseInsensitiveFiltering: true,
            maxHeight: 200,
            onChange: function (element, checked) {
                var brands = $('#PersonListSelected option:selected');
                //if (brands.length > 3){
                //    $('#popoverOption').show();
                //}
                //else{
                //    $('#popoverOption').hide();
                //}
                var selected = [];
                var selectedName = [];
                $(brands).each(function (index, brand) {
                    selected.push([$(this).val()]);
                    if (selectedName.length == 0) {
                        selectedName.push([$(this).text()]);
                    }
                    else {
                        selectedName.push(["<br />" + $(this).text()]);
                    }
                    $('#PersonListValue').val(selected);
                });
                $('#popoverOption').attr('data-content', selectedName);
            }
        });
        $('#OwnerId').multiselect({
            enableFiltering: true,
            maxHeight: 200,
            filterPlaceholder: 'Search for Name',
            enableCaseInsensitiveFiltering: true
        });
        $('#yearlyDiv').hide();
        $('#MonthlyDiv').hide();
        $('#QuaterlyDiv').hide();
        $('#WeeklyDiv').hide();
        
        // $('#divdatefrequency').hide();
        $('#lvlFinalDueDate').hide();
        $('#divFinalDueDate').hide();
        $('#lvlInitialDueDate').text('Due Date');
        SetDate($('#StartDate').val());
        //debugger;
        $('#FrequencyListValue').change(function () {
            $('#yearlyDiv').hide();
            $('#MonthlyDiv').hide();
            $('#QuaterlyDiv').hide();
            $('#WeeklyDiv').hide();
            $('#divdatefrequency').show();
            var frequencyVal = $(this).val();
            if (frequencyVal == 1) {//One time
                $('#lvlInitialDueDate').text('Due Date');
                $('#lvlFinalDueDate').hide();
                $('#divFinalDueDate').hide();
                $('#divdatefrequency').hide();
            }
            else if (frequencyVal == 4) {//Monthly
                $('#MonthlyDiv').show();
            }
            else if (frequencyVal == 6) {//Quaterly
                $('#QuaterlyDiv').show();
            }
            else if (frequencyVal == 8) {//Yearly
                $('#yearlyDiv').show();
            }
            else if (frequencyVal == 3) {//weekly
                $('#WeeklyDiv').show();
            }
            
            if (frequencyVal != 1) {
                $('#lvlInitialDueDate').text('Initial Due Date');
                $('#lvlFinalDueDate').show();
                $('#divFinalDueDate').show();
            }
        });
        if ($('#FrequencyListValue').val() == 1) {
            $('#lvlInitialDueDate').text('Due Date');
            $('#lvlFinalDueDate').hide();
            $('#divFinalDueDate').hide();
        } else if ($('#FrequencyListValue').val() == 4) { // monthly

            $('#lvlInitialDueDate').text('Initial Due Date');
            $('#lvlFinalDueDate').show();
            $('#divFinalDueDate').show();
            $('#MonthlyDiv').show();
        } else if ($('#FrequencyListValue').val() == 6) { // quartly
            $('#lvlInitialDueDate').text('Initial Due Date');
            $('#lvlFinalDueDate').show();
            $('#divFinalDueDate').show();
            $('#QuaterlyDiv').show();
        }
        else if ($('#FrequencyListValue').val() == 8) { // yearly
            $('#lvlInitialDueDate').text('Initial Due Date');
            $('#lvlFinalDueDate').show();
            $('#divFinalDueDate').show();
            $('#yearlyDiv').show();
        }
        else if ($('#FrequencyListValue').val() == 3) { // weekly
            $('#lvlInitialDueDate').text('Initial Due Date');
            $('#lvlFinalDueDate').show();
            $('#divFinalDueDate').show();
            $('#WeeklyDiv').show();
            //$('#$('#WeeklyDiv')').show();
        }
        else {
            $('#lvlFinalDueDate').hide();
            $('#divFinalDueDate').hide();
        }
        var brands = $('#PersonListSelected option:selected');
        //if (brands.length > 3) {
        //    $('#popoverOption').show();
        //}
        //else {
        //    $('#popoverOption').hide();
        //}
        var selectedName = [];
        $(brands).each(function (index, brand) {
            if (selectedName.length == 0) {
                selectedName.push([$(this).text()]);
            }
            else {
                selectedName.push(["<br />" + $(this).text()]);
            }
        });
        $('#popoverOption').attr('data-content', selectedName);

        $('#StartDate').change(function () {
            var date = new Date($('#StartDate').val());
            //var date = $(this).datepicker('getDate'),
            var day = date.getDate(),
                month = date.getMonth() + 1,
                year = date.getFullYear();
            $('#yearlyMonth').val(month);
            $('#yearlyDay').val(day.toString());
            $('#monthlyDay').val(day.toString());
            $('#quaterlyDay').val(day.toString());
        });

        //var date = new Date();
        //date.setDate(date.getDate() - 1);
        $('.datepicker').datepicker().on('changeDate', function (e) {
            SetDate($('#StartDate').val());
            //var date = new Date($('#StartDate').val());

        });
        $('#monthlyDay').change(function () {
            // Step 1 : create the date
            // Step 2 : Check if the date is valid
            // Step 3 : If date is valid then update the main date
            var date = new Date($('#StartDate').val());
            if (validateDate($('#StartDate').val()) == false) {
                var date = new Date();
            }
            var day = $('#monthlyDay').val();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            var dd = month.toString() + "/" + day.toString() + "/" + year.toString();
            if (validateDate(dd) == true) {
                $('#StartDate').val(dd);
            }
        });
        $('#quaterlyDay').change(function () {
            var date = new Date($('#StartDate').val());
            if (validateDate($('#StartDate').val()) == false) {
                var date = new Date();
            }
            var day = $('#quaterlyDay').val();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            var dd = month.toString() + "/" + day.toString() + "/" + year.toString();
            if (validateDate(dd) == true) {
                $('#StartDate').val(dd);
            }
        });
        $('#yearlyDay').change(function () {
            var date = new Date($('#StartDate').val());
            if (validateDate($('#StartDate').val()) == false) {
                var date = new Date();
            }
            var day = $('#yearlyDay').val();
            var month = $('#yearlyMonth').val();
            var year = date.getFullYear();
            var dd = month.toString() + "/" + day.toString() + "/" + year.toString();
            if (validateDate(dd) == true) {
                $('#StartDate').val(dd);
            }
        });
        $('#yearlyMonth').change(function () {
            var date = new Date($('#StartDate').val());
            if (validateDate($('#StartDate').val()) == false) {
                var date = new Date();
            }
            var day = date.getDate();
            var month = $('#yearlyMonth').val();
            var year = date.getFullYear();
            var dd = month.toString() + "/" + day.toString() + "/" + year.toString();
            if (validateDate(dd) == true) {
                $('#StartDate').val(dd);
            }
        });
        function SetDate(d) {
            var date = new Date(d);
            var day = date.getDate(),
                month = date.getMonth() + 1,
                year = date.getFullYear();
            $('#yearlyMonth').val(month);
            $('#yearlyDay').val(day.toString());
            $('#monthlyDay').val(day.toString());
            $('#quaterlyDay').val(day.toString());
        }
        function validateDate(d) {
            var comp = d.split('/');
            var m = parseInt(comp[0], 10);
            var d = parseInt(comp[1], 10);
            var y = parseInt(comp[2], 10);
            var date = new Date(y, m - 1, d);
            if (date.getFullYear() == y && date.getMonth() + 1 == m && date.getDate() == d) {
                return true;
            } else {
                return false;
            }
        }
        $("#monthlyDay").focusout(function () {
            var day = $('#monthlyDay').val();
            if (!$.isNumeric(day)){
                alert("Day of the month should be number");
            }
        });
        $("#yearlyDay").focusout(function () {
            var day = $('#yearlyDay').val();
            if (!$.isNumeric(day)) {
                alert("Day of the month should be number");
            }
        });
        $("#quaterlyDay").focusout(function () {
            var day = $('#quaterlyDay').val();
            if (!$.isNumeric(day)) {
                alert("Day of the month should be number");
            }
        });
        $('#divTaskDetails').show();
    }
    catch (excpetion) {
    }
});