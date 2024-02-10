using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ads
{
    public class InterDelayer : MonoBehaviour
    {
        [Header("Images")] [SerializeField] private Image _breakTimeImage;
        [SerializeField] private Image _tapToContinue;
        [Header("Basic")] [SerializeField] private Text _timeText;
        [SerializeField] private Image _visualizeTimer;

        private float _localTimer;
        private bool _isActive;

        public void Init()
        {
            _breakTimeImage.gameObject.SetActive(false);
            AdsManager.instance.InterFailed += BreakEnd;
            AdsManager.instance.InterSuccess += BreakEnd;
            _isActive = false;
        }

        public void StartInter(Action action, float timeWaiting)
        {
            StopAllCoroutines();
            StartCoroutine(BreakTime(action, timeWaiting));
        }

        public void ResumeGame()
        {
            AudioListener.pause = false;
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Break time end
        /// </summary>
        private void BreakEnd()
        {
            if (_isActive == false)
                return;
            _isActive = false;
            _breakTimeImage.gameObject.SetActive(false);
            _tapToContinue.gameObject.SetActive(true);
        }

        /// <summary>
        /// Break time starting
        /// </summary>
        private IEnumerator BreakTime(Action action, float timeWaiting)
        {
            _isActive = true;
            _timeText.gameObject.SetActive(true);
            _breakTimeImage.gameObject.SetActive(true);
            StartCoroutine(AnimatedBreakImageTimer(_visualizeTimer, 1f, timeWaiting));
            _localTimer = timeWaiting;
            while (_localTimer >= 0f) //Timer before starting ads
            {
                string timer = _localTimer == 0 ? $"START" : $"{_localTimer}";
                _timeText.text = timer;

                yield return new WaitForSeconds(1f);

                _localTimer--;
            }

            action?.Invoke();
        }

        /// <summary>
        /// Tween animate images with amount fill 
        /// </summary>
        /// <param name="targetFill"></param>
        /// <param name="total"></param>
        /// <param name="duration"></param>
        private IEnumerator AnimatedBreakImageTimer(Image targetFill, float total, float duration)
        {
            targetFill.fillAmount = 0;
            while (targetFill.fillAmount < total)
            {
                yield return new WaitForFixedUpdate();
                targetFill.fillAmount += Time.fixedDeltaTime / duration;
            }
        }
    }
}