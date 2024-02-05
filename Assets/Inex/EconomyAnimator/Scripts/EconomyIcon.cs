using System;
using Managers;
using UnityEngine;

public class EconomyIcon : MonoBehaviour
{
    public Action OnDestinationReached;

    [HideInInspector] public int value;
    [HideInInspector] public bool lastItem;
    
    private float alpha, speed;
    private bool isMoving;
    private Vector2 basePosition, destination;
    private RectTransform rectTransform;
    private AnimationCurve alphaCurve;

    private void Awake() => rectTransform = GetComponent<RectTransform>();

    private void Update()
    {
        if (!isMoving) return;

        alpha += Time.deltaTime * speed;
        rectTransform.position = Vector2.Lerp(basePosition, destination, alphaCurve.Evaluate(alpha));
        if (alpha >= 1)
        {
            alpha = 0;
            isMoving = false;
            OnDestinationReached?.Invoke();
        }
    }

    public void Move(Vector2 BasePosition, Vector2 FirstDestination, Vector2 SecondDestnation, Action onDone)
    {
        OnDestinationReached = () =>
        {
            OnDestinationReached = () =>
            {
                EconomyManager.instance.AddGemWithoutEffect(value);
                onDone?.Invoke();
                gameObject.SetActive(false);
            };

            basePosition = rectTransform.position;
            destination = SecondDestnation;
            alphaCurve = EconomyAnimator.instance.secondCurve;
            speed = EconomyAnimator.instance.secondSpeed;
            isMoving = true;
        };

        basePosition = BasePosition;
        destination = FirstDestination;
        alphaCurve = EconomyAnimator.instance.firstCurve;
        speed = EconomyAnimator.instance.firstSpeed;
        isMoving = true;
    }
}
