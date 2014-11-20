var TablesToAudit = new Array();
var ColumnToAudit = [];
var SaveTableConfigRequest = [];
var SaveColumnConfigRequest = [];
var lista = [];

jQuery(document).ready(function () {

    FillSelectProcessList();
    FillSelectStepList();
    FillTableList();
    GetTablesConfiguratedByStep();




    //toastr.info('Are you the 6 fingered man?');

    //// Display a warning toast, with no title
    //toastr.warning('My name is Inigo Montoya. You killed my father, prepare to die!');

    //// Display a success toast, with a title
    //toastr.success('Have fun storming the castle!', 'Miracle Max Says');

    //// Display an error toast, with a title
    //toastr.error('I do not think that word means what you think it means.', 'Inconceivable!');

});

//#region Fill DropDownList controls

this.FillSelectProcessList = function () {

    var uri = app + 'api/AuditConfiguration/GetAllProcess';

    $("#selectSteps").prop('disabled', true);
    $("#selectConnectionsOrigin").prop('disabled', true);
    $("#selectConnectionsDestination").prop('disabled', true);



    $.getJSON(uri).done(function (data) {

        var UI = new Ultimus.UI();
        UI.CleanSelectControl("#selectProcesses");

        // On success, 'data' contains a list of products.
        $.each(data, function (key, item) {

            UI.AddSelectOption("#selectProcesses", item.IdProcess, item.ProcessName);

        });

        UI.ENDREQUEST();
    });

 

};

this.FillSelectStepList = function () {
 
    var uri = app + 'api/AuditConfiguration/GetStepByProcessId/';

    $("#selectProcesses").change(function () {

        var UI = new Ultimus.UI();
        var seletedId = $("#selectProcesses option:selected").val();

        $("#selectSteps option:selected").val(-1);
        $("#content").empty();

        if (seletedId == -1) {

            UI.ENDREQUEST();
            return;
        }

        //$.getJSON(server + uri + seletedId)
        $.getJSON(uri + seletedId)
         .done(function (data) {

             var UI = new Ultimus.UI();
             UI.CleanSelectControl("#selectSteps");

             // On success, 'data' contains a list of products.
             $.each(data, function (key, item) {

                 UI.AddSelectOption("#selectSteps", item.IdStep, item.StepName);

             });

             $("#selectSteps").prop('disabled', false);
             FillSelectConnectionList();
         });
    });

};

this.FillSelectConnectionList = function () {

    var uri = app +'api/AuditConfiguration/GetConnections/';


    var seletedId = $("#selectProcesses option:selected").val();

    $.getJSON( uri + seletedId).done(function (data) {

        var UI = new Ultimus.UI();

        UI.CleanSelectControl("#selectConnectionsOrigin");
        UI.CleanSelectControl("#selectConnectionsDestination");

        // On success, 'data' contains a list of Connections.
        $.each(data.CatConnectionsList, function (key, item) {

            UI.AddSelectOption("#selectConnectionsOrigin", item.IdConnections, item.ConnectionName);
            UI.AddSelectOption("#selectConnectionsDestination", item.IdConnections, item.ConnectionName);

        });

        //if (data.haveConnectionConfig == true) {

        //    $("#selectConnectionsOrigin").val(data.SeletedOriginId);
        //    $("#selectConnectionsDestination").val(data.SeletedDestinationId);

        //    $("#selectConnectionsOrigin").prop('disabled', true);
        //    $("#selectConnectionsDestination").prop('disabled', true);

        //}
        //else {

            $("#selectConnectionsOrigin").prop('disabled', false);
            $("#selectConnectionsDestination").prop('disabled', false);

        //}

        UI.ENDREQUEST();
 
    });

};

//#endregion

//#region Add tables

this.AddtableToAudit = function (varTableName) {

    TablesToAudit.push({ TableName: varTableName });

};

