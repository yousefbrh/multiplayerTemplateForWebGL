const elem = document.getElementById("fullscreen");

if (/iPhone|iPad|iPod/i.test(navigator.userAgent)) {
  elem.style.display = "none";
}
setFullScreenImage();
document.addEventListener('fullscreenchange', function () {
  setFullScreenImage();
});

elem.onclick = function () {
  let de = document.documentElement;
  if (!document.fullscreenElement) {
    if (de.requestFullscreen) {
      de.requestFullscreen();
    } else if (de.mozRequestFullScreen) {
      de.mozRequestFullScreen();
    } else if (de.webkitRequestFullscreen) {
      de.webkitRequestFullscreen();
    } else if (de.msRequestFullscreen) {
      de.msRequestFullscreen();
    }
  } else {
    document.exitFullscreen();
  }
};

function IsFullScreen() {
  return !!(document.fullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement || document.msFullscreenElement)
}

function setFullScreenImage() {
  if (IsFullScreen()) {
    elem.src = "TemplateData/FloatWindow/data/roll_up.svg";
  } else {
    elem.src = "TemplateData/FloatWindow/data/unwrap.svg";
  }
}

function toggleFullScreenMode(event) {
  if (document.fullscreenElement == null) {
    document.documentElement.requestFullscreen();
  } else if (document.exitFullscreen) {
    document.exitFullscreen();
  }
}

