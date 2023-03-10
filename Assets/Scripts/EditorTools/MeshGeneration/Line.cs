using UnityEngine;
using System;
using Sirenix;
using Sirenix.OdinInspector;

namespace DefaultNamespace.EditorTools.MeshGeneration {
    [Serializable]
    public class Line {
        // first and last vertices describe start and end directions
        [SerializeField]
        private Vector3[] _line ;

        [SerializeField]
        private bool CustomWidth = false;
        
        [SerializeField, ShowIf(nameof(CustomWidth))]
        private float[] _width;
        [SerializeField, HideIf(nameof(CustomWidth))]
        private float _generalWidth;

        public int Count => _line.Length - 2;
        public Vector3 Vertex(int index) => _line[index+1];
        public Vector3[] GetEdgeVertices(int index, Vector3 vertical) {
            Vector3 forwardVector = (_line[index + 2] - _line[index]).normalized;
            Vector3 sideVector = Quaternion.AngleAxis(90, vertical) * forwardVector;
            if (CustomWidth) {
                sideVector *= _width[index];
            } else {
                sideVector *= _generalWidth;
            }
            return new Vector3[] {_line[index+1]+sideVector, _line[index+1]-sideVector};
        }
    }
}