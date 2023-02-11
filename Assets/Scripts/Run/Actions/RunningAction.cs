using System;
using System.Collections;
using System.Linq;
using Extensions;
using Services;
using UnityEngine;

namespace DefaultNamespace.Run.Actions {
    [CreateAssetMenu(menuName = "Action/Run", order = 0, fileName = "CharacterRunAction")]
    public class RunningAction : CharacterAction {
        private const float G = 9.873f;
        private const float JUMPING_START_TIME = 0.75f;
        
        public float RunningSpeed;
        public float ChargingSpeed;
        public float LateralSpeed;
        public float Acceleration;
        public float JumpingSpeed;

        private float _jumpTime;
        private GameObject _debugCube;
        private bool _isGrounded = true;
        private static readonly int _runningSpeed = Animator.StringToHash("RunningSpeed");
        private static readonly int _grounded = Animator.StringToHash("Grounded");

        protected override IEnumerator ActionRoutine() {
            _jumpTime = Time.timeSinceLevelLoad;
            var positionService = ServiceLibrary.GetService<PositionService>();
            
            while (true) {
                var desiredSpeed = CalculateDesiredSpeed(positionService.ForwardVector.XZProjection().normalized);
                currentSpeed += (desiredSpeed - currentSpeed).WithY(0).normalized * Acceleration * Time.deltaTime;
                
                UpdateGroundedState();
                UpdateVerticalSpeed(ref currentSpeed);
                UpdateCharacterRotationAndAnimation();
                
                character.Animator.SetFloat(_runningSpeed, currentSpeed.WithY(0).magnitude);
                character.CharacterController.Move(currentSpeed * Time.deltaTime);
                positionService.ProcessNewPosition(character.Transform.position);
                yield return null;
            }
        }

        private Vector3 CalculateDesiredSpeed(Vector2 forwardVector) {
            if (Input.GetKey(KeyCode.S)) {
                return Vector3.zero;
            }
            
            var forwardSpeed = (Input.GetKey(KeyCode.W) ? ChargingSpeed : RunningSpeed) * Vector2.up;
            var sideSpeed = (Input.GetKey(KeyCode.D) ? LateralSpeed : 0) * Vector2.right + (Input.GetKey(KeyCode.A) ? LateralSpeed : 0) * Vector2.left;
            return (forwardSpeed + sideSpeed).ToXZByDirection(forwardVector);
        }

        private void UpdateGroundedState() {
            var isGrounded = IsGrounded();
            if (isGrounded != _isGrounded) {
                _isGrounded = isGrounded;
                character.Animator.SetBool(_grounded, _isGrounded);
            }
        }

        private void UpdateCharacterRotationAndAnimation() {
            var position = character.Transform.position;
            var positionDelta = (position - lastFramePosition).WithY(0);
            if (positionDelta.sqrMagnitude > 0) {
                character.Transform.forward = positionDelta;
            }
            lastFramePosition = position;
        }

        private void UpdateVerticalSpeed(ref Vector3 speed) {
            if (_isGrounded && (Time.timeSinceLevelLoad - _jumpTime) > JUMPING_START_TIME ) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    speed = speed.WithY(JumpingSpeed * Time.deltaTime);
                    _jumpTime = Time.timeSinceLevelLoad;
                    character.Animator.SetTrigger("Jump");
                } else {
                    speed = speed.WithY(-0.1f);
                }
            } else {
                speed += Vector3.down* G * Time.deltaTime;
            }
        }
        
        private bool IsGrounded() { 
            var position = character.CharacterController.center + character.CharacterController.transform.position + (character.CharacterController.height / 2 + 0.1f) * Vector3.down;
            return Physics.OverlapSphere(position, character.CharacterController.radius).Any(collider => collider.CompareTag("ground"));
        }
    }
}