using UnityEngine;

namespace Inex.Movement
{
    [AddComponentMenu("Inex/Movement/Look At Camera")]
    public class LookAtCamera : MonoBehaviour
    {
        private Camera _camera;
        private void Start()
        {
            _camera = Camera.main;
        }
        private void Update()
        {
            transform.LookAt(_camera.transform);
        }

    }
}