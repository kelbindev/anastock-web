$(document).ready(function () {
    LoadSalesChart();
    LoadActivities();
});
function LoadSalesChart() {
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetSalesData',
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
                }, 200);
            }
        }
    });
}

function LoadActivities() {
    var ul = '';
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetActivityData',
        success: function (data) {
            $.each(data, (k, v) => {
                ul = ul + `<li  class="list-group-item list-group-item-action flex-column align-items-start">
                            <p class="mb-1">${v["description"]}</p>
                            <small>${v["user"]}</small>
                        </li>`;
                });
            $("#activitygroup").html(ul);
        }
    });
}

