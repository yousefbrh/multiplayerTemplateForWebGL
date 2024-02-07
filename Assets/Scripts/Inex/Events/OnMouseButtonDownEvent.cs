using UnityEngine;
using UnityEngine.Events;

namespace Inex.Events
{
    [AddComponentMenu("Inex/Events/On Mouse Down")]
    public class OnMouseButtonDownEvent : MonoBehaviour
    {
        public UnityEvent events;
        private void OnMouseDown()
        {
            events?.Invoke();
        }
    }
}