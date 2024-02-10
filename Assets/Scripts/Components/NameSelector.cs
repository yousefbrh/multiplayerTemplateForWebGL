using System;
using System.Globalization;
using Managers;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Components
{
    public class NameSelector : MonoBehaviour, IPointerClickHandler
    {
        #region SerializeField
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_Text previewNameField;
        [SerializeField] private int minNameLenght = 1;
        [SerializeField] private int maxNameLenght = 12;
        #endregion

        #region Private
        private Keyboard _keyboard;
        private bool _keyboardIsActive;
        private bool _isAuthenticated;
        #endregion

        #region Public
        public Action<string, bool> onPlayerNameChanged;
        #endregion
        
        private void Start()
        {
            if (IsServer()) return;
            SetInstance();
            SetBridgeName();
            SubscribeListeners();
            SetNameOnUI();
        }

        private bool IsServer()
        {
            return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
        }

        private void SetInstance()
        {
            _keyboard = Keyboard.Instance;
        }

        private void SetBridgeName()
        {
            var bridgeName = BridgeDataManager.Instance.GetPlayerName();
            if (bridgeName != "")
                Prefs.SetPlayerName(bridgeName);
        }

        private void SubscribeListeners()
        {
            nameField.onValidateInput += (input, charIndex, addedChar) => ValidateInput(addedChar);
            nameField.onValueChanged.AddListener(HandleNameChange);
            nameField.onSelect.AddListener(InputFieldSelected);
            nameField.onDeselect.AddListener(InputFieldDeselected);
            _keyboard.OnValueAdded += ValueChangedByKeyboard;
        }
        
        private void UnsubscribeListeners()
        {
            nameField.onValidateInput -= (input, charIndex, addedChar) => ValidateInput(addedChar);
            nameField.onValueChanged.RemoveListener(HandleNameChange);
            nameField.onSelect.RemoveListener(InputFieldSelected);
            nameField.onDeselect.RemoveListener(InputFieldDeselected);
            if (_keyboard)
                _keyboard.OnValueAdded -= ValueChangedByKeyboard;
        }

        private void SetNameOnUI()
        {
            var playerName = Prefs.GetPlayerName();
            nameField.text = playerName;
            previewNameField.text = playerName;
        }
        
        private char ValidateInput(char addedChar)
        {
            return addedChar is >= 'a' and <= 'z' || addedChar is >= 'A' and <= 'Z' || addedChar is >= '0' and <= '9' ? addedChar : '\0';
        }
        
        private void HandleNameChange(string newInput)
        {
            var text = System.Text.RegularExpressions.Regex.Replace(newInput, "[^a-zA-Z0-9 ]", string.Empty);
            UIManager.Instance.IsNameCorrect(text.Length >= minNameLenght && text.Length <= maxNameLenght);
        }
        
        private void InputFieldSelected(string arg0)
        {
            ActivateKeyboard();
        }
        
        private void InputFieldDeselected(string arg0)
        {
        }

        private void ActivateKeyboard()
        {
            if (_keyboardIsActive) return;
            _keyboardIsActive = true;
            _keyboard.m_onReturn.AddListener(SetName);
            _keyboard.Show(nameField);
        }

        private void DisableKeyboard()
        {
            _keyboardIsActive = false;
            _keyboard.m_onReturn.RemoveListener(SetName);
            _keyboard.Hide();
        }
        
        private void ValueChangedByKeyboard(char ch)
        {
            var validatedChar = ValidateInput(ch);
            nameField.text += validatedChar;
        }
        
        public void SetName()
        {
            DisableKeyboard();
            if (IsNameTheSame()) return;
            Prefs.SetPlayerName(nameField.text);
            previewNameField.text = nameField.text;
            onPlayerNameChanged?.Invoke(nameField.text, true);
        }

        private bool IsNameTheSame()
        {
            var currentName = Prefs.GetPlayerName();
            if (currentName != nameField.text) return false;
            onPlayerNameChanged?.Invoke("", false);
            return true;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerEnter == _keyboard.gameObject) return;
            if (!_keyboardIsActive) return;
            DisableKeyboard();
        }
        
        private void OnDestroy()
        {
            UnsubscribeListeners();
        }
    }
}