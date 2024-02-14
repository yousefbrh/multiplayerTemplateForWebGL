using System;
using Misc;
using UnityEngine;

namespace Components
{
    public class Move : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        private float _horizontal;
        private float _vertical;
        private CharacterController _characterController;
        private Transform _playerTransform;
        private float _speed;
        private bool _canMove;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerTransform = transform;
            _speed = speed;
        }

        private void Update()
        {
            CharacterMovement();
        }

        private void CharacterMovement()
        {
            if (!_canMove) return;
            _horizontal = DynamicJoystick.instance.Horizontal; 
            _vertical = DynamicJoystick.instance.Vertical;
            var gravity = Vector3.zero;
            if (!_characterController.isGrounded) gravity = Physics.gravity;
            if (_horizontal == 0) _horizontal = Input.GetAxis(Utility.HorizontalKey);
            if (_vertical == 0) _vertical = Input.GetAxis(Utility.VerticalKey);
            
            if (_horizontal == 0 && _vertical == 0)
            {
                return;
            }

            _playerTransform.eulerAngles = new Vector3(0, (Mathf.Atan2(_vertical, _horizontal) * -180f / Mathf.PI) + 90, 0);
        
            Vector3 movement = _playerTransform.forward * Time.deltaTime * _speed;
            movement += gravity;
            _characterController.Move(movement);
        }

        public void CanMove(bool isAble)
        {
            _canMove = isAble;
        }
    }
}