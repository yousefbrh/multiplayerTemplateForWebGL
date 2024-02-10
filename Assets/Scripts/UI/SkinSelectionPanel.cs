using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Misc;
using UI;
using UnityEngine;

public class SkinSelectionPanel : Panel
{
    public static SkinSelectionPanel Instance;
    public Sprite buttonSelectedSprite, buttonDeselectedSprite;
    public List<SkinSelectionButton> buttons;
    int currentSelectedSkinIndex;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        var isFirstShown = Prefs.IsSkinPanelShown();
        if (isFirstShown) return;
        Show();
        Prefs.SetSkinPanelShown(true);
    }

    public override void Show()
    {
        base.Show();
        ActiveSelected();
    }

    private void ActiveSelected()
    {
        currentSelectedSkinIndex = Prefs.GetSkinIndex();
        var targetButton = buttons[currentSelectedSkinIndex];
        targetButton.SelectVisually();
    }

    private void ResetSelection()
    {
        currentSelectedSkinIndex = 0;
        foreach (SkinSelectionButton skinButton in buttons)
        {
            skinButton.DeselectVisually();
        }
    }

    public void SelectionButtonClicked(SkinSelectionButton clickedButton)
    {
        ResetSelection();
        clickedButton.SelectVisually();
        currentSelectedSkinIndex = buttons.IndexOf(clickedButton);
    }

    public void Selected()
    {
        Hide();
        Prefs.SetSkinIndex(currentSelectedSkinIndex);
    }


}
