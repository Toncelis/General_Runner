using System.Collections;
using Extensions;
using Services;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace.Run.Actions {
    [CreateAssetMenu(menuName = "Action/Run", order = 0, fileName = "CharacterRunAction")]
    public class RunningAction : CharacterAction {
        public float G = 9.873f;
        private const float JUMPING_START_TIME = 0.1f;

        public float RunningSpeed;
        public float ChargingSpeed;
        public float LateralSpeed;
        public float Acceleration;
        public float JumpHeight;
        private float JumpSpeed => Mathf.Sqrt(2 * JumpHeight * G);

        private float _jumpTime;
        private GameObject _debugCube;
        private bool _isGrounded = true;
        private static readonly int _runningSpeed = Animator.StringToHash("RunningSpeed");
        private static readonly int _grounded = Animator.StringToHash("Grounded");

        private RaycastHit groundHit;
        private PositionService PositionService => ServiceLibrary.GetService<PositionService>();

        protected override IEnumerator ActionRoutine() {
            _jumpTime = Time.timeSinceLevelLoad;

            while (true) {
                var desiredSpeed = CalculateDesiredSpeed(PositionService.ForwardVector.XZProjection().normalized);
                
                UpdateCharacterRotationAndAnimation();
                UpdateGroundedState();

                if (_isGrounded && !jumping) {
                    if (Input.GetKeyDown(KeyCode.Space)) {
                        currentSpeed = desiredSpeed.WithY(JumpSpeed);
                        character.Animator.SetTrigger("Jump");
                        _jumpTime = Time.timeSinceLevelLoad;
                    } else {    
                        currentSpeed = Quaternion.FromToRotation(PositionService.ForwardVector.WithY(0), mainMovementDirection) * desiredSpeed - groundNormal * 2;
                    }
                } else {
                    currentSpeed = desiredSpeed.WithY(currentSpeed.y - G * Time.deltaTime);
                }

                Debug.Log($"current speed : {currentSpeed}");

                character.Animator.SetFloat(_runningSpeed, desiredSpeed.magnitude);
                character.CharacterController.Move(currentSpeed * Time.deltaTime);
                PositionService.ProcessNewPosition(character.Transform.position);
                
                character.currentSpeedArrow.forward = currentSpeed;
                character.currentSpeedArrow.localScale = currentSpeed.magnitude * Vector3.one;
                character.normalArrow.forward = groundNormal;
                character.forwardArrow.forward = mainMovementDirection;
                
                yield return null;
            }
        }

        private Vector3 CalculateDesiredSpeed(Vector2 forwardVector) {
            var forwardSpeed = (Input.GetKey(KeyCode.W) ? 4 : 2 + (Input.GetKey(KeyCode.S) ? -1 : 0)) * Vector2.up;
            var sideSpeed = (Input.GetKey(KeyCode.D) ? 2 : 0) * Vector2.right + (Input.GetKey(KeyCode.A) ? 2 : 0) * Vector2.left;
            var speed = (forwardSpeed + sideSpeed).normalized * RunningSpeed;
            return speed.ToXZByDirection(forwardVector);
        }

        private void UpdateGroundedState() {
            var isGrounded = IsGrounded();
            Debug.Log($"is grounded : {isGrounded}");
            if (isGrounded != _isGrounded) {
                if (isGrounded) {
                    currentSpeed = currentSpeed.WithY(Mathf.Max(-0.1f, currentSpeed.y));
                }
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

            //      currentSpeed = (position - lastFramePosition)/Time.deltaTime;
            lastFramePosition = position;
        }

        private bool IsGrounded() {
            var position = character.CharacterController.center + character.CharacterController.transform.position;
            if (!Physics.SphereCast(position, character.CharacterController.radius-0.01f, Vector3.down, out groundHit, character.CharacterController.height / 2 + 0.1f, 1<<LayerMask.NameToLayer("Default"))) {
                return false;
            };
            Debug.Log($"ground is {groundHit.transform.name}", groundHit.transform);

            if (!groundHit.collider.CompareTag("ground")) {
                return false;
            }
            return Vector3.Project(currentSpeed, groundNormal).normalized != groundNormal;
        }

        private Vector3 groundNormal => _isGrounded ? groundHit.GetCorrectNormalForSphere(Vector3.down) : Vector3.up;
        private Vector3 mainMovementDirection => Vector3.ProjectOnPlane(PositionService.ForwardVector, groundNormal).normalized;

        private bool jumping => Time.timeSinceLevelLoad - _jumpTime < JUMPING_START_TIME;
    }
}