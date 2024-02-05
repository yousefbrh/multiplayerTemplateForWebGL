using Misc;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CounterContainer : MonoBehaviour
    {
        [SerializeField] private Animator iconAnimator;
        [SerializeField] private TMP_Text counterTMP;

        [HideInInspector] public int total;

        public void AddWithoutEffect(int amount)
        {
            total += amount;
            counterTMP.text = $"{total}";
        }

        public void RemoveWithoutEffect(int amount)
        {
            total -= amount;
            counterTMP.text = $"{total}";
        }
    
        public void RemoveAllWithoutEffect()
        {
            RemoveWithoutEffect(total);
        }

        public void Add(int amount)
        {
            AddWithoutEffect(amount);
            if (iconAnimator) iconAnimator.SetTrigger(Utility.AddHashKey);
        }

        public void Remove(int amount)
        {
            RemoveWithoutEffect(amount);
            if (iconAnimator) iconAnimator.SetTrigger(Utility.RemoveHashKey);
        }

        public int RemoveAll()
        {
            Remove(total);
            return 0;
        }
    }
}