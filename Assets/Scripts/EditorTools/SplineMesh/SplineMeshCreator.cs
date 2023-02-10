using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineMeshCreator : MonoBehaviour {
    public SplineCreator SplineCreator;
    public Material RoadMaterial;
    public int Density;
    public float RoadWidth;

    [Button]
    public void GenerateMesh() {
        var spline = SplineCreator.Spline;
        if (spline.count < 2) {
            return;
        }

        var mesh = new Mesh {
            name = "Procedural Mesh"
        };

        var vertices = GenerateVertices(spline);
        mesh.vertices = vertices;

        var triangles = GenerateTrianglesWithVolume(vertices.Length);
        mesh.triangles = triangles.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = RoadMaterial;
    }

    private Vector3[] GenerateVertices(Spline spline) {
        var vertices = new List<Vector3>();

        var segmentStart = spline.points[0].Position;
        AddSquareVertices(segmentStart, spline.points[0].Direction, vertices, RoadWidth);

        float step = 1f / Density;

        for (int i = 0; i < spline.count - 1; i++) {
            for (float t = step; t <= 1; t += step) {
                var segmentEnd = Bezier(
                    spline.points[i].Position, spline.points[i].guide,
                    spline.points[i + 1].tailGuide, spline.points[i + 1].Position,
                    t);
                AddSquareVertices(segmentEnd, segmentEnd - segmentStart, vertices, RoadWidth);
                segmentStart = segmentEnd;
            }
        }
        AddSquareVertices(spline.points.Last().Position, spline.points.Last().Direction, vertices, RoadWidth);

        return vertices.ToArray();
    }

    private void AddSquareVertices(Vector3 center, Vector3 direction, List<Vector3> vertices, float width, float height = 4) {
        direction = direction.WithY(0).normalized * width;
        var sideVector = Quaternion.Euler(0, 90, 0) * direction;
        vertices.Add(center + sideVector);
        vertices.Add(center - sideVector);
        vertices.Add(center + sideVector + Vector3.down * height);
        vertices.Add(center - sideVector + Vector3.down * height);
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

    private Vector3 Bezier(int segmentIndex, float t) {
        
    }
}