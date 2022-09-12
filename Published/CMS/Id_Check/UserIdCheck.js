//$(function() {
var errorDob_s = "";

function ValidateClienID() {
    // first clear any left over error messages
    $('#error_message p').remove();
    $('#error_keyupValidate p').remove();

    // store the error div, to save typing
    var error = $('#error_message');
    errorDob_s = $('#error_keyupValidate');

    var idNumber = $('#IdNo').val();


    // assume everything is correct and if it later turns out not to be, just set this to false
    var correct = true;

    //Ref: http://www.sadev.co.za/content/what-south-african-id-number-made
    // SA ID Number have to be 13 digits, so check the length
    if (idNumber.length != 13 || !isNumber(idNumber)) {
        error.append('<p>ID number does not appear to be authentic - input not a valid number</p>');
        correct = false;
    }

    // get first 6 digits as a valid date
    var tempDate = new Date(idNumber.substring(0, 2), idNumber.substring(2, 4) - 1, idNumber.substring(4, 6));

    var id_date = tempDate.getDate();
    var id_month = tempDate.getMonth();
    var id_year = tempDate.getFullYear();

    /*var fullDate = id_date + "-" + id_month  + "-" + id_year;*/

    if (!((tempDate.getYear() == idNumber.substring(0, 2)) && (id_month == idNumber.substring(2, 4) - 1) && (id_date == idNumber.substring(4, 6)))) {
        error.append('<p>ID number does not appear to be authentic - date part not valid</p>');
        correct = false;
    }

    var id_year = idNumber.substring(0, 2);
    var id_month = idNumber.substring(2, 4)
    var id_date = idNumber.substring(4, 6)

    var cutoff = (new Date()).getFullYear() - 2000

    var fullDate = (id_year > cutoff ? '19' : '20') + id_year + '-' + id_month + '-' + id_date;
    //debugger

    // get the gender
    var genderCode = idNumber.substring(6, 10);
    var gender = genderCode.length == 4 ? parseInt(genderCode) < 5000 ? "Female" : "Male" : "";

    // get country ID for citzenship
    var citzenship = parseInt(idNumber.substring(10, 11)) == 0 ? "Yes" : "No";

    // apply Luhn formula for check-digits
    var tempTotal = 0;
    var checkSum = 0;
    var multiplier = 1;
    for (var i = 0; i < 13; ++i) {
        tempTotal = parseInt(idNumber.charAt(i)) * multiplier;
        if (tempTotal > 9) {
            tempTotal = parseInt(tempTotal.toString().charAt(0)) + parseInt(tempTotal.toString().charAt(1));
        }
        checkSum = checkSum + tempTotal;
        multiplier = (multiplier % 2 == 0) ? 1 : 2;
    }
    if ((checkSum % 10) != 0) {
        error.append('<p>ID number does not appear to be authentic - check digit is not valid</p>');
        correct = false;
    };


    // if no error found, hide the error message
    if (correct) {
        try {
            error.css('display', 'none');
            //document.getElementById("submit").disabled = false;

            // clear the result div
            $('#success_message').empty();

            //Set DOB
            document.getElementById("Dob").value = fullDate;

            //debugger
            //Set gender
            //document.getElementById("Gender").value = gender;
            //debugger
            var enteredAge = getAge(fullDate);
            ShowSuccessAlert(' Age:   ' + enteredAge + ' Years Old');

            ShowSuccessAlert(' SA Citizen:  ' + citzenship);
            ShowSuccessAlert(' Gender:  ' + gender);
            ShowSuccessAlert(' Birth Date:   ' + fullDate);
            ShowSuccessAlert(' South African ID Number:   ' + idNumber);

        } catch (e) {
            //debugger
            //alert(e);
        }
    }
    // otherwise, show the error
    else {
        //debugger
        error.css('display', 'block');

        //Set DOB
        document.getElementById("Dob").value = fullDate;

        validateDOB_M(fullDate);
        errorDob_s.css('display', 'block');
        //Set gender
        //document.getElementById("Gender").value = gender;
    }

    return false;
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function getAge(DOB) {
    //debugger
    var today = new Date();
    var birthDate = new Date(DOB);
    var age = today.getFullYear() - birthDate.getFullYear();
    var m = today.getMonth() - birthDate.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
        age--;
    }
    return age;
}

function validateDOB_M(DOB) {
    errorDob_s = $('#error_keyupValidate');

    var data = DOB.split("-");
    if (data[0] < 1000 || data[0] > 3000 || data[1] == 0 || data[1] > 12) {
        errorDob_s.append('<p>DOB number does not appear to be valid</p>');
        document.getElementById("submit").disabled = true;
        return false;
    } else {
        document.getElementById("submit").disabled = false;
    }

    return true;
}
