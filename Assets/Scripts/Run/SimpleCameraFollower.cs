using Extensions;
using Services;
using UnityEngine;

namespace DefaultNamespace.Run {
    public class SimpleCameraFollower : MonoBehaviour {
        [SerializeField] private Transform CameraHelper;
        [SerializeField] private Transform CharacterCenter;
        
        private Vector3 _offset;
        private Vector3 _currentForwardVector;

        private PositionService _positionService => ServiceLibrary.GetService<PositionService>();
        
        private void Start() {
            _offset = CameraHelper.position - CharacterCenter.position;
            _currentForwardVector = Vector3.forward;
        }

        private void Update() {
            var forwardVector = _positionService.forwardVector;
            if (_currentForwardVector != forwardVector) {
                _offset = Quaternion.FromToRotation(_currentForwardVector.WithY(0), forwardVector.WithY(0)) * _offset;
                _currentForwardVector = forwardVector;
            }

            CameraHelper.position = CharacterCenter.position + _offset;
        }
    }
}