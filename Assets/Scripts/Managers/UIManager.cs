using System;
using Components;
using Misc;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private TMP_Text reportText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private NameSelector nameSelector;
        [SerializeField] private GameObject namePreviewGameObject;
        [SerializeField] private GameObject nameInputFieldGameObject;
        [SerializeField] private GameObject backgroundGameObject;
        [SerializeField] private Button setNameButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button skinButton;
        [SerializeField] private NetworkMessagePanel networkMessagePanel;
        [SerializeField] private CounterContainer counterContainer;
        #endregion

        #region Private
        private bool _isShowingAd;
        private bool _isNameCorrect;
        private bool _isAuthenticated;
        private bool _isMatchmaking;
        private float _timeInQueue;
        #endregion

        #region Public
        public static UIManager Instance;
        #endregion
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            InitButtons();
            SubscribeListeners();
        }

        private void Update()
        {
            SetCountdown();
        }

        private void InitButtons()
        {
            var hasName = Prefs.HasName();
            namePreviewGameObject.SetActive(hasName);
            nameInputFieldGameObject.SetActive(!hasName);
            setNameButton.gameObject.SetActive(!hasName);
            startButton.gameObject.SetActive(hasName);
        }

        private void SubscribeListeners()
        {
            nameSelector.onPlayerNameChanged += PlayerNameChanged;
            setNameButton.onClick.AddListener(SetNameButtonClicked);
            startButton.onClick.AddListener(StartButtonClicked);
            skinButton.onClick.AddListener(SkinButtonClicked);
            LocalAdManager.Instance.onShowAdCalled += AdCalled;
        }

        private void SetCountdown()
        {
            if (!_isMatchmaking) return;
            _timeInQueue += Time.deltaTime;
            var ts = TimeSpan.FromSeconds(_timeInQueue);
            timeText.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }

        private void PlayerNameChanged(string value, bool isChanged)
        {
            namePreviewGameObject.SetActive(true);
            nameInputFieldGameObject.SetActive(false);
            setNameButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
            if (!isChanged) return;
            GameManager.Instance.PlayerNameChanged(value);
        }
        
        private void AdCalled(bool isFinished)
        {
            _isShowingAd = !isFinished;
            ActiveButton();
        }

        private void StartButtonClicked()
        {
            GameManager.Instance.CreateGame();
        }

        private void SetNameButtonClicked()
        {
            nameSelector.SetName();
            Keyboard.Instance.Hide();
        }

        private void SkinButtonClicked()
        {
            SkinSelectionPanel.Instance.Show();
        }
        
        public void IsNameCorrect(bool isActive)
        {
            _isNameCorrect = isActive;
            ActiveButton();
        }

        private void ActiveButton()
        {
            if (!startButton) return;
            setNameButton.interactable = _isNameCorrect;
            startButton.interactable = _isAuthenticated && _isNameCorrect && !_isShowingAd;
            reportText.text = !_isAuthenticated ? "Loading..." : "";
        }

        public void Authenticated(bool isActive)
        {
            _isAuthenticated = isActive;
            ActiveButton();
        }

        public void ApplyCreateGameSetting()
        {
            reportText.text = "Searching...";
            _timeInQueue = 0;
            _isMatchmaking = true;
            startButton.interactable = false;
        }

        public void ApplyCancelingGame()
        {
            startButton.interactable = true;
            _isMatchmaking = false;
            buttonText.text = "Find Match";
            reportText.text = string.Empty;
            timeText.text = string.Empty;
        }

        public void ApplyGameRespond(bool isSuccessful)
        {
            reportText.text = isSuccessful ? "Connecting..." : "Please Try Again";
        }

        public void ShowEntryPanel(bool isActive)
        {
            backgroundGameObject.SetActive(isActive);
        }

        public void ShowMessageOnNetworkMessagePanel(string message)
        {
            networkMessagePanel.ShowMessage(message);
        }

        public void AddCoinWithoutEffect(int value)
        {
            counterContainer.AddWithoutEffect(value);
        }
    }
}