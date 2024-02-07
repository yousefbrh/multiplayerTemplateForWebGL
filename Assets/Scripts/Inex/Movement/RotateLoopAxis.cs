using DG.Tweening;
using Inex.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inex.Movement
{
    [AddComponentMenu("Inex/Movement/Rotate Loop axis")]
    public class RotateLoopAxis : MonoBehaviour
    {
        [SerializeField, EnumToggleButtons] private VectorType axis;

        [SerializeField, PropertyRange(0.1f, 50f)]
        private float duration = 1;

        private void Start()
        {
            // rotate with dotween loop
            if (axis == VectorType.X)
            {
                transform.DOLocalRotate(new Vector3(360, 0, 0), duration, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            }
            else if (axis == VectorType.Y)
            {
                transform.DOLocalRotate(new Vector3(0, 360, 0), duration, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            }
            else if (axis == VectorType.Z)
            {
                transform.DOLocalRotate(new Vector3(0, 0, 360), duration, RotateMode.LocalAxisAdd)
                    .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            }
        }
    }
}