using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.EditorTools;
using DefaultNamespace.EditorTools.Splines;
using DefaultNamespace.Interfaces.DataAccessors;
using Extensions;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ISplineHolder))]
public class SplineMeshCreator : MonoBehaviour {
    protected Spline spline;
    public int Density = 20;

    private void OnValidate() {
        spline = new Spline(GetComponent<ISplineHolder>().Spline);
    }

    [Button]
    public void GenerateHardMesh(float leftEdgeOffset, float rightEdgeOffset, float bottomEdgeOffset, float topEdgeOffset, MeshFilter meshFilter) {
        if (spline.count < 2) {
            return;
        }

        var mesh = new Mesh {
            name = "Procedural Mesh"
        };

        var initialVertices = GenerateVertices(spline, leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset);

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector3> normals = new();

        GenerateHardMesh(initialVertices, vertices, triangles, normals);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        meshFilter.mesh = mesh;
    }

    [Button]
    public void GenerateMesh(float leftEdgeOffset, float rightEdgeOffset, float bottomEdgeOffset, float topEdgeOffset, MeshFilter meshFilter) {
        if (spline.count < 2) {
            return;
        }

        var mesh = new Mesh {
            name = "Procedural Mesh"
        };

        var vertices = GenerateVertices(spline, leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset);
        mesh.vertices = vertices;
        var triangles = GenerateTrianglesWithVolume(vertices.Length);
        mesh.triangles = triangles.ToArray();

        meshFilter.mesh = mesh;
    }

    [Button]
    public GameObject GenerateChildMesh(string childName, float leftEdgeOffset, float rightEdgeOffset, float bottomEdgeOffset, float topEdgeOffset, bool useHardQuads) {
        MeshFilter meshFilter;

        for (int i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i);
            if (child.name != childName) {
                continue;
            }

            meshFilter = child.GetOrAddComponent<MeshFilter>();
            if (useHardQuads) {
                GenerateHardMesh(leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset, meshFilter);
            } else {
                GenerateMesh(leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset, meshFilter);
            }
            GameObject o;
            (o = child.gameObject).AddComponent<SerializeMesh>();
            return o;
        }

