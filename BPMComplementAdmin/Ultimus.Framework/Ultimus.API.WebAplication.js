
this.ShowWebAplicationRelated = function (id) {

    var uri = 'api/WebAplications/GetAllWebAplications/';

    $.getJSON(server + uri + id).done(function (data) {

        var UI = new Ultimus.UI();
        var tableHtml = "";

        tableHtml += "<tr>";
        //tableHtml += "<th>#</th>";
        tableHtml += "<th>Aplicación Web</th>";
        tableHtml += "<th>Url</th>";
        tableHtml += "<th></th>";
        tableHtml += "</tr>";

        ColumnToAudit = data;

        // On success, 'data' contains a list of Connections.
        $.each(data, function (key, item) {


            if (id == item.IdProcess) {

                tableHtml += "<tr class=\"success\">";
            }
            else {
                tableHtml += "<tr>";
            }

            //tableHtml += "<td>" + item.IdWebAplication + "</td>";
            tableHtml += "<td>" + item.WebAplicationName + "</td>";
            tableHtml += "<td>" + item.WebAplicationPath + "</td>";

            if (id == item.IdProcess) {

                if (item.isAddedToProcess) {

                    tableHtml += "<td>Menu Configurated</td>";
                }
                else {
                    tableHtml += "<td>" + btnRemoveWebAplication(id, item.IdWebAplication) + "</td>";
                }
            }
            else {

                tableHtml += "<td>" + btnAddWebAplication(id, item.IdWebAplication) + "</td>";
            }

            tableHtml += "</tr>";

        });

        tableHtml = "<table class='table table-striped'>" + tableHtml + "</table>";

        $("#WebAplicationRelatedBody").empty();
        $('#WebAplicationRelatedBody').append(tableHtml);

        $("#loadingGif").hide();

        $('#WebAplicationRelated').modal('show');

        $('.modal-content').css({ 'height': '600px', 'overflow': 'auto' });
    });

};


this.btnAddWebAplication = function (id, IdWebAplication) {

    var tableHtml = "";

    tableHtml += "<button id=\"btnAddWebAplication\" type=\"button\" class=\"btn btn-info btn-xs\" "
    tableHtml += "onClick=\"btnAddWebAplication_OnClick('" + id + "', '" + IdWebAplication + "');\">";
    tableHtml += "Agregar Aplicación";
    tableHtml += "</button>";

    return tableHtml;
};

this.btnAddWebAplication_OnClick = function (id, IdWebAplication) {

    var uri = 'api/WebAplications/SaveWebAplicationsRelation/';

    CatWebAplications = { "IdWebAplication": IdWebAplication, "IdProcess": id }

    var DTO = JSON.stringify(CatWebAplications);

    $.ajax({
        type: 'POST',
        url: server + uri,
        cache: false,
        data: DTO,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
     
            if (data == "DONE") {
                TablesToAudit = new Array();
                toastr.success('Your configuration was saved successfully', "Success!");

                ShowWebAplicationRelated(id);
            }
            else {
                TablesToAudit = new Array();
                toastr.error("An unexpected error has occurred.Please contact Ultimus Support.", "Ups!");
            }
        }
    });
};

this.btnRemoveWebAplication = function (id, IdWebAplication) {

    var tableHtml = "";

    tableHtml += "<button id=\"btnAddWebAplication\" type=\"button\" class=\"btn btn-danger btn-xs\" "
    tableHtml += "onClick=\"btnRemoveWebAplication_OnClick('" + id + "', '" + IdWebAplication + "');\">";
    tableHtml += "Remove Web Aplication";
    tableHtml += "</button>";

    return tableHtml;
};

this.btnRemoveWebAplication_OnClick = function (id, IdWebAplication) {

    var uri = 'api/WebAplications/RemoveWebAplicationsRelation/';

    CatWebAplications = { "IdWebAplication": IdWebAplication, "IdProcess": id }

    var DTO = JSON.stringify(CatWebAplications);

    $.ajax({
        type: 'POST',
        url: server + uri,
        cache: false,
        data: DTO,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            if (data == "DONE") {

                TablesToAudit = new Array();
                toastr.success('Your configuration was saved successfully', "Success!");

                ShowWebAplicationRelated(id);
            }
            else {
                TablesToAudit = new Array();
                toastr.error("An unexpected error has occurred.Please contact Ultimus Support.", "Ups!");
            }
        }
    });



};