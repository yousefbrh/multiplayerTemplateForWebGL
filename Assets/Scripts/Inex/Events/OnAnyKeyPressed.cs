using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Inex.Events
{
    [InfoBox("this Component will call the event when any key is pressed")]
    [AddComponentMenu("Inex/Events/On Any Key Pressed")]
    public class OnAnyKeyPressed : MonoBehaviour
    {
        public UnityEvent events;

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                events?.Invoke();
            }
        }
    }
}