this.FillTableList = function () {

    var uri = app + 'api/AuditConfiguration/GetTablesByConnectionId/';

    $("#btnAddTablesToAudit").click(function () {

        var seletedProcessId = $("#selectProcesses option:selected").val();
        var seletedStepId = $("#selectSteps option:selected").val();
        var seletedOriginId = $("#selectConnectionsOrigin option:selected").val();
        var seletedDestinationId = $("#selectConnectionsDestination option:selected").val();

        if (seletedProcessId == -1) 
            toastr.warning('Debe seleccionar un <strong>Proceso</strong>.');

        if (seletedStepId == -1)
            toastr.warning('Debe seleccionar una <strong>Etapa</strong>.');

        if (seletedOriginId == -1)
            toastr.warning('Debe seleccionar un <strong>conector de origen</strong>.');


        if (seletedDestinationId == -1)
            toastr.warning('Debe seleccionar un <strong>conector de destino</strong>.');

        if ((seletedProcessId == -1) ||
            (seletedStepId == -1) ||
            (seletedOriginId == -1) ||
            (seletedDestinationId == -1)) return;

        //$.getJSON(server + uri + seletedOriginId + "/" + seletedStepId).done(function (data) {
          $.getJSON(uri + seletedOriginId + "/" + seletedStepId).done(function (data) { 

            var UI = new Ultimus.UI();
            var tableHtml = "";

            tableHtml += "<tr>";
            tableHtml += "<th>Tablas disponibles en la base de datos</th>";
            tableHtml += "<th>Agregar a Auditoría</th>";
            tableHtml += "</tr>";

            // On success, 'data' contains a list of products.
            $.each(data, function (key, item) {

                //#region Checkbox control

                var CheckboxControl = "";

                CheckboxControl += "<input type=\"checkbox\" class=\"form-control\" style=\"width:20px;height:20px;\"";
                CheckboxControl += "value=\"" + item.TableName + "\"";
                CheckboxControl += "onchange=\"javascript: AddtableToAudit('" + item.TableName + "')\"></input>";

                //#endregion

                tableHtml += "<tr>";
                tableHtml += "<td>" + item.TableName + "</td>";
                tableHtml += "<td>" + CheckboxControl + "</td>";
                tableHtml += "</tr>";

            });

            tableHtml = "<table class='table table-striped table-hover'>" + tableHtml + "</table>";

            $("#modalTableBody").empty();
            $('#modalTableBody').append(tableHtml);

            UI.ENDREQUEST();
            $('#modalTableConfig').modal('show');

            $('#modalTableConfig .modal-content').css({'height':'600px', 'overflow':'auto'});


        });

    });


};

$("#btnSaveTableConfiguration").click(function () {

    var uri = app + 'api/AuditConfiguration/SaveTablesToAudit/';

    SaveTableConfigRequest = {
        SeletedProcessId: $("#selectProcesses option:selected").val(),
        SeletedStepId: $("#selectSteps option:selected").val(),
        SeletedOriginId: $("#selectConnectionsOrigin option:selected").val(),
        SeletedDestinationId: $("#selectConnectionsDestination option:selected").val(),
        TablesToAudit: TablesToAudit
    };

    var DTO = JSON.stringify(SaveTableConfigRequest);

    $.ajax({
        type: 'POST',
        url: uri,
        cache: false,
        data: DTO,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            if (data == "DONE") {
                TablesToAudit = new Array();
                toastr.success('Su configuración fue grabada exitosamente', "Éxito!");
                GetTablesByStepId();
                $('#modalTableConfig').modal("hide");

                $("#selectConnectionsOrigin").prop('disabled', true);
                $("#selectConnectionsDestination").prop('disabled', true);
      
            }
            else {
                TablesToAudit = new Array();
                toastr.error("Ha ocurrido uin error inesperado.Por favor, contacte al soporte de Ultimus.", "Error");
            }
        }
    });

});

