window.blazor_chart = {
  scrollToBottom: function (ref) {
    ref.scrollTop = ref.scrollHeight;
  },
  blurAndFocus: function (ref) {
    ref.value = "";
    ref.blur();
    ref.focus();
  },
  setupfileInput: function (fileRef, uploadUrl, objRef) {
    fileRef.onchange = function () {
      if (fileRef.files.length <= 0)
        return;

      var file = fileRef.files[0];
      var form = new FormData();
      form.append("file", file);
      var xhr = new XMLHttpRequest();
      xhr.open("POST", uploadUrl);
      xhr.send(form);
      xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
          var resultObj = JSON.parse(xhr.responseText);
          objRef.invokeMethodAsync("FileUploadCompleted", resultObj.filePath).then();
        }
      }
    }
  }
}