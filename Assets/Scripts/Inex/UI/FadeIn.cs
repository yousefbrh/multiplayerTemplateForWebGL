using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Inex.UI
{
    [AddComponentMenu("Inex/UI/Fade In")]
    public class FadeIn : MonoBehaviour
    {
        [SerializeField, PropertyRange(0, 5f)] float duration = 0.5f;
        [SerializeField] bool modifyAlpha = true;

        [SerializeField, ShowIf("modifyAlpha")]
        float initialAlpha = 0;

        public bool delay = false;

        [PropertyRange(0, 10), ShowIf("delay")]
        public float delayTime = 3f;

        private Graphic _graphic;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
        }

        private void OnEnable()
        {
            if (modifyAlpha)
            {
                _graphic.color = new Color(_graphic.color.r, _graphic.color.g, _graphic.color.b, initialAlpha);
            }
            if (delay)
            {
                this.DelayedCall(Animate, delayTime);
                this.DelayedCall(Animate, delayTime);
            }
            else
            {
                Animate();
            }
        }

        private void Animate()
        {
            if (modifyAlpha)
            {
                _graphic.color = new Color(_graphic.color.r, _graphic.color.g, _graphic.color.b, initialAlpha);
            }

            _graphic.DOFade(1, duration);
        
        }
    }
}