this.GetTablesByStepId = function () {

    var uri = app + 'api/AuditConfiguration/GetTablesByStepId/';

    var seletedId = $("#selectSteps option:selected").val();
  
    $.getJSON(uri + seletedId).done(function (data) {

        var UI = new Ultimus.UI();
        var tableHtml = "";

        tableHtml += "<tr>";
        tableHtml += "<th>Tablas disponibles en la base de datos</th>";
        tableHtml += "<th>Estado</th>";
        tableHtml += "<th></th>";
        tableHtml += "</tr>";

        if (data.length > 0) {

            // On success, 'data' contains a list of Connections.
            $.each(data, function (key, item) {

                tableHtml += "<tr>";
                tableHtml += "<td>" + item.AuditTablesDefinition.TableName + "</td>";

                //#region Status definition
                
                if (item.AuditTablesDefinition.CatAuditTablesStatus.IdTablesStatus.trim() == "N") {
                    tableHtml += "<td>" + UI.Createlabel(item.AuditTablesDefinition.CatAuditTablesStatus.StatusName, "info") + "</td>";
                }
                else if (item.AuditTablesDefinition.CatAuditTablesStatus.IdTablesStatus.trim() == "PC") {
                    tableHtml += "<td>" + UI.Createlabel(item.AuditTablesDefinition.CatAuditTablesStatus.StatusName, "info") + "</td>";
                }
                else if (item.AuditTablesDefinition.CatAuditTablesStatus.IdTablesStatus.trim() == "P") {
                    tableHtml += "<td>" + UI.Createlabel(item.AuditTablesDefinition.CatAuditTablesStatus.StatusName, "success") + "</td>";
                }
                else if (item.AuditTablesDefinition.CatAuditTablesStatus.IdTablesStatus.trim() == "D") {
                    tableHtml += "<td>" + UI.Createlabel(item.AuditTablesDefinition.CatAuditTablesStatus.StatusName, "danger") + "</td>";
                }

                //#endregion

                tableHtml += "<td style=\"text-align: right;\">" + CreateBottonTableOption(item.IdTableDefinition) + "</td>";
                tableHtml += "</tr>";

            });

            tableHtml = "<table class='table table-striped'>" + tableHtml + "</table>";
            UI.InsertContent(tableHtml);
        }
        else {
            $("#content").empty();
            toastr.info("Esta <strong>Etapa</strong> no tiene ninguna tabla configurada para Auditoría.");
        }
        
        UI.ENDREQUEST();
    });

};

this.GetTablesConfiguratedByStep = function () {

    $("#selectSteps").change(function () {

        GetTablesByStepId();

    });


};

this.CreateBottonTableOption = function (varTableName) {

    var stringHtml = "";

    stringHtml += "<div class=\"btn-group\">";
    stringHtml += " <button type=\"button\" class=\"btn btn-success btn-xs\" onClick=\"PublishTable('" + varTableName + "');\">";
    stringHtml += " <i class=\"fa fa-upload\"></i>&nbsp;Publicar</button>";

    stringHtml += " <button type=\"button\" class=\"btn btn-primary btn-xs\" onClick=\"ColumnConfigDialog('" + varTableName + "');\">";
    stringHtml += " <i class=\"fa fa-wrench\"></i>&nbsp;Columnas</button>";

    stringHtml += " <button type=\"button\" class=\"btn btn-danger btn-xs\" onClick=\"\">";
    stringHtml += " <i class=\"fa fa-times-circle\"></i>&nbsp;Deshabilitar</button>";

    stringHtml += " </div>";

    return stringHtml;

};

//#endregion

//#region Column configuration

this.UpdateColumn_isKey = function ( varColumnName, valueToUpdate) {

    for (var item in ColumnToAudit) {
        if (ColumnToAudit[item].ColumnName == varColumnName) {
            ColumnToAudit[item].IsKey = valueToUpdate;
        }
    }
};

this.UpdateColumn_IsAuditColumn = function ( varColumnName, valueToUpdate) {

    for (var item in ColumnToAudit) {
        if (ColumnToAudit[item].ColumnName == varColumnName) {
            ColumnToAudit[item].IsAuditColumn = valueToUpdate;
        }
    }
};

this.GetColumnByName = function (data, varColumnName) {

    for (var item in data) {
        
        if (data[item].ColumnName == varColumnName) {
            return data[item];
        }
    }
    return null;
};

this.ColumnConfigDialog = function (varIdTableDefinition) {

    var uri = app + 'api/AuditConfiguration/GetColumnByTableId/';

    $.getJSON(uri + varIdTableDefinition).done(function (data) {

        var UI = new Ultimus.UI();
        var tableHtml = "";

        tableHtml += "<tr>";
        tableHtml += "<th>Column Name</th>";
        tableHtml += "<th>Type</th>";
        tableHtml += "<th>Length</th>";
        tableHtml += "<th></th>";
        tableHtml += "</tr>";

        ColumnToAudit = data;

        // On success, 'data' contains a list of Connections.
        $.each(data, function (key, item) {


            if (item.IsKey) {
     
                tableHtml += "<tr class=\"success\">";
            }
            else if (item.IsAuditColumn) {
                tableHtml += "<tr class=\"warning\">";
            }
            else {
                tableHtml += "<tr>";
            }

            tableHtml += "<td>" + item.ColumnName + "</td>";
            tableHtml += "<td>" + item.DataType + "</td>";
            tableHtml += "<td>" + item.DataLength + "</td>";
         
            if (item.IsKey) {
             
                tableHtml += "<td>Llave Primaria</td>";
            }
            else if (item.IsAuditColumn)
            {
                tableHtml += "<td>En Auditoría</td>";
            }
            else {
                tableHtml += "<td>" + CreateAddColumn(item) + "</td>";
            }


            tableHtml += "</tr>";

        });

        tableHtml = "<table class='table table-striped'>" + tableHtml + "</table>";

        $("#modalBody").empty();
        $('#modalBody').append(tableHtml);

        $("#loadingGif").hide();

        $('#modalColumnConfig').modal('show');

        $('#modalColumnConfig .modal-content').css({ 'height': '600px', 'overflow': 'auto' });
    });


};

