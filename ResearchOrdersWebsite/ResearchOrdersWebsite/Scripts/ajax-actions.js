//$(function () {
//    $('.detailsLink').click(function () {
//        $.ajax({
//            type: "GET",
//            url: this.href,
//            success: function (data, textStatus, jqXHR) {
//                $('#orderDetailsPartial').html(data);
//            }
//        });
//    });
//});

$(document).ready(function () {
    $(".btn").click(function () {
        $("#toggleDNF").collapse('toggle');
    });
});
