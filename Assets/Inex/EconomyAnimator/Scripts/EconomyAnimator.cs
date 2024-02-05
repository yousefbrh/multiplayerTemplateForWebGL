using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EconomyAnimator : MonoBehaviour
{
    public static EconomyAnimator instance;
    
    public RectTransform CounterContainerIcon;
    public GameObject economyIconPrefab;
    public int numberOfEconomyIconsToGenerate;
    [Range(0, 1)] public float screenDeadZone;
    public float firstSpeed, secondSpeed;
    public AnimationCurve firstCurve, secondCurve;

    [HideInInspector] public bool loadNextLevelOnPlayed;
    
    private List<GameObject> EconomyIcons = new List<GameObject>();
    private Border border;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        float horizontalBorder = Screen.width * screenDeadZone;
        float verticalBorder = Screen.height * screenDeadZone;
        Sprite economySprite = CounterContainerIcon.GetComponent<Image>().sprite;
        Vector2 counterContainerIconScale = CounterContainerIcon.GetComponent<RectTransform>().sizeDelta;

        border = new Border(verticalBorder, verticalBorder , horizontalBorder, horizontalBorder);

        for (int i = 0; i < numberOfEconomyIconsToGenerate; i++)
        {
            GameObject icon = Instantiate(economyIconPrefab, transform);
            
            icon.GetComponent<Image>().sprite = economySprite;
            icon.GetComponent<RectTransform>().sizeDelta = counterContainerIconScale;

            if (i == numberOfEconomyIconsToGenerate - 1) icon.GetComponent<EconomyIcon>().lastItem = true;

            EconomyIcons.Add(icon);
        }
    }

    public void Play(int cashAmount, Vector2 basePosition, bool loadNextLevelOnDone = false, Action onDone = null)
    {
        SFXManager.instance.Play(11);
        loadNextLevelOnPlayed = loadNextLevelOnDone;
        int remainder = cashAmount % EconomyIcons.Count;
        for (int i = 0; i < EconomyIcons.Count; i++)
        {
            GameObject tempObject = EconomyIcons[i];
            tempObject.GetComponent<RectTransform>().position = basePosition;
            tempObject.SetActive(true);
            EconomyIcon icon = tempObject.GetComponent<EconomyIcon>();
            icon.value = cashAmount / EconomyIcons.Count;
            if (i + 1 == EconomyIcons.Count) { icon.value += remainder; }
            Vector2 firstDestination = new Vector2(Random.Range(0 + border.l, Screen.width - border.r), Random.Range(0 + border.b, Screen.height - border.t));
            icon.Move(basePosition, firstDestination, CounterContainerIcon.position, onDone);
        }
    }
}
