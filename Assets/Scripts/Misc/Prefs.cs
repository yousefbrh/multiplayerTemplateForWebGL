using Managers;
using UnityEngine;

namespace Misc
{
    public static class Prefs
    {
        private const string PlayerNameKey = "PlayerName";
        private const string MusicKey = "MusicEnabled";
        private const string SfxKey = "SoundEffectsEnabled";
        private const string FirstShowPanelKey = "isFirstShownCharacterPanel";
        private const string SkinIndexKey = "CurrentSkinIndex";
        
        public static bool HasName()
        {
            var bridgeName = BridgeDataManager.Instance.GetPlayerName();
            var hasName = bridgeName != "";
            if (!hasName)
                hasName = PlayerPrefs.HasKey("PlayerName");
            return hasName;
        }

        public static string GetPlayerName(string defaultValue = "")
        {
            return PlayerPrefs.GetString("PlayerName", defaultValue);
        }

        public static void SetPlayerName(string value)
        {
            PlayerPrefs.SetString(PlayerNameKey, value);
        }

        public static bool IsMusicEnable(int defaultValue = 1)
        {
            return PlayerPrefs.GetInt(MusicKey, defaultValue) == 1;
        }

        public static void SetMusicActive(bool value)
        {
            PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
        }

        public static bool IsSfxEnable(int defaultValue = 1)
        {
            return PlayerPrefs.GetInt(SfxKey, defaultValue) == 1;
        }
        
        public static void SetSfxActive(bool value)
        {
            PlayerPrefs.SetInt(SfxKey, value ? 1 : 0);
        }

        public static int GetPoints(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt("Point", defaultValue);
        }
        
        public static void SetPoints(int value)
        { 
            PlayerPrefs.SetInt("Point", value);
        }

        public static bool IsSkinPanelShown(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(FirstShowPanelKey, defaultValue) == 1;
        }
        
        public static void SetSkinPanelShown(bool value)
        {
            PlayerPrefs.SetInt(FirstShowPanelKey, value ? 1 : 0);
        }

        public static int GetSkinIndex(int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(SkinIndexKey, defaultValue);
        }
        
        public static void SetSkinIndex(int value)
        {
            PlayerPrefs.SetInt(SkinIndexKey, value);
        }
    }
}