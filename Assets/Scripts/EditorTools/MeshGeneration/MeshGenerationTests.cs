using System.Collections.Generic;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.EditorTools.MeshGeneration {
    public class MeshGenerationTests : MonoBehaviour {
        [Button]
        public void GenerateCubeWithHardEdges() {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();

            AddHardQuad(verticesList, trianglesList, Vector3.zero, Vector3.up, Vector3.back + Vector3.left);
            AddHardQuad(verticesList, trianglesList, Vector3.zero, Vector3.down, Vector3.back + Vector3.left);
            AddHardQuad(verticesList, trianglesList, Vector3.zero, Vector3.right, Vector3.back + Vector3.up);
            AddHardQuad(verticesList, trianglesList, Vector3.zero, Vector3.left, Vector3.back + Vector3.up);
            AddHardQuad(verticesList, trianglesList, Vector3.zero, Vector3.forward, Vector3.up + Vector3.left);
            AddHardQuad(verticesList, trianglesList, Vector3.zero, Vector3.back, Vector3.up + Vector3.left);

            GenerateObj("hard cube", GenerateMesh(verticesList, trianglesList, null));
        }

        [Button]
        public void GenerateCubeWithSoftEdges() {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();

            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.up, Vector3.back + Vector3.left);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.down, Vector3.back + Vector3.left);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.right, Vector3.back + Vector3.up);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.left, Vector3.back + Vector3.up);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.forward, Vector3.up + Vector3.left);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.back, Vector3.up + Vector3.left);

            GenerateObj("soft cube", GenerateMesh(verticesList, trianglesList, null));
        }

        [Button]
        public void GenerateCubeWithSoftEdgesAndDiameterNormals() {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.up, Vector3.back + Vector3.left);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.down, Vector3.back + Vector3.left);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.right, Vector3.back + Vector3.up);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.left, Vector3.back + Vector3.up);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.forward, Vector3.up + Vector3.left);
            AddSoftQuad(verticesList, trianglesList, Vector3.zero, Vector3.back, Vector3.up + Vector3.left);

            foreach (var vertex in verticesList) {
                Vector3 normal = vertex.normalized;
                normals.Add(normal);
            }

            GenerateObj("soft cube with normals", GenerateMesh(verticesList, trianglesList, normals));
        }

        private void AddHardQuad(List<Vector3> vertices, List<int> triangles, Vector3 center, Vector3 edgeCenter, Vector3 diagonal) {
            var vertexNewIndex = vertices.Count;

            for (int i = 0; i < 4; i++) {
                var vertex = edgeCenter + Quaternion.AngleAxis(i * 90, edgeCenter - center) * diagonal;
                vertices.Add(vertex);
            }

            AddQuad(new [] { vertexNewIndex, vertexNewIndex + 1, vertexNewIndex + 3, vertexNewIndex + 2 }, triangles);
        }

        private void AddSoftQuad(List<Vector3> vertices, List<int> triangles, Vector3 center, Vector3 edgeCenter, Vector3 diagonal) {
            List<int> vertexIndexes = new();

            for (int i = 0; i < 4; i++) {
                var vertex = edgeCenter + Quaternion.AngleAxis(i * 90, edgeCenter - center) * diagonal;
                vertices.AddIfNewAndNotNull(vertex);
                vertexIndexes.Add(vertices.IndexOf(vertex));
            }

            (vertexIndexes[3], vertexIndexes[2]) = (vertexIndexes[2], vertexIndexes[3]);
            AddQuad(vertexIndexes.ToArray(), triangles);
        }

        private void AddQuad(int[] indexes, List<int> triangles) {
            triangles.Add(indexes[0]);
            triangles.Add(indexes[1]);
            triangles.Add(indexes[2]);

            triangles.Add(indexes[1]);
            triangles.Add(indexes[3]);
            triangles.Add(indexes[2]);
        }

        private void GenerateObj(string name, Mesh mesh) {
            var meshHolder = new GameObject(name) {
                transform = {
                    parent = transform,
                    localPosition = Vector3.zero
                }
            };
            var meshFilter = meshHolder.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshHolder.AddComponent<MeshRenderer>();
        }

        private Mesh GenerateMesh(List<Vector3> vertices, List<int> triangles, List<Vector3> normals) {
            var mesh = new Mesh {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };

            if (normals != null) {
                mesh.normals = normals.ToArray();
            } else {
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
            }

            return mesh;
        }
    }
}