function loadItemSalesReport() {
    var _url = "/Report/GetItemSalesReport";
    var dateFrom = moment($('#dateFrom').val(), "DD/MM/YYYY").toDate();
    dateFrom = formatDate(dateFrom);
    var dateTo = moment($('#dateTo').val(), "DD/MM/YYYY").toDate();
    dateTo = formatDate(dateTo);
    var status = $('#Status').val();

    tableSales = $("#tbSales").DataTable({
        "ajax": {
            "destroy": true,
            "url": _url,
            "type": "GET",
            "datatype": "json",
            "dataSrc": "",
            "data": { dateFrom: dateFrom, dateTo: dateTo, status: status },
        },
        "bDestroy": true,
        "columns": [
            { "data": "name" },
            { "data": "uom"},
            { "data": "qty", render: $.fn.dataTable.render.number(',', '.', 2, '')},
            { "data": "total", render: $.fn.dataTable.render.number(',', '.', 2, '$')},
        ],
        "language": {
            "emptytable": "No data found"
        },
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ]
    });
}

function loadSalesDetails() {
    loadQuoteCountsDaily();
    LoadSalesChartDaily();
    loadItemSalesChartDaily();
}

function loadQuoteCountsDaily() {
    var _url = "/Report/GetSalesDetails";
    var dateFrom = moment($('#dateFromDaily').val(), "DD/MM/YYYY").toDate();
    dateFrom = formatDate(dateFrom);
    var dateTo = moment($('#dateToDaily').val(), "DD/MM/YYYY").toDate();
    dateTo = formatDate(dateTo);

    tableSalesDetails = $("#tbSalesDaily").DataTable({
        "ajax": {
            "destroy": true,
            "url": _url,
            "type": "GET",
            "datatype": "json",
            "dataSrc": "",
            "data": { dateFrom: dateFrom, dateTo: dateTo},
        },
        "bDestroy": true,
        "columns": [
            { "data": "name", orderable: false },
            { "data": "value", render: $.fn.dataTable.render.number(',', '.', 0, ''), orderable:false }
        ],
        "language": {
            "emptytable": "No data found"
        },
        dom: 't'
    });
}

function formatDate(date) {
    //YYYY-MM-DD FORMAT
    var result = date.getFullYear() + "-" +
        ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + "-" +
        ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));

    return result;
}

function LoadSalesChartDaily() {
    var dateFrom = moment($('#dateFromDaily').val(), "DD/MM/YYYY").toDate();
    dateFrom = formatDate(dateFrom);
    var dateTo = moment($('#dateToDaily').val(), "DD/MM/YYYY").toDate();
    dateTo = formatDate(dateTo);

    $.ajax({
        type: "GET",
        url: '/Report/GetSalesData',
        "data": { dateFrom: dateFrom, dateTo: dateTo },
        success: function (data) {
            var period = [];
            var total = [];
            var balancedue = [];
            $.each(data, function (propName, propVal) {
                period.push(propVal["period"]);
                total.push(propVal["total"]);
                balancedue.push(propVal["balanceDue"]);
            });

            var chartDom = document.getElementById('salesChart');
            var myChart = echarts.init(chartDom);
            var option;

            option = {
                tooltip: {
                    trigger: 'axis'
                },
                xAxis: {
                    type: 'category',
                    data: period
                },
                yAxis: {
                    type: 'value'
                },
                series: [{
                    data: total,
                    type: 'line',
                    name: 'Total',
                    color: '#66BB6A'
                },
                {
                    data: balancedue,
                    type: 'line',
                    name: 'Balance Due',
                    color: "#D84315"
                }
                ],
                yAxis: {
                    type: 'value',
                    axisLabel: {
                        formatter: '${value}'
                    }
                },
                legend: {
                    data: ['Total', 'Balance Due']
                },
            };

            myChart.setOption(option);

            $(window).on('resize', resize);
            function resize() {
                setTimeout(function () {
                    // Resize chart
                    myChart.resize();
                }, 250);
            }
        }
    });
}

function loadItemSalesChartDaily() {
    var dateFrom = moment($('#dateFromDaily').val(), "DD/MM/YYYY").toDate();
    dateFrom = formatDate(dateFrom);
    var dateTo = moment($('#dateToDaily').val(), "DD/MM/YYYY").toDate();
    dateTo = formatDate(dateTo);

    $.ajax({
        type: "GET",
        url: '/Report/GetItemSalesChartData',
        "data": { dateFrom: dateFrom, dateTo: dateTo },
        success: function (data) {
            var productName = [];
            var total = [];
            $.each(data, function (propName, propVal) {
                productName.push(propVal["productName"]);
                total.push(propVal["total"]);
            });

            var chartDom = document.getElementById('salesByItemChart');
            var myChart = echarts.init(chartDom);
            var option;

            option = {
                tooltip: {
                    trigger: 'axis'
                },
                xAxis: {
                    type: 'category',
                    data: productName
                },
                yAxis: {
                    type: 'value'
                },
                series: [{
                    data: total,
                    type: 'line',
                    name: 'Total',
                    color: '#66BB6A'
                }
                ],
                yAxis: {
                    type: 'value',
                    axisLabel: {
                        formatter: '${value}'
                    }
                },
                legend: {
                    data: ['Total']
                },
            };

            myChart.setOption(option);

            $(window).on('resize', resize);
            function resize() {
                setTimeout(function () {
                    // Resize chart
                    myChart.resize();
                }, 250);
            }
        }
    });
}