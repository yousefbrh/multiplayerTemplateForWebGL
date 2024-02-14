using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Components;
using DG.Tweening;
using Entities;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{ 
    public class GameChat : NetworkBehaviour
    {
        [SerializeField] private TMP_InputField _Text;
        [SerializeField] private Transform _inputRoot;
        [SerializeField] private CanvasGroup _textAlphaTimer;
        [SerializeField] private TMP_Text _mainText;
        [SerializeField] private Button phoneImageButton;
        [SerializeField] private RectTransform panelRectTransform;
        [SerializeField] private GameObject content;
        [SerializeField] private int phoneLimit;
        [SerializeField] private int pcLimit;
        [SerializeField] private int messageLimit;
        [SerializeField] private int pcYSize;
        [SerializeField] private int pcYPosition;
        [SerializeField] private float panelYPositionOnKeyboard;

        [SerializeField] private TMP_Text _disableText;
        private List<Message> _messages = new List<Message>();
        private string playerNick;
        private Coroutine _alphaTimer;
        private Player _player;
        private bool _firstInit;
        private bool _isActive;
        private bool _phoneModeActive;
        private string _sandButtonPC;
        private string _sandButtonTap;
        private int _chosenLimit;
        private float _defaultPanelYPos;
        [DllImport("__Internal")]
        private static extern bool IsMobile();

        public static GameChat Instance;
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        
        private void Start()
        {
            if (!IsServer)
                Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (_phoneModeActive) return;
                Show();
            }
            if (IsServer)
                return;
            if (_isActive)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (_phoneModeActive) return;
                    ClientSend();
                }
            }
        }

        public void Active(bool isActive)
        {
            content.SetActive(isActive);
        }

        public void PlayerInit(Player player)
        {
            _player = player;
        }

        private void Init()
        {
            playerNick = PlayerPrefs.GetString("PlayerName");
    #if UNITY_EDITOR
            _chosenLimit = pcLimit;
            var rect = _mainText.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, pcYPosition);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, pcYSize);
            phoneImageButton.gameObject.SetActive(false);
    #else
            _chosenLimit = IsMobile() ? phoneLimit : pcLimit;
            if (!IsMobile())
            {
                var rect = _mainText.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, pcYPosition);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, pcYSize);
                phoneImageButton.gameObject.SetActive(false);
            }
            else
            {
                _disableText.gameObject.SetActive(false);
                phoneImageButton.onClick.AddListener(PreparePhoneMode);
            }
    #endif
        }

        private void PreparePhoneMode()
        {
            if (_isActive) return;
            _phoneModeActive = true;
            Keyboard.Instance.m_onReturn.AddListener(PreparePhoneModeExit);
            Keyboard.Instance.Show(_Text);
            Keyboard.Instance.OnValueAdded += CharacterAdded;
            var anchoredPosition = panelRectTransform.anchoredPosition;
            _defaultPanelYPos = anchoredPosition.y;
            anchoredPosition = new Vector2(anchoredPosition.x, panelYPositionOnKeyboard);
            panelRectTransform.anchoredPosition = anchoredPosition;
            Show();
        }

        private void CharacterAdded(char obj)
        {
            _Text.text += obj;
        }

        private void PreparePhoneModeExit()
        {
            _phoneModeActive = false;
            Keyboard.Instance.m_onReturn.RemoveListener(PreparePhoneModeExit);
            var anchoredPosition = panelRectTransform.anchoredPosition;
            anchoredPosition = new Vector2(anchoredPosition.x, _defaultPanelYPos);
            panelRectTransform.anchoredPosition = anchoredPosition;
            ClientSend();
            Keyboard.Instance.OnValueAdded -= CharacterAdded;
        }

        private void Show()
        {
            SetActive(true);
        }

        public void FinishButtonClicked()
        {
            if (_phoneModeActive)
            {
                PreparePhoneModeExit();
                Keyboard.Instance.Hide();
            }
            else
                ClientSend();
        }
        public void ClientSend()
        {
            SetActive(!_isActive);
            if (_Text.text == "") return;
            if (_Text.text.Length > messageLimit)
            {
                _Text.text = "";
                return;
            }
            SendMessageServerRpc(_Text.text, playerNick);
            _Text.text = "";
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendMessageServerRpc(string message, string playerName)
        {
            SendMessageClientRpc(message, playerName);
        }


        private void SetActive(bool active)
        {
            _player.IsChatting(active);
            _isActive = active;
            _disableText.enabled = !active;
            _inputRoot.gameObject.SetActive(active);
            if (!active) return;
            _Text.ActivateInputField();
            ActivateVisualChat();
        }

        [ClientRpc]
        public void SendMessageClientRpc(string message, string playerName)
        {
            SendMassage(message, playerName);
            ActivateVisualChat();

            _alphaTimer = StartCoroutine(Alpha());
        }

        private void ActivateVisualChat()
        {
            _textAlphaTimer.alpha = 1;
            if (_alphaTimer != null)
            {
                StopCoroutine(_alphaTimer);
                _alphaTimer = null;
            }
        }

        private void SendMassage(string message, string playerName)
        {
            _messages.Add(new Message(playerName, message));
            if (_messages.Count > 6)
                _messages.Remove(_messages.First());
            UpdateUi();
        }

        private void UpdateUi()
        {
            var count = 0;
            var index = 0;
            for (var i = _messages.Count - 1; i >= 0; i--)
            {
                count += _messages[i].MessageOwner.Length + 2;
                count += _messages[i].Text.Length;
                if (count <= _chosenLimit) continue;
                index = i + 1;
                break;
            }
            var stringBuilder = new StringBuilder();
            
            for (var i = index; i <= _messages.Count - 1; i++)
            {
                var hexColor = ColorUtility.ToHtmlStringRGBA(Color.white);
                stringBuilder.Append("<b>");
                stringBuilder.Append($"<color=#{hexColor}>{_messages[i].MessageOwner}</color>");
                stringBuilder.Append(": ");
                stringBuilder.Append("</b>");
                stringBuilder.Append(_messages[i].Text);
                stringBuilder.Append("\n");
            }
            _mainText.text = stringBuilder.ToString();
        }
        
        public class Message
        {
            public Message(string player, string message)
            {
                MessageOwner = player;
                Text = message;
            }

            public string MessageOwner;
            public string Text;
        }

        private IEnumerator Alpha()
        {
            yield return new WaitForSeconds(12);
            yield return _textAlphaTimer.DOFade(0, 2f);
        }
    }

}