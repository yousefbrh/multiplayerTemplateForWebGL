bridge.advertisement.on('banner_state_changed', state => sendMessageToUnity('Bridge', 'BannerStateChanged', state))
bridge.advertisement.on('interstitial_state_changed', state => sendMessageToUnity('Bridge', 'InterstitialStateChanged', state))
bridge.advertisement.on('rewarded_state_changed', state => sendMessageToUnity('Bridge', 'RewardedStateChanged', state))
bridge.game.on('visibility_state_changed', state => sendMessageToUnity('Bridge', 'VisibilityStateChanged', state))

function sendMessageToUnity(objectName, name, value) {
    window.unityInstance.SendMessage(objectName, name, value)
}

function callAds(keyMessage){
    console.log("### callAds " + keyMessage);
    window.parent.postMessage({key: keyMessage});
}
window.getPlatformId = function() {
    return bridge.platform.id;
}
window.getPlatformLanguage = function() {
    return bridge.platform.language;
}
window.getDeviceType = function() {
    return bridge.device.type;
}
window.getPlayerName = function() {
    return bridge.player.name.toString();
}
window.getPlayerAvatar = function() {
    return bridge.player.photos.toString();
}
window.showInterstitial = function() {
    bridge.advertisement.showInterstitial();
}
window.showRewarded = function() {
    bridge.advertisement.showRewarded();
}
