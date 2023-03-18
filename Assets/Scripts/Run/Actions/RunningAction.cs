using System.Collections;
using Extensions;
using Services;
using UnityEngine;

namespace DefaultNamespace.Run.Actions {
    [CreateAssetMenu(menuName = "Action/Run", order = 0, fileName = "CharacterRunAction")]
    public class RunningAction : CharacterAction {
        private const float JUMPING_START_TIME = 0.1f;
        private const float TIME_STEP = 0.01f;

        
        [SerializeField] private float G = 18;
        [SerializeField] private float RunningSpeed = 8;
        [SerializeField] private float SprintExtraSpeed = 6;
        [SerializeField] private float Acceleration = 20;
        [SerializeField] private float JumpHeight = 2;
        
        private PositionService positionService => ServiceLibrary.GetService<PositionService>();
        private float jumpSpeed => Mathf.Sqrt(2 * JumpHeight * G);
        private Vector3 groundNormal => _isGrounded ? _groundHit.GetCorrectNormalForSphere(Vector3.down) : Vector3.up;
        private Vector3 mainMovementDirection => Vector3.ProjectOnPlane(positionService.forwardVector, groundNormal).normalized;
        private bool jumping => Time.timeSinceLevelLoad - _jumpTime < JUMPING_START_TIME;
        
        private float _jumpTime;
        private GameObject _debugCube;
        private bool _isGrounded = true;
        private static readonly int _runningSpeed = Animator.StringToHash("RunningSpeed");
        private static readonly int _grounded = Animator.StringToHash("Grounded");

        private RaycastHit _groundHit;
        private static readonly int _jumpHash = Animator.StringToHash("Jump");

        protected override IEnumerator ActionRoutine() {
            lastFramePosition = character.characterTransform.position;
            _jumpTime = Time.timeSinceLevelLoad-JUMPING_START_TIME;
            
            while (true) {
                var desiredSpeed = CalculateDesiredSpeed(positionService.forwardVector.XZProjection().normalized);
                
                UpdateCharacterRotationAndAnimation();
                UpdateGroundedState();
                UpdateSpeed(desiredSpeed);

                character.characterController.Move(currentSpeed * TIME_STEP);
                positionService.ProcessNewPosition(character.characterTransform.position);
                
                /*
                character.currentSpeedArrow.forward = currentSpeed;
                character.currentSpeedArrow.localScale = currentSpeed.magnitude * Vector3.one;
                character.normalArrow.forward = groundNormal;
                character.forwardArrow.forward = mainMovementDirection;
                */
                
                yield return new WaitForSeconds(TIME_STEP);
            }
        }
        
        private Vector3 CalculateDesiredSpeed(Vector2 forwardVector) {
            var forwardSpeed = (Input.GetKey(KeyCode.W) ? 4 : 2 + (Input.GetKey(KeyCode.S) ? -1 : 0)) * Vector2.up;
            var sideSpeed = (Input.GetKey(KeyCode.D) ? 2 : 0) * Vector2.right + (Input.GetKey(KeyCode.A) ? 2 : 0) * Vector2.left;
            var speed = (forwardSpeed + sideSpeed).normalized * RunningSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) {
                speed += Vector2.up * SprintExtraSpeed;
            }
            return speed.ToXZByDirection(forwardVector);
        }
        
        private void UpdateCharacterRotationAndAnimation() {
            var position = character.characterTransform.position;
            var positionDelta = (position - lastFramePosition).WithY(0);
            if (positionDelta.sqrMagnitude > 0) {
                character.characterTransform.forward = positionDelta;
            }   

            lastFramePosition = position;
        }
        
        private void UpdateGroundedState() {
            var isGrounded = IsGrounded();
            if (isGrounded != _isGrounded) {
                _isGrounded = isGrounded;
                character.animator.SetBool(_grounded, _isGrounded);
            }
        }
        
        private bool IsGrounded() {
            var position = character.characterController.center + character.characterController.transform.position;
            if (!Physics.SphereCast(position, character.characterController.radius-0.01f, Vector3.down, out _groundHit, character.characterController.height / 2 + 0.1f, 1<<LayerMask.NameToLayer("Default"))) {
                return false;
            };

            if (!_groundHit.collider.CompareTag("ground")) {
                return false;
            }

            if (groundNormal.y * groundNormal.y < groundNormal.WithY(0).sqrMagnitude) {
                return false;
            }
            
            return Vector3.Project(currentSpeed, groundNormal).normalized != groundNormal;
        }

        private void UpdateSpeed(Vector3 desiredSpeed) {
            Vector3 tangentSpeed;
            Vector3 normalSpeed;
            
            if (_isGrounded && !jumping) {
                if (Input.GetKey(KeyCode.Space)) {
                    tangentSpeed = desiredSpeed.WithY(0);
                    normalSpeed = jumpSpeed * Vector3.up;
                    character.animator.SetTrigger(_jumpHash);
                    _jumpTime = Time.timeSinceLevelLoad;
                } else {
                    tangentSpeed = Quaternion.FromToRotation(positionService.forwardVector.WithY(0), mainMovementDirection) * desiredSpeed;
                    normalSpeed = -groundNormal * 2;
                }
            } else {
                tangentSpeed = desiredSpeed.WithY(0);
                normalSpeed = (currentSpeed.y - G * TIME_STEP) * Vector3.up;
            }

            Vector3 previousTangentSpeed = normalSpeed == Vector3.zero ?  Vector3.ProjectOnPlane(currentSpeed, Vector3.up) : Vector3.ProjectOnPlane(currentSpeed, normalSpeed);
            character.animator.SetFloat(_runningSpeed, previousTangentSpeed.magnitude);
            Vector3 desiredDeltaTangentSpeed = tangentSpeed - previousTangentSpeed;
            
            if (desiredDeltaTangentSpeed.magnitude <= Acceleration * TIME_STEP) {
                currentSpeed = tangentSpeed + normalSpeed;
            } else {
                currentSpeed = previousTangentSpeed + normalSpeed + desiredDeltaTangentSpeed.normalized * Acceleration * TIME_STEP;
            }
            
        }
    }
}