using TMPro;
using UnityEngine;

namespace UI
{
    public class NetworkMessagePanel : Panel
    {
        [SerializeField] private TMP_Text panelText;

        public void ShowMessage(string text)
        {
            panelText.text = text;
            Show();
        }
    }
}