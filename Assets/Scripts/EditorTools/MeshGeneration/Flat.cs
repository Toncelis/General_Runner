using System;
using UnityEngine;

namespace DefaultNamespace.EditorTools.MeshGeneration {
    [Serializable]
    public class Flat {
        private Vector3[] vertices;
        private int[] triangles;
        private int[] borderEdges;
    }
}