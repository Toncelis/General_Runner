using System.Buffers;
using System.Linq;
using DefaultNamespace.EditorTools;
using DefaultNamespace.Interfaces.DataAccessors;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using World.Model;

namespace DefaultNamespace.World.View {
    public class TileView : MonoBehaviour, ISplineHolder {
        [SerializeField]
        private Spline _spline;

        public Spline Spline => _spline;
        public int Density;
        public TileConfig TileConfig;

        public Vector3 exitDirectionLocal => Spline.points.Last().Direction; 
        public Vector3 exitDirectionWorld =>transform.TransformDirection(exitDirectionLocal);

        public Vector3 exitPointLocal => Spline.points.Last().Position;
        public Vector3 exitPointWorld => transform.TransformPoint(exitPointLocal);

        public void Setup(TileConfig tileConfig) {
            TileConfig = tileConfig;
        }

        public void Destroy() {
            Destroy(gameObject);
        }

        public void SetSpline(Spline spline) {
            _spline = new Spline(spline);
        }
        
        [Button]
        private void RefreshSpline() {
            var splineCreator = gameObject.GetComponent<SplineCreator>();
            if (splineCreator != null) {
                _spline = new Spline(splineCreator.Spline);
            }
        }

        private void OnEnable() {
            var serializedMeshes = transform.GetComponentsInChildren<SerializeMesh>();
            foreach (var serializer in serializedMeshes) {
                var mesh = serializer.Rebuild();;
                var meshCollider = serializer.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
            }
        }
    }
}