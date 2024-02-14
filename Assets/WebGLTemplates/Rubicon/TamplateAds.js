window.addEventListener('message', function(event) {
    switch (event.data.key) {
        case 'GetPlatformId':
            sendMessageToUnity('Bridge', 'OnGetPlatformId', event.data.value)
            break;
        case 'GetPlatformLanguage':
            sendMessageToUnity('Bridge', 'OnGetPlatformLanguage', event.data.value)
            break;
        case 'GetDeviceType':
            sendMessageToUnity('Bridge', 'OnGetDeviceType', event.data.value)
            break;
        case 'GetPlayerName':
            sendMessageToUnity('Bridge', 'OnGetPlayerName', event.data.value)
            break;
        case 'InterstitialStateChanged':
            sendMessageToUnity('Bridge', 'OnInterstitialStateChanged', event.data.value)
            break;
        case 'RewardedStateChanged':
            sendMessageToUnity('Bridge', 'OnRewardedStateChanged', event.data.value)
            break;
        case 'VisibilityStateChanged':
            sendMessageToUnity('Bridge', 'OnVisibilityStateChanged', event.data.value)
            break;
        case 'BannerStateChanged':
            sendMessageToUnity('Bridge', 'OnVisibiliOnBannerStateChangedtyStateChanged', event.data.value)
            break;
    }
});

function sendMessageToUnity(objectName, name, value) {
    console.log("### " + name);
    window.unityInstance.SendMessage(objectName, name, value)
}

function callAds(keyMessage){
    console.log("### callAds " + keyMessage);
    window.parent.postMessage({key: keyMessage});
}
window.getPlatformId = function() {
    callAds('ad_PageLoaded');
}
window.getPlatformLanguage = function() {
    callAds('ad_GetPlatformLanguage');
}
window.getDeviceType = function() {
    callAds('ad_GetDeviceType');
}
window.getPlayerName = function() {
    callAds('ad_GetPlayerName');
}
window.showInterstitial = function() {
    callAds('ad_ShowInterstitial');
}
window.showRewarded = function() {
    callAds('ad_ShowRewarded');
}

