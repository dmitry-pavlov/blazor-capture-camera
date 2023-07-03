export function start(videoElem, canvasElem, dotnetObject, onFrameHandlerName) {

  if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
    navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
      const video = document.getElementById(videoElem.id);
      if ("srcObject" in video) {
        video.srcObject = stream;
      } else {
        video.src = window.URL.createObjectURL(stream);
      }

      video.onloadedmetadata = function (e) {
        video.play();
        const canvas = document.getElementById(canvasElem.id);
        video.ontimeupdate = (e) => {
          canvas.getContext("2d").drawImage(video, 0, 0);
          const data = canvas.toDataURL("image/png");
          dotnetObject.invokeMethodAsync(onFrameHandlerName, data);
        };
      };
    });
  }
}

export function stop(videoElem) {
  const video = document.getElementById(videoElem.id);
  if ("srcObject" in video) {
    video.srcObject.getTracks().forEach(t => t.stop());
  }

  if ("ontimeupdate" in video) {
    video.ontimeupdate = null;
    video.pause();
  }
}