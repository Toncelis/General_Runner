using System.Collections;
using Extensions;
using Services;
using UnityEngine;

namespace DefaultNamespace.Run.Actions {
    [CreateAssetMenu(menuName = "Action/Run", order = 0, fileName = "CharacterRunAction")]
    public class RunningAction : CharacterAction {
        public float G = 9.873f;
        private const float JUMPING_START_TIME = 0.1f;
        private const float TIME_STEP = 0.01f;

        public float RunningSpeed;
        public float Acceleration;
        public float JumpHeight;
        private float JumpSpeed => Mathf.Sqrt(2 * JumpHeight * G);
        private Vector3 _effectiveSpeed = Vector3.zero;
        
        private float _jumpTime;
        private GameObject _debugCube;
        private bool _isGrounded = true;
        private static readonly int _runningSpeed = Animator.StringToHash("RunningSpeed");
        private static readonly int _grounded = Animator.StringToHash("Grounded");

        private RaycastHit groundHit;
        private PositionService PositionService => ServiceLibrary.GetService<PositionService>();

        protected override IEnumerator ActionRoutine() {
            lastFramePosition = character.Transform.position;
            _jumpTime = Time.timeSinceLevelLoad-JUMPING_START_TIME;
            
            while (true) {
                var desiredSpeed = CalculateDesiredSpeed(PositionService.ForwardVector.XZProjection().normalized);
                
                UpdateCharacterRotationAndAnimation();
                UpdateGroundedState();
                UpdateSpeed(desiredSpeed);

                character.CharacterController.Move(currentSpeed * TIME_STEP);
                PositionService.ProcessNewPosition(character.Transform.position);
                
                character.currentSpeedArrow.forward = currentSpeed;
                character.currentSpeedArrow.localScale = currentSpeed.magnitude * Vector3.one;
                character.normalArrow.forward = groundNormal;
                character.forwardArrow.forward = mainMovementDirection;

                yield return new WaitForSeconds(TIME_STEP);
            }
        }

        private Vector3 CalculateDesiredSpeed(Vector2 forwardVector) {
            var forwardSpeed = (Input.GetKey(KeyCode.W) ? 4 : 2 + (Input.GetKey(KeyCode.S) ? -1 : 0)) * Vector2.up;
            var sideSpeed = (Input.GetKey(KeyCode.D) ? 2 : 0) * Vector2.right + (Input.GetKey(KeyCode.A) ? 2 : 0) * Vector2.left;
            var speed = (forwardSpeed + sideSpeed).normalized * RunningSpeed;
            return speed.ToXZByDirection(forwardVector);
        }

        private void UpdateSpeed(Vector3 desiredSpeed) {
            Vector3 tangentSpeed = Vector3.zero;
            Vector3 normalSpeed = Vector3.zero;
            
            if (_isGrounded && !jumping) {
                if (Input.GetKey(KeyCode.Space)) {
                    tangentSpeed = desiredSpeed.WithY(0);
                    normalSpeed = JumpSpeed * Vector3.up;
                    character.Animator.SetTrigger("Jump");
                    _jumpTime = Time.timeSinceLevelLoad;
                } else {
                    tangentSpeed = Quaternion.FromToRotation(PositionService.ForwardVector.WithY(0), mainMovementDirection) * desiredSpeed;
                    normalSpeed = -groundNormal * 2;
                }
            } else {
                tangentSpeed = desiredSpeed.WithY(0);
                normalSpeed = (currentSpeed.y - G * TIME_STEP) * Vector3.up;
            }

            Vector3 previousTangentSpeed = normalSpeed == Vector3.zero ?  Vector3.ProjectOnPlane(currentSpeed, Vector3.up) : Vector3.ProjectOnPlane(currentSpeed, normalSpeed);
            character.Animator.SetFloat(_runningSpeed, previousTangentSpeed.magnitude);
            Vector3 desiredDeltaTangentSpeed = tangentSpeed - previousTangentSpeed;
            
            if (desiredDeltaTangentSpeed.magnitude <= Acceleration * TIME_STEP) {
                currentSpeed = tangentSpeed + normalSpeed;
            } else {
                currentSpeed = previousTangentSpeed + normalSpeed + desiredDeltaTangentSpeed.normalized * Acceleration * TIME_STEP;
            }
            
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

            _effectiveSpeed = (position - lastFramePosition) / TIME_STEP;
            lastFramePosition = position;
        }

        private bool IsGrounded() {
            var position = character.CharacterController.center + character.CharacterController.transform.position;
            if (!Physics.SphereCast(position, character.CharacterController.radius-0.01f, Vector3.down, out groundHit, character.CharacterController.height / 2 + 0.1f, 1<<LayerMask.NameToLayer("Default"))) {
                return false;
            };

            if (!groundHit.collider.CompareTag("ground")) {
                return false;
            }

            if (groundNormal.y * groundNormal.y < groundNormal.WithY(0).sqrMagnitude) {
                return false;
            }
            
            return Vector3.Project(currentSpeed, groundNormal).normalized != groundNormal;
        }

        private Vector3 groundNormal => _isGrounded ? groundHit.GetCorrectNormalForSphere(Vector3.down) : Vector3.up;
        private Vector3 mainMovementDirection => Vector3.ProjectOnPlane(PositionService.ForwardVector, groundNormal).normalized;

        private bool jumping => Time.timeSinceLevelLoad - _jumpTime < JUMPING_START_TIME;
    }
}