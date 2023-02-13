using System;
using UnityEngine;

namespace DefaultNamespace.EditorTools.Splines {
    public static class SplineExt {
        public static Vector3 Bezier(Vector3 start, Vector3 guide, Vector3 tailGuide, Vector3 end, float t) {
            return (MathF.Pow(1 - t, 3) * start + (3 * Mathf.Pow(1 - t, 2) * t * guide + 3 * (1 - t) * t * t * tailGuide + Mathf.Pow(t, 3) * end));
        }

        public static Vector3 Bezier(this Spline spline, int subcurveIndex, float partition) {
            return Bezier(
                spline.points[subcurveIndex].Position, spline.points[subcurveIndex].guide,
                spline.points[subcurveIndex + 1].tailGuide, spline.points[subcurveIndex + 1].Position,
                partition);
        }

        public static Vector3 GetSegmentStart(this Spline spline, int subcurveIndex, int segmentIndex, int density) {
            return spline.Bezier(subcurveIndex, (float)segmentIndex / density);
        }
        
        public static Vector3 GetSegmentEnd(this Spline spline, int subcurveIndex, int segmentIndex, int density) {
            return spline.Bezier(subcurveIndex, (float)(segmentIndex + 1) / density);
        }

        public static Vector3 GetSegmentDirection(this Spline spline, int subcurveIndex, int segmentIndex, int density) {
            return (spline.GetSegmentEnd(subcurveIndex, segmentIndex, density) - spline.GetSegmentStart(subcurveIndex, segmentIndex, density)).normalized;
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