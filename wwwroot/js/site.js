/// <reference path="../lib/signalr/dist/browser/signalr.js" />
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//$(() => {
//    let connection = new signalR.HubConnectionBuilder().withUrl("/signalServer").build();

//    connection.start();

//    connection.on("refreshProductAndService", function () {
//        loadData();
//    });

//    loadData();

//    function loadData() {
//        var tr = '';

//        $.ajax({
//            url: '/MasterData/GetProductAndServices',
//            method: 'GET',
//            success: (result) => {
//                $.each(result, (k, v) => {
//                    var isAvailable = v.isAvailable ? 'available' : 'not available';
//                    var className = v.isAvailable ? 'badge badge-success' : 'not badge badge-warning';
//                    tr = tr + `<tr>
//                        <td>${v.name}</td>
//                        <td>${v.category}</td>
//                        <td>
//                            <a href="/MasterData/EditProduct/${v.id}">Edit</a>
//                            <a onclick="return confirm ('Are you sure want to delete product?');" href="/MasterData/DeleteProduct?productId=${v.id}">Delete</a>
//                        </td>
//                    </tr>`;
//                });
//                $("#tableBody").html(tr);
//            },
//            error: (error) => {
//                console.log(error);
//            }
//        });
//    }
//});
