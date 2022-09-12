$(document).ready(function () {
    // Set new default font family and font color to mimic Bootstrap's default styling
    Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
    Chart.defaults.global.defaultFontColor = '#858796';

    // Pie Chart Example
    var ctx = document.getElementById("myPieChart");
    let labels = [];
    let data = [];
    var jsonObj;

    $.ajax({
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        url: '/home/pieChart',
        data: '{}',
        success: function (response) {
            debugger
            jsonObj = JSON.parse(response);

            for (i = 0; i < jsonObj.length; i++) {
                data[i] = jsonObj[i].data;
                labels[i] = jsonObj[i].labels;
            }

            //alert(JSON.stringify(jsonObj.map(labels => labels.labels)));
            //alert(jsonObj.map(data => data.data));

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    //labels:["License Dis Renewal","Vehicle Registration","Change Of Ownership","Roadworthy"]
                    labels: JSON.stringify(jsonObj.map(labels => labels.labels)),
                    datasets: [{
                        //data: [0,1,1,0]
                        data: [jsonObj.map(data => data.data)],
                        backgroundColor: ['#4e73df', '#fc0303', '#36b9cc', '#fc8803'],
                        hoverBackgroundColor: ['#2e59d9', '#fc4949', '#2c9faf', '#f79a2f'],
                        hoverBorderColor: "rgba(234, 236, 244, 1)",
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        caretPadding: 10,
                    },
                    legend: {
                        display: false
                    },
                    cutoutPercentage: 80,
                },
            });


        },

    });


});