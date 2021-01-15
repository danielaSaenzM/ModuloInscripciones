$(document).ready(function () {
    var list = $("#list").val();
    switch (list) {
        case "ALL":
            $("#listALL a").css("background-color", "red");
            break;
        case "AP":
            $("#listAP a").css("background-color", "red");
            break;
        case "AG":
            $("#listAG a").css("background-color", "red");
            break;
    }

    $("#salir").click(function () {
        document.location.href = $(this).val();
    })
});