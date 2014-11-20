Ultimus.UI = function () {

    this.AddSelectOption = function (_selectId, _optionId, _text) {

        listItems = "<option value='" + _optionId + "'>" + _text + "</option>";
        $(_selectId).append(listItems);
    };

    this.CleanSelectControl = function (_selectId) {

        $(_selectId).empty();

        listItems = "<option value='-1'>SELECCIONE UNA OPCIÓN</option>";
        $(_selectId).append(listItems);
    };

    this.InsertContent = function (stringHtml) {

        $("#content").empty();
        $('#content').append(stringHtml);

    };

    this.showAlertInfo = function (messageText) {

        $("#messageContainer").empty();

        stringHtml = "<div class='alert alert-info alert-dismissable'>";
        stringHtml += "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>";
        stringHtml += "<i class='fa fa-info-circle'></i>&nbsp;";
        stringHtml += "<strong>"; 
        stringHtml += messageText;
        stringHtml += "</strong>";
        stringHtml += "</div>";

        $('#messageContainer').append(stringHtml);

    };

    this.Createlabel = function (text, type) {

        var stringHtml = "";
        stringHtml += "<span class=\"label label-" + type + "\">";
        stringHtml += text;
        stringHtml += "</span>";

        return stringHtml;

    };

    this.CreateBadge = function (text) {

        var stringHtml = "";
        stringHtml += "<span class=\"badge\">";
        stringHtml += text;
        stringHtml += "</span>";

        return stringHtml;
    };

    this.ENDREQUEST = function (text) {
        $("#loadingGif").hide();
    };


   
};

