using Extensions;
using Services;
using UnityEngine;

namespace DefaultNamespace.Run {
    public class SimpleCameraFollower : MonoBehaviour {
        private Vector3 offset;
        private Vector3 currentForwardVector;

        public Transform cameraHelper;
        public Transform characterCenter;

        private PositionService _positionService => ServiceLibrary.GetService<PositionService>();
        
        private void Start() {
            offset = cameraHelper.position - characterCenter.position;
            currentForwardVector = Vector3.forward;
        }

        private void Update() {
            var forwardVector = _positionService.ForwardVector;
            if (currentForwardVector != forwardVector) {
                offset = Quaternion.FromToRotation(currentForwardVector.WithY(0), forwardVector.WithY(0)) * offset;
                currentForwardVector = forwardVector;
            }

            cameraHelper.position = characterCenter.position + offset;
        }
    }
}