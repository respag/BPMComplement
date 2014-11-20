var server = "http://" + location.host + "/";
//var server = "http://" + location.host + "/Audit/";
//var server = "http://" + location.host + "/ComponentManager/";

var Ultimus = {};

$(document).ajaxStart(function () {

    LoadMessage();


});

jQuery(document).ready(function () {

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "10000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "swing",
        "showMethod": "slideDown",
        "hideMethod": "slideUp"
    }

    $("form").submit(function () {
        if (!$(this).valid()) {
            return false;
        }
        else {
          
            LoadMessage();

            return true;
        }
    });

  

    $("[data-toggle=tooltip]").tooltip({ placement: 'right' });

    $('[data-toggle="popover"]').popover({ trigger: 'hover', 'placement': 'top' });

    jQuery('[data-confirm]').click(function (e) {
        if (!confirm(jQuery(this).attr("data-confirm"))) {
            e.preventDefault();
        }
        else {

            LoadMessage();

        }
    });

});

this.SendForm = function (form) {

    if (!$(form).valid()) {
        return false;
    }
    else {

        LoadMessage();

        form.submit();
    }
};

this.LoadMessage = function (form) {



        $("#toast-container").empty();

        var stringHtml = "";

        stringHtml += "<div id=\"loadingGif\" class=\"toast toast-default\">";
        stringHtml += "<table>";
        stringHtml += "<tr>";
        stringHtml += "<td><i class=\"fa fa-cog fa-spin fa-4x\"></i></td>";
        stringHtml += "<td>&nbsp;&nbsp;</td>";
        stringHtml += "<td>";
        stringHtml += "<div class=\"toast-title\">Cargando, por favor, espere!</div>";
        stringHtml += "</td>";
        stringHtml += "</tr>";
        stringHtml += "</table>";
        stringHtml += "</div>";

        $("#toast-container").append(stringHtml);


};

this.SendFormDLL = function (form) {


        LoadMessage();

        form.submit();
 
};
