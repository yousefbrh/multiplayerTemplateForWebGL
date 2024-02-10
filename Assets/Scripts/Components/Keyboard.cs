using System;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Components
{
	public class Keyboard : MonoBehaviour
	{
		[SerializeField] private TMP_InputField m_inputText;

		[Space]
		[SerializeField] private GameObject m_words;
		[SerializeField] private GameObject m_nums;
		[SerializeField] private Animator keyboardAnimator;
		[SerializeField] private RectTransform _rectTransform;

		[Space]
		[SerializeField] public UnityEvent m_onReturn;
	
		[DllImport("__Internal")]
		private static extern bool IsMobile();

		private Text[] m_allTexts;
		private bool m_isUpper;
		private bool m_enable;
		private bool _isMovingUp;
		private bool _isMovingDown;
		private static readonly int Enter = Animator.StringToHash("Enter");
		private static readonly int Exit = Animator.StringToHash("Exit");

		public Action<char> OnValueAdded;

		public static Keyboard Instance;

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
		private void OnEnable()
		{
			m_allTexts = GetComponentsInChildren<Text>();
			m_enable = true;
			ToLower();
			ToABC();
			m_nums.SetActive(false);
		}

		private void Start()
		{
			foreach (Text t in m_allTexts)
			{
				Button b = t.transform.parent.GetComponent<Button>();
				b.onClick.AddListener(() => Type(t.text));
			}
		}
	
		private void ToUpper()
		{
			foreach (Text t in m_allTexts)
			{
				t.text = t.text.ToUpper();
			}
		}

		private void ToLower()
		{
			foreach (Text t in m_allTexts)
			{
				t.text = t.text.ToLower();
			}
		}

		public void Type(string word)
		{
			if (!m_enable)
				return;

			if (word == "return" || word == "RETURN")
			{
				m_onReturn?.Invoke();
				return;
			}

			if (word == ".?123" || word == "ABC" || word == "abc")
				return;
        
			if (word.Length == 1)
				OnValueAdded?.Invoke(word[0]);
		}

		public void UpperCase()
		{
			if (m_isUpper)
			{
				ToLower();
			}
			else
			{
				ToUpper();
			}

			m_isUpper = !m_isUpper;
		}

		public void Delete()
		{
			if (!m_enable)
				return;

			if (m_inputText.text.Length > 0)
				m_inputText.text = m_inputText.text.Remove(m_inputText.text.Length - 1);
		}

		public void ToABC()
		{
			m_words.SetActive(true);
			m_nums.SetActive(false);
		}

		public void ToNUM()
		{
			m_words.SetActive(false);
			m_nums.SetActive(true);
		}

		public void ColorizeWord(Color color, string word)
		{
			foreach (Text item in m_allTexts)
			{
				if (item.text.ToLower() == word.ToLower())
					item.transform.parent.GetComponent<Image>().color = color;
			}
		}

		public Color GetWordColor(string word)
		{
			foreach (Text item in m_allTexts)
			{
				if (item.text.ToLower() == word.ToLower())
					return item.transform.parent.GetComponent<Image>().color;
			}

			return Color.black;
		}

		public void Show(TMP_InputField inputField)
		{
#if !UNITY_EDITOR
			if (!IsMobile()) return;
#endif
			if (_isMovingUp) return;
			keyboardAnimator.SetTrigger(Enter);
			m_inputText = inputField;
		}

		public void Hide()
		{
			if (_isMovingDown) return;
			if (_rectTransform.rect.height != 0)
				keyboardAnimator.SetTrigger(Exit);
		}

		public void IsMovingUp()
		{
			_isMovingUp = true;
		}
	
		public void IsMovingDown()
		{
			_isMovingDown = true;
		}
	
		public void IsNotMoving()
		{
			_isMovingUp = false;
			_isMovingDown = false;
		}
	}
}