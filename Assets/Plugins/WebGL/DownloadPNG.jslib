mergeInto(LibraryManager.library, {
  DownloadPNG: function (dataPtr, length, filenamePtr) {
    var filename = UTF8ToString(filenamePtr);

    // Bytes aus dem WASM Heap lesen
    var bytes = new Uint8Array(Module.HEAPU8.buffer, dataPtr, length);

    // Blob erzeugen und Download triggern
    var blob = new Blob([bytes], { type: "image/png" });
    var url = URL.createObjectURL(blob);

    var a = document.createElement("a");
    a.href = url;
    a.download = filename || "screenshot.png";
    a.style.display = "none";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);

    setTimeout(function () {
      URL.revokeObjectURL(url);
    }, 100);
  }
});
