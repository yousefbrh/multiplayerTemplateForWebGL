using UnityEngine;

namespace Inex.Common.Destroy
{
    [AddComponentMenu("Inex/Common/Destroy/Destroy With Delay")]
    public class DestroyWithDelay : MonoBehaviour
    {
        [SerializeField] private float delay = 10f;

        private void Start()
        {
            Destroy(gameObject, delay);
        }
    }
}
