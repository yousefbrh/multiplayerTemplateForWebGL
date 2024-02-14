var landscape = {{{ LANDSCAPE }}};
var portrait = {{{ PORTRET }}};

if(bridge.platform.id != "yandex" || bridge.platform.id != "crazy_games") {
  Check();
  window.addEventListener("orientationchange", function () {
    Check();
  }, false);
}

//function lock(orientation) {
//
//  let de = document.documentElement;
//  de.focus();
//  if (de.requestFullscreen) {
//    de.requestFullscreen();
//  } else if (de.mozRequestFullScreen) {
//    de.mozRequestFullScreen();
//  } else if (de.webkitRequestFullscreen) {
//    de.webkitRequestFullscreen();
//  } else if (de.msRequestFullscreen) {
//    de.msRequestFullscreen();
//  }

  //screen.lockOrientationUniversal(orientation);
//}

function Check() {
  if (IsMobile() == false)
    return;
  console.log(screen.orientation.type);
  const orent = document.getElementById('mobile-orentation');
  const gameCanvas = document.getElementById('game-canvas');
  switch (screen.orientation.type) {
    case "landscape-secondary":
    case "landscape-primary":
      if (landscape) {
        orent.style.display = 'none';
        gameCanvas.focus();
      }
      else {
        orent.style.display = 'block';
        orent.focus();
      }
      break;
    case "portrait-secondary":
    case "portrait-primary":
      if (portrait) {
        orent.style.display = 'none';
        gameCanvas.focus();
      }
      else {
        orent.style.display = 'block';
        orent.focus();
      }
      break;
    default:
      console.log("The orientation API isn't supported in this browser :(");
  }
}

function IsMobile() {
  return (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent));
}