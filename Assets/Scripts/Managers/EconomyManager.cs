using System;
using Misc;
using UnityEngine;

namespace Managers
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager instance;

        private void Awake() 
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UIManager.Instance.AddCoinWithoutEffect(Prefs.GetPoints());
        }

        public void AddGemWithoutEffect(int amount)
        {
            UIManager.Instance.AddCoinWithoutEffect(amount);
        }
    }
}