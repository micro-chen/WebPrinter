/// <reference path="jquery-1.8.2.js" />


$(document).ready(function () {
    var btnGetAllProducts = $("#btn_get_all_products");
    var zoneOfResult = $("#txt_result");
    btnGetAllProducts.click(function () {

        zoneOfResult.text('');

        var timestamp = new Date().getTime();
        $.get(("http://localhost:6699/api/Product/GetAllProducts?tm=" + timestamp), function (data) {

            if (!data) {
                alert("error lalallala");
            }
            var msg = '';
            for (var i = 0; i < data.length; i++) {
                msg += data[i].Name + '\r\n';
            }
            zoneOfResult.text(msg);

        });
    });


});