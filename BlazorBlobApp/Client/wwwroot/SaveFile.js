function FileSaveAs2() {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:text/plain;charset=utf-8," + encodeURIComponent(fileContent)
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

}

function FileSaveAs(filename, fileContent) {
    var link = document.createElement('a');
    link.href = "api/containers/あ.xlsx";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

