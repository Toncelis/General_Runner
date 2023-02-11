using System;
using System.Drawing;
using UnityEngine;

namespace DefaultNamespace.EditorTools.Splines {
    public static class SplineExt {
        public static Vector3 Bezier(Vector3 start, Vector3 guide, Vector3 tailGuide, Vector3 end, float t) {
            return (MathF.Pow(1 - t, 3) * start + (3 * Mathf.Pow(1 - t, 2) * t * guide + 3 * (1 - t) * t * t * tailGuide + Mathf.Pow(t, 3) * end));
        }

        public static Vector3 Bezier(this Spline spline, int segmentIndex, float t) {
            return Bezier(
                spline.points[segmentIndex].Position, spline.points[segmentIndex].guide,
                spline.points[segmentIndex+1].tailGuide, spline.points[segmentIndex+1].Position,
                t);
        }

        public static void CopyFrom(this Spline spline, Spline oldSpline) {
            spline.points.Clear();
            foreach (var point in oldSpline.points) {
                spline.points.Add(new SplinePoint(point));
            }
        }

        public static void CopyTo(this Spline spline, Spline newSpline) {
            newSpline.points.Clear();
            foreach (var point in spline.points) {
                newSpline.points.Add(new SplinePoint(point));
            }
        }
    }
}