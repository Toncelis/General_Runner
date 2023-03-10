using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.EditorTools.MeshGeneration {
    public class FlatToMeshGenerator : MonoBehaviour {
        public Line line;

        public float height;


        [SerializeField] private Mesh _mesh;
        
        [Button]
        public void GenerateFlatMesh() {
            ClearCollisions();
            
            List<Vector3> vertices = new();
            for (int i = 0; i < line.Count; i++) {
                vertices.AddRange(line.GetEdgeVertices(i, Vector3.up));
            }

            List<int> triangles = new();
            for (int i = 0; i < vertices.Count - 2; i += 2) {
                triangles.AddRange(new [] {i, i+1, i+2});
                triangles.AddRange(new [] {i+2, i+1, i+3});
            }
            
            List<Vector3> normals = Enumerable.Repeat(Vector3.up, vertices.Count).ToList();

            var meshFilter = GetComponent<MeshFilter>();
            _mesh = new Mesh {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                normals = normals.ToArray(),
                name = $"MeshFromLine{name}"
            };
            meshFilter.mesh = _mesh;
        }

        private void ClearCollisions() {
            var colliders = GetComponents<Collider>();
            for (int i = colliders.Length - 1; i >= 0; i--) {
                DestroyImmediate(colliders[i]);
            }
        }
    }
}