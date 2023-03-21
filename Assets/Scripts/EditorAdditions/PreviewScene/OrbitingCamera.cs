using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace.EditorAdditions.PreviewScene {
    [ExecuteInEditMode]
    public class OrbitingCamera : MonoBehaviour{
        private Vector3 _radius;
        private float _angle;
        [SerializeField] private Camera Camera;

        private readonly CancellationTokenSource _tokenSource = new();

        private const float PERIOD = 2f;
        private const float REFRESH_PERIOD = 0.05f;

        [SerializeField] private Transform CameraPositioner;

        public Camera camera => Camera;
        
        public void StartMovement(Vector3 radius) {
            _radius = radius;
            UpdateCameraPosition();
            Task.Run(Spin);
        }
        
        private async void Spin() {
            while (true) {
                _angle += 360 * REFRESH_PERIOD / PERIOD;
                _angle = Mathf.Repeat(_angle, 360f);
                UpdateCameraPosition();
                await Task.Delay(Mathf.FloorToInt(PERIOD * 100f));
            }
        }

        private void UpdateCameraPosition() {
            var radius = Quaternion.Euler(0, _angle, 0) * _radius;
            camera.transform.position = radius;
            camera.transform.LookAt(Vector3.zero);
            
            Debug.Log("camera position : " + camera.transform.position);
        }

        private void OnDisable() {
            _tokenSource.Cancel();
        }
    }
}