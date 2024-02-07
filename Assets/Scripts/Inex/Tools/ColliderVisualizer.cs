using Sirenix.OdinInspector;
using UnityEngine;

namespace Inex.Tools
{
    [AddComponentMenu("Inex/Tools/Collider Visualizer")]
    public class ColliderVisualizer : MonoBehaviour
    {
        [InfoBox("Currently this script only supports box collider")]
        [SerializeField, ColorPalette] private Color color = Color.yellow;

        private void OnDrawGizmos()
        {
            var collider = GetComponent<Collider>();
            if (collider == null)
            {
                return;
            }

            if (collider is BoxCollider)
            {
                DrawCube((BoxCollider)collider);
            }
        }

        private void DrawCube(BoxCollider boxCollider)
        {
            Gizmos.color = color;
            var size = boxCollider.size;
            var center = boxCollider.center;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(center, size);
        }
    }
}