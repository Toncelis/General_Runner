using System.Linq;
using DefaultNamespace.EditorTools;
using DefaultNamespace.EditorTools.Splines;
using DefaultNamespace.Interfaces.DataAccessors;
using DefaultNamespace.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
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

        private MeasuredSpline _measuredSpline;
        
        public void Setup(TileConfig tileConfig) {
            TileConfig = tileConfig;
            _measuredSpline = new MeasuredSpline(_spline, Density);
            GenerateTrackObjects();
        }

        public void Destroy() {
            Destroy(gameObject);
        }

        public void SetSpline(Spline spline) {
            _spline = new Spline(spline);
        }

        private void OnEnable() {
            var serializedMeshes = transform.GetComponentsInChildren<SerializeMesh>();
            foreach (var serializer in serializedMeshes) {
                var mesh = serializer.Rebuild();;
                var meshCollider = serializer.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
            }
        }

        private void GenerateTrackObjects() {
            Debug.Log($"generating track objects for {name}", this);
            GameObject contentHolder = new GameObject("ContentHolder") {
                transform = {
                    parent = transform,
                    localPosition = Vector3.zero,
                    localRotation = Quaternion.identity
                }
            };
            Transform holderTransform = contentHolder.transform;
            
            float coveredLength = TileConfig.InitialBlankSpace;
            while (coveredLength < _measuredSpline.Length) {
                float freeLength = _measuredSpline.Length - coveredLength;
                bool canFillObjWithBlankSpace = TileConfig.GetNextTrackObject(objConfig => objConfig.Length <= freeLength - TileConfig.MinBlankSpace, out var obj);
                if (!canFillObjWithBlankSpace) {
                    bool canFillLastObject = TileConfig.GetNextTrackObject(objConfig => objConfig.Length <= freeLength, out obj);
                    if (canFillLastObject) {
                        PlaceObject(obj, Random.Range(coveredLength, _measuredSpline.Length - obj.Length), holderTransform);
                    }
                    break;
                }

                float extraFreeLength = freeLength - obj.Length;
                float maxBlankSpace = Mathf.Min(extraFreeLength, TileConfig.MaxBlankSpace);
                float positioningLength = coveredLength + Random.Range(TileConfig.MinBlankSpace, maxBlankSpace);
                PlaceObject(obj, positioningLength, holderTransform);
                coveredLength = positioningLength + obj.Length;
            }
        }

        public (Vector3, Vector3) GetPositionAndDirectionFromLength(float length) {
            var (localPosition, localDirection) = _measuredSpline.GetPositionAndDirection(length);
            return (transform.TransformPoint(localPosition), transform.TransformDirection(localDirection));
        } 
        
        public (Vector3, Vector3) GetLocalPositionAndDirectionFromLength(float length) {
            return _measuredSpline.GetPositionAndDirection(length);
        }

        private void PlaceObject(TrackObjectConfig obj, float positioningLength, Transform parent) {
            Debug.Log($"placing object : {obj.name}");
            if (obj.Complex) {
                foreach (var part in obj.ObjectParts) {
                    PlaceObject(part, positioningLength, parent);
                    positioningLength += part.Length;
                }
                return;
            }

            var (position, direction) = _measuredSpline.GetPositionAndDirection(positioningLength);
            var newContent = Instantiate(obj.Prefab, parent);
            var contentManager = newContent.GetComponent<TrackObjectManager>();
            if (contentManager != null) {
                contentManager.Setup(this, positioningLength);
            } else {
                newContent.transform.localPosition = position;
                newContent.transform.forward = parent.TransformDirection(direction);
            }
        }
        
        #if UNITY_EDITOR
        [Button]
        private void RefreshSpline() {
            var splineCreator = gameObject.GetComponent<SplineCreator>();
            if (splineCreator != null) {
                _spline = new Spline(splineCreator.Spline);
            }
        }
        #endif
    }
}