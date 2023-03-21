using System.Linq;
using DefaultNamespace.EditorTools;
using DefaultNamespace.EditorTools.Splines;
using DefaultNamespace.Interfaces.DataAccessors;
using DefaultNamespace.Managers;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using World.Model;

namespace DefaultNamespace.World.View {
    public class TileView : MonoBehaviour, ISplineHolder {
        [SerializeField]
        private Spline _spline;

        public Spline Spline => _spline;
        public int Density;

        public Vector3 exitDirectionLocal => Spline.points.Last().Direction; 
        public Vector3 exitDirectionWorld =>transform.TransformDirection(exitDirectionLocal);

        public Vector3 exitPointLocal => Spline.points.Last().Position;
        public Vector3 exitPointWorld => transform.TransformPoint(exitPointLocal);

        private MeasuredSpline _measuredSpline;
        private TileConfig _tileConfig;
        public TileConfig TileConfig =>_tileConfig;
        
        public void Setup(TileConfig tileConfig) {
            _tileConfig = tileConfig;
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
            
            float coveredLength = _tileConfig.initialBlankSpace;
            while (coveredLength < _measuredSpline.Length) {
                float offset = 0f;
                float freeLength = _measuredSpline.Length - coveredLength;
                bool canFillObjWithBlankSpace = _tileConfig.GetNextTrackObject(objConfig => objConfig.length <= freeLength - _tileConfig.minBlankSpace, out var obj);
                if (!canFillObjWithBlankSpace) {
                    bool canFillLastObject = _tileConfig.GetNextTrackObject(objConfig => objConfig.length <= freeLength, out obj);
                    if (canFillLastObject) {
                        PlaceObject(obj, Random.Range(coveredLength, _measuredSpline.Length - obj.length), holderTransform, ref offset);
                    }
                    break;
                }

                float extraFreeLength = freeLength - obj.length;
                float maxBlankSpace = Mathf.Min(extraFreeLength, _tileConfig.maxBlankSpace);
                float positioningLength = coveredLength + Random.Range(_tileConfig.minBlankSpace, maxBlankSpace);
                PlaceObject(obj, positioningLength, holderTransform, ref offset);
                coveredLength = positioningLength + obj.blockingLength;
            }
        }

        public (Vector3, Vector3) GetPositionAndDirectionFromLength(float length) {
            var (localPosition, localDirection) = _measuredSpline.GetPositionAndDirection(length);
            return (transform.TransformPoint(localPosition), transform.TransformDirection(localDirection));
        } 
        
        public (Vector3, Vector3) GetLocalPositionAndDirectionFromLength(float length) {
            return _measuredSpline.GetPositionAndDirection(length);
        }

        private void PlaceObject(TrackObjectSettings obj, float positioningLength, Transform parent, ref float offset) {
            Debug.Log($"placing object : {obj.config.name}");
            if (obj.config.complex) {
                offset += obj.offset;
                foreach (var part in obj.config.objectParts) {
                    PlaceObject(part, positioningLength, parent, ref offset);
                    positioningLength += part.length;
                }
                return;
            }

            var (position, direction) = _measuredSpline.GetPositionAndDirection(positioningLength);
            var offsetDirection = Quaternion.Euler(0,90,0) * direction.WithY(0);
            offset += obj.offset;
            position += obj.offset * offsetDirection + Vector3.up * obj.height;
            var newContent = Instantiate(obj.config.prefab, parent);
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