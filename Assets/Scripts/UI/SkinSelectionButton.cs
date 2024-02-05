using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectionButton : MonoBehaviour
{
    public Image bgImage;
    Button button;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(ButtonClicked);
    }

    public void SelectVisually()
    {
        bgImage.sprite = SkinSelectionPanel.Instance.buttonSelectedSprite;
    }


    public void DeselectVisually()
    {
        bgImage.sprite = SkinSelectionPanel.Instance.buttonDeselectedSprite;
    }


    void ButtonClicked()
    {
        SkinSelectionPanel.Instance.SelectionButtonClicked(this);
    }


}