        var meshHolder = new GameObject(childName);
        meshHolder.transform.parent = transform;
        meshHolder.transform.localPosition = Vector3.zero;
        meshFilter = meshHolder.AddComponent<MeshFilter>();
        meshHolder.AddComponent<SerializeMesh>();
        if (useHardQuads) {
            GenerateHardMesh(leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset, meshFilter);
        } else {
            GenerateMesh(leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset, meshFilter);
        }
        return meshHolder;
    }

    private Vector3[] GenerateVertices(Spline spline, float leftEdgeOffset, float rightEdgeOffset, float bottomEdgeOffset, float topEdgeOffset) {
        var vertices = new List<Vector3>();

        var segmentStart = spline.points[0].Position;
        AddSquareVertices(segmentStart, spline.points[0].Direction, vertices, leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset);

        float step = 1f / Density;

        for (int i = 0; i < spline.count - 1; i++) {
            for (float t = step; t <= 1; t += step) {
                var segmentEnd = spline.Bezier(i, t);
                AddSquareVertices(segmentEnd, segmentEnd - segmentStart, vertices, leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset);
                segmentStart = segmentEnd;
            }
        }
        AddSquareVertices(spline.points.Last().Position, spline.points.Last().Direction, vertices, leftEdgeOffset, rightEdgeOffset, bottomEdgeOffset, topEdgeOffset);

        return vertices.ToArray();
    }

    private void AddSquareVertices(Vector3 center, Vector3 direction, List<Vector3> vertices, float leftEdgeOffset, float rightEdgeOffset, float bottomEdgeOffset, float topEdgeOffset) {
        direction = direction.WithY(0).normalized;
        var sideVector = Quaternion.Euler(0, 90, 0) * direction;
        vertices.Add(center + sideVector * rightEdgeOffset + Vector3.up * topEdgeOffset);
        vertices.Add(center + sideVector * leftEdgeOffset + Vector3.up * topEdgeOffset);
        vertices.Add(center + sideVector * rightEdgeOffset + Vector3.up * bottomEdgeOffset);
        vertices.Add(center + sideVector * leftEdgeOffset + Vector3.up * bottomEdgeOffset);
    }

    private void GenerateHardMesh(Vector3[] initialVertices, List<Vector3> vertices, List<int> triangles, List<Vector3> normals) {
        for (int i = 0; i < initialVertices.Length - 7; i += 4) {
            var quadVertices = new[] { initialVertices[i], initialVertices[i + 1], initialVertices[i + 4], initialVertices[i + 5] };
            //top quad
            AddHardQuad(quadVertices, vertices, triangles, normals);
            quadVertices = new[] { initialVertices[i + 3], initialVertices[i + 2], initialVertices[i + 7], initialVertices[i + 6] };
            //bot quad
            AddHardQuad(quadVertices, vertices, triangles, normals);
            quadVertices = new[] { initialVertices[i + 2], initialVertices[i], initialVertices[i + 6], initialVertices[i + 4] };
            //right quad
            AddHardQuad(quadVertices, vertices, triangles, normals);
            quadVertices = new[] { initialVertices[i + 1], initialVertices[i + 3], initialVertices[i + 5], initialVertices[i + 7] };
            //left quad
            AddHardQuad(quadVertices, vertices, triangles, normals);
        }
    }

    private int[] GenerateTrianglesWithVolume(int verticesCount) {
        var triangles = new List<int>();
        for (int i = 0; i < verticesCount - 7; i += 4) {
            //top quad
            AddQuad(new[] { i, i + 1, i + 4, i + 5 }, triangles);
            //bot quad
            AddQuad(new[] { i + 3, i + 2, i + 7, i + 6 }, triangles);
            //right quad
            AddQuad(new[] { i + 2, i, i + 6, i + 4 }, triangles);
            //left quad
            AddQuad(new[] { i + 1, i + 3, i + 5, i + 7 }, triangles);
        }

        //back quad
        AddQuad(new[] { 1, 0, 3, 2 }, triangles);

        //front quad
        AddQuad(new[] { verticesCount - 4, verticesCount - 3, verticesCount - 2, verticesCount - 1 }, triangles);

        return triangles.ToArray();
    }

    private void AddHardQuad(Vector3[] initialVertices, List<Vector3> vertices, List<int> triangles, List<Vector3> normals) {
        int quadStartIndex = vertices.Count;
        vertices.AddRange(initialVertices);

        triangles.Add(quadStartIndex);
        triangles.Add(quadStartIndex + 1);
        triangles.Add(quadStartIndex + 2);

        triangles.Add(quadStartIndex + 1);
        triangles.Add(quadStartIndex + 3);
        triangles.Add(quadStartIndex + 2);

        Vector3 normal0 = Vector3.Cross(initialVertices[1] - initialVertices[0], initialVertices[2] - initialVertices[0]).normalized;
        Vector3 normal = Vector3.Cross(initialVertices[3] - initialVertices[0], initialVertices[2] - initialVertices[1]).normalized;
        Vector3 normal3 = Vector3.Cross(initialVertices[3] - initialVertices[1], initialVertices[2] - initialVertices[1]).normalized;

        normals.AddRange(new[] { normal0, normal, normal, normal3 });
    }

    private void AddQuad(int[] indexes, List<int> triangles) {
        // if (indexes.Length != 4) { Debug.LogError("Error")); return;}

        triangles.Add(indexes[0]);
        triangles.Add(indexes[1]);
        triangles.Add(indexes[2]);

        triangles.Add(indexes[1]);
        triangles.Add(indexes[3]);
        triangles.Add(indexes[2]);
    }

    private Vector3 Bezier(Vector3 start, Vector3 guide, Vector3 tailGuide, Vector3 end, float t) {
        return (MathF.Pow(1 - t, 3) * start + (3 * Mathf.Pow(1 - t, 2) * t * guide + 3 * (1 - t) * t * t * tailGuide + Mathf.Pow(t, 3) * end));
    }
}