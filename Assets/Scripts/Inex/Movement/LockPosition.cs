using Sirenix.OdinInspector;
using UnityEngine;

namespace Inex.Movement
{
    [AddComponentMenu("Inex/Movement/Lock Position")]
    public class LockPosition : MonoBehaviour
    {
        [PropertySpace(8,15),Required]
        public Transform target;
        [BoxGroup("Options")]
        public bool followX = true;
        [BoxGroup("Options")]
        public bool followY = true;
        [BoxGroup("Options")]
        public bool followZ = true;
        [BoxGroup("Options")]
        public Vector3 offset = Vector3.zero;
        [BoxGroup("Options")]
        public bool useSmoothing = false;
        [BoxGroup("Options")] [ShowIf("useSmoothing")] [PropertyRange(0,10f)]
        public float smoothing = 0.5f;
    
    
        private void Update()
        {
            Vector3 targetPosition = target.position + offset;
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = currentPosition;
            if (followX)
            {
                newPosition.x = targetPosition.x;
            }
            if (followY)
            {
                newPosition.y = targetPosition.y;
            }
            if (followZ)
            {
                newPosition.z = targetPosition.z;
            }
            if (useSmoothing)
            {
                transform.position = Vector3.Lerp(currentPosition, newPosition, smoothing * Time.deltaTime);
            }
            else
            {
                transform.position = newPosition;
            }
        }
    }
}
