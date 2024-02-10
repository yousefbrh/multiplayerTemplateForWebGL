using System;
using UnityEngine;

namespace Rubicon.Ads
{
    public class BridgeGame : MonoBehaviour
    {
        public event Action GamePause, GameRun;
        private float _timeScale = 1;

        private bool _isAdPause = false;
        public void AdsOpen()
        {
            _isAdPause = true;
            Pause();
        }
        
        public void AdsClose()
        {
            _isAdPause = false;
            Play();
        }
        public void VisibilityStateChanged(string value)
        {
            if(_isAdPause)
                return;
            
            if (value == "visible")
                Play();
            else // "hidden"
                Pause();

            print("### BridgeGame.VisibilityStateChanged " + value);
        }
        
        private void Pause()
        {
            _timeScale = Time.timeScale;
            AudioListener.pause = true;
            Time.timeScale = 0;
            GamePause?.Invoke();
        }

        private void Play()
        {
            AudioListener.pause = false;
            Time.timeScale = _timeScale;
            GameRun?.Invoke();
        }
    }
}