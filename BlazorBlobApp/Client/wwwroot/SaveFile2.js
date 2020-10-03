function FileSaveAs2(filename) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream," + "api/containers/" + filename;
    link.target = "_blank";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}