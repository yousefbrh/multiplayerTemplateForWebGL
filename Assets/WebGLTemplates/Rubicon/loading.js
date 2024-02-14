//Lock Scaling/Zooming
document.getElementsByTagName("head")[0].innerHTML += '<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no">';
document.getElementsByTagName("head")[0].innerHTML += '<meta name="HandheldFriendly" content="true" />';

document.addEventListener("gesturestart", function (e)
{
  e.preventDefault();
});

document.addEventListener("gesturechange", function (e)
{
  e.preventDefault();
});
document.addEventListener("gestureend", function (e)
{
  e.preventDefault();
});

var container = document.querySelector("#game-container");
var canvas = document.querySelector("#game-canvas");
var loadingBar = document.querySelector("#loading-view");
var progressBarFull = document.querySelector(".loading-progress-bar");
var drawing = document.querySelector("#drawing");

var buildUrl = "Build";
var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
var config = {
dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
#if USE_WASM
codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
#endif
#if MEMORY_FILENAME
memoryUrl: buildUrl + "/{{{ MEMORY_FILENAME }}}",
#endif
#if SYMBOLS_FILENAME
symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
#endif
streamingAssetsUrl: "StreamingAssets",
companyName: {{{ JSON.stringify(COMPANY_NAME) }}},
productName: {{{ JSON.stringify(PRODUCT_NAME) }}},
productVersion: {{{ JSON.stringify(PRODUCT_VERSION) }}},
};

//Mobile
if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) 
{
var meta = document.createElement('meta');
meta.name = 'viewport';
meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
document.getElementsByTagName('head')[0].appendChild(meta);

container.className = "unity-mobile";

canvas.style.width = "100%";
canvas.style.height = "100%";

//Resize window when chage orentation
window.addEventListener("orientationchange", event =>
{
  canvas.style.width = "100%";
  canvas.style.height = "100%";
});
} 
//PC
else 
{
canvas.style.width = "100%";
canvas.style.height = "100%";
}

#if BACKGROUND_FILENAME
canvas.style.background = "url('" + buildUrl + "/{{{ BACKGROUND_FILENAME.replace(/'/g, '%27') }}}') center / cover";
#endif
loadingBar.style.display = "block";

var script = document.createElement("script");
script.src = loaderUrl;
script.onload = () => {
createUnityInstance(canvas, config, (progress) => {
  progressBarFull.style.width = 100 * progress + "%";
}).then((unityInstance) => {
  window.unityInstance = unityInstance;
  
  loadingBar.style.display = "none";
  drawing.style.display = "none";
  document.getElementById("float-overlay").style.display = "block";
}).catch((message) => {
  alert(message);
});
};
document.body.appendChild(script);