mergeInto(LibraryManager.library, {

    InstantGamesBridgeGetPlatformId: function() {
        var platformId = window.getPlatformId()
        var bufferSize = lengthBytesUTF8(platformId) + 1
        var buffer = _malloc(bufferSize)
        stringToUTF8(platformId, buffer, bufferSize)
        return buffer
    },

    InstantGamesBridgeGetPlatformLanguage: function() {
        var platformLanguage = window.getPlatformLanguage()
        var bufferSize = lengthBytesUTF8(platformLanguage) + 1
        var buffer = _malloc(bufferSize)
        stringToUTF8(platformLanguage, buffer, bufferSize)
        return buffer
    },

    InstantGamesBridgeGetDeviceType: function() {
        var deviceType = window.getDeviceType()
        var bufferSize = lengthBytesUTF8(deviceType) + 1
        var buffer = _malloc(bufferSize)
        stringToUTF8(deviceType, buffer, bufferSize)
        return buffer
    },

    InstantGamesBridgePlayerName: function() {
        var playerName = window.getPlayerName()
        var bufferSize = lengthBytesUTF8(playerName) + 1
        var buffer = _malloc(bufferSize)
        stringToUTF8(playerName, buffer, bufferSize)
        return buffer
    },
    
    InstantGamesBridgePlayerAvatar: function() {
            var playerName = window.getPlayerAvatar()
            var bufferSize = lengthBytesUTF8(playerName) + 1
            var buffer = _malloc(bufferSize)
            stringToUTF8(playerName, buffer, bufferSize)
            return buffer
        },

    InstantGamesBridgeShowRewarded: function() {
        console.log("### lib InstantGamesBridgeShowRewarded");
        window.showRewarded()
    },

    InstantGamesBridgeShowInterstitial: function() {
        console.log("### lib InstantGamesBridgeShowInterstitial");
        window.showInterstitial()
    }
});