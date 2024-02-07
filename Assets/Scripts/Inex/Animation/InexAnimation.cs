using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Inex.Animation
{
    [Serializable]
    public class AnimationItem
    {
        // animation properties
        public AnimationType type;
        public float duration = 1f;
        [PropertySpace(8, 0)] public bool hasDelay = false;
        [ShowIf("hasDelay")] public float delay = 0f;

        public bool setEase = false;
        [ShowIf("setEase")] public Ease ease = Ease.Linear;

        [PropertySpace(0, 10)] public bool playInParallelWithPrevious = false;

        // move
        [ShowIf("type", Value = AnimationType.Move), BoxGroup("Move")]
        public Vector3 moveTo;

        [ShowIf("type", Value = AnimationType.Move), BoxGroup("Move")]
        public bool isRelative = true;

        // scale
        [ShowIf("type", Value = AnimationType.Scale), BoxGroup("Scale")]
        public bool uniformScale = true;

        // [ShowIf("type", Value = AnimationType.Scale &&),ShowIf("uniformScale"), BoxGroup("Scale")]
        [ShowIf("@this.uniformScale && type == AnimationType.Scale"), BoxGroup("Scale")]
        public float scaleFactor = 1f;

        [ShowIf("@this.uniformScale == false && type == AnimationType.Scale"), BoxGroup("Scale")]
        public Vector3 scaleTo = Vector3.one;

        // rotate
        [ShowIf("type", Value = AnimationType.Rotate), BoxGroup("Rotate")]
        public Vector3 rotateTo;

        [ShowIf("type", Value = AnimationType.Rotate), BoxGroup("Rotate")]
        public bool isRelativeRotation = true;

        // shake
        [ShowIf(
             "@type == AnimationType.ShakePosition || type == AnimationType.ShakeRotation || type == AnimationType.ShakeScale"),
         BoxGroup("Shake")]
        public float shakeStrength = 5f;

        [ShowIf(
             "@type == AnimationType.ShakePosition || type == AnimationType.ShakeRotation || type == AnimationType.ShakeScale"),
         BoxGroup("Shake")]
        public int shakeVibrato = 10;

        [ShowIf(
             "@type == AnimationType.ShakePosition || type == AnimationType.ShakeRotation || type == AnimationType.ShakeScale"),
         BoxGroup("Shake")]
        public float shakeRandomness = 90f;
    }

    public enum AnimationType
    {
        Move,
        Rotate,
        Scale,
        ShakePosition,
        ShakeRotation,
        ShakeScale,
    }

    [AddComponentMenu("Inex/Animation/Inex Animation")]
    public class InexAnimation : MonoBehaviour
    {
        public bool loop = false;
        public bool initialProperties = false;

        [FormerlySerializedAs("InitialScale")] [ShowIf("initialProperties")]
        public Vector3 initialScale = Vector3.zero;

        [SerializeField] private AnimationItem[] timeline;

        Sequence sequence;

        private void OnEnable()
        {
            ApplyInitialProperties();
            sequence = DOTween.Sequence();

            for (int i = 0; i < timeline.Length; i++)
            {
                var item = timeline[i];
                var tween = GetTween(item);
                if (item.playInParallelWithPrevious)
                {
                    sequence.Join(tween);
                }
                else
                {
                    sequence.Append(tween);
                }
            }

            if (loop) sequence.SetLoops(-1);

            sequence.Play();
        }

        private void OnDisable()
        {
            sequence.Kill();
        }

        private void OnDestroy()
        {
            sequence.Kill();
        }


        private Tween GetTween(AnimationItem item)
        {
            Tween tween;

            switch (item.type)
            {
                case AnimationType.Move:
                    tween = transform.DOMove(item.moveTo, item.duration).SetRelative(item.isRelative);
                    break;
                case AnimationType.Scale when item.uniformScale:
                    tween = transform.DOScale(item.scaleFactor, item.duration);
                    break;
                case AnimationType.Scale:
                    tween = transform.DOScale(item.scaleTo, item.duration);
                    break;
                case AnimationType.Rotate:
                    tween = transform.DORotate(item.rotateTo, item.duration).SetRelative(item.isRelativeRotation);
                    break;
                case AnimationType.ShakePosition:
                    tween = transform.DOShakePosition(item.duration, item.shakeStrength, item.shakeVibrato,
                        item.shakeRandomness);
                    break;
                case AnimationType.ShakeRotation:
                    tween = transform.DOShakeRotation(item.duration, item.shakeStrength, item.shakeVibrato,
                        item.shakeRandomness);
                    break;
                case AnimationType.ShakeScale:
                    tween = transform.DOShakeScale(item.duration, item.shakeStrength, item.shakeVibrato,
                        item.shakeRandomness);
                    break;
                default:
                    tween = transform.DOScale(1, 1f); // this should never happen
                    break;
            }


            if (item.setEase)
                tween.SetEase(item.ease);

            if (item.hasDelay)
                tween.SetDelay(item.delay);

            return tween;
        }


        // apply initial properties
        private void ApplyInitialProperties()
        {
            if (initialProperties)
            {
                transform.localScale = initialScale;
            }
        }
    }
}