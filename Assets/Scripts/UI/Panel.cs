using UnityEngine;

namespace UI
{
    public class Panel : MonoBehaviour
    {
        public GameObject content;
        
        public virtual void Show()
        {
            content.SetActive(true);
        }

        public virtual void Hide()
        {
            content.SetActive(false);
        }
    }
}