using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Inex.Events
{
    [InfoBox("this Component is used to detect clicks anywhere on the screen and then call a function")]
    [AddComponentMenu("Inex/Events/On Click Anywhere")]
    public class OnClickAnywhere : MonoBehaviour
    {
        public UnityEvent events;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                events?.Invoke();
            }
        }
    }
}