this.CreateAddColumn = function (item) {

    var stringHtml = "";

    stringHtml += "<input type=\"checkbox\" class=\"form-control\" style=\"width:20px;height:20px;\" ";

    if (item.IsAuditColumn == true) {
        stringHtml += " disabled=\"true\"";
    }

    stringHtml += " value=\"" + item.ColumnName + "\" ";
    stringHtml += "onchange=\"javascript: AddColumnToAudit('" + item.ColumnName + "', this)\"></input>";

    return stringHtml;
};

this.CreateKeyColumn = function (item) {

    var stringHtml = "";

    stringHtml += "<input type=\"checkbox\" class=\"form-control\" style=\"width:20px;height:20px;\" ";

    if (item.IsAuditColumn == true) {
        stringHtml += " disabled=\"true\"";
    }

    stringHtml += " value=\"" + item.ColumnName + "\" ";
    stringHtml += "onchange=\"javascript: KeyColumnToAudit('" + item.ColumnName + "', this)\"></input>";

    return stringHtml;
};

this.AddColumnToAudit = function (varColumnName, varSelection) {

    UpdateColumn_IsAuditColumn( varColumnName, varSelection.checked);

};

this.KeyColumnToAudit = function (varColumnName, varSelection) {

    UpdateColumn_isKey(varColumnName, varSelection.checked);

};

$("#btnSaveColumnConfiguration").click(function () {

    var uri = app + 'api/AuditConfiguration/SaveColumnToAudit/';

    var DTO = JSON.stringify(ColumnToAudit);

    // Agrego un campo Numero a la tabla de auditoria
    var p = DTO.substring(1, DTO.length - 1);
    //Obtengo el IdTableDef
    var stringArr = p.split(",");
    var id = stringArr[5].substring(20);
    id = id.replace('}', '');
    p = '[{"ColumnName": "Numero", "DataType": "int", "DataLength": 4, "IsKey": true, "IsAuditColumn": true, "IdTableDefinition":' + id + '},';
    p += DTO.substring(1);
    DTO = p;

    $.ajax({
        type: 'POST',
        url: uri,
        cache: false,
        data: DTO,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            if (data == "DONE") {

                ColumnToAudit = new Array();
                toastr.success('Su configuración fue grabada exitosamente', "Éxito!");
                GetTablesByStepId();
                $('#modalColumnConfig').modal("hide");

            }
            else if  (data == "NO_CHANGES") {

                ColumnToAudit = new Array();
                toastr.warning('No se detectaron cambios de auditoría', "Atención!");
                GetTablesByStepId();
                $('#modalColumnConfig').modal("hide");

            }
            else {
                toastr.error("Ha ocurrido un error inesperado.Por favor, contacte al soporte de Ultimus.", "Error!");
            }
        }
    });

});

//#endregion

//#region Publish Table

this.PublishTable = function (varIdTableDefinition) {

    var uri =  app + 'api/AuditConfiguration/PublishTable/';

    $.getJSON( uri + varIdTableDefinition).done(function (data) {

        var UI = new Ultimus.UI();

        if (data == "DONE") {

            toastr.success('La <strong>Tabla</strong> fue publicada exitosamente', "Exito!");
            GetTablesByStepId();
        }
        else if (data == "NO_COLUMNS") {

            toastr.warning("La <strong>Tabla</strong> no puede publicarse porque no tiene columnas configuradas", "Atención!");
            GetTablesByStepId();
        }
        else if (data == "TABLE_ALREADY_PUBLISHED") {

            toastr.warning("La <strong>Tabla</strong> no puede publicarse porque ya ha sido publicada", "Atención!");
            GetTablesByStepId();
        }
        else {
            toastr.error("Ha ocurrido un error inesperado.Por favor, contacte al soporte de Ultimus.", "Error!");
        }
    });


};

//#endregion