function openInNewWd(link) {
    window.open(link);
}
function dbHighlight(id, data) {
    var data = @Html.Raw(@Newtonsoft.Json.JsonConvert.SerializeObject(Model.PlagiarismDB.DocumentIdToInitialDocumentHtml));

    if (data != null) {
        var innerHtml = data[id];
        document.getElementById("inputText").innerHTML = innerHtml;
    }
}
function webHighlight(id, data) {
    var data = @Html.Raw(@Newtonsoft.Json.JsonConvert.SerializeObject(Model.PlagiarismWeb.UrlToInitialDocumentHtml));
    if (data != null) {
        var innerHtml = data[id];
        document.getElementById("inputText").innerHTML = innerHtml;
    }
}