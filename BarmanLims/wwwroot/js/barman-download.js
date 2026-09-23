window.BlazorDownloadFile = function (fileName, contentType, byteArray) {
    const blob = new Blob([new Uint8Array(byteArray)], {
        type: contentType
    });

    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");

    anchor.href = url;
    anchor.download = fileName;
    anchor.style.display = "none";

    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);

    URL.revokeObjectURL(url);
};
window.BarmanOpenFilePicker = function (inputElement) {
    if (inputElement) {
        inputElement.click();
    }
};