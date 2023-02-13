using System.Linq;
using Extensions;
using UnityEngine;

namespace DefaultNamespace.EditorTools.Splines {
    public class MeasuredSpline {
        private readonly Spline _spline;
        private readonly int _density;
        private readonly float _length;
        private readonly float[] _subcurveLengths;


        public MeasuredSpline(Spline spline, int density) {
            _spline = spline;
            _density = density;
            _length = 0;
            _subcurveLengths = new float [spline.count-1];

            for (int i = 0; i < spline.count - 1; i++) {
                _subcurveLengths[i] = CalculateSubcurveLength(i);
                _length += _subcurveLengths[i];
            }
        }

        private float CalculateSubcurveLength(int index) {
            float length = 0;
            for (int i = 0; i < _density; i++) {
                length += SegmentLength(index, i);
            }
            return length;
        }

        private float SubcurveLength(int index) => _subcurveLengths[index];

        private float SegmentLength(int subcurveIndex, int segmentIndex) {
            Vector3 segmentStart = _spline.GetSegmentStart(subcurveIndex, segmentIndex, _density);
            Vector3 segmentEnd = _spline.GetSegmentEnd(subcurveIndex, segmentIndex, _density);
            return Vector3.Distance(segmentStart, segmentEnd);
        }

        public float Length => _length;

        public (Vector3, Vector3) GetPositionAndDirection(float length) {
            if (Length < length) {
                return 
                    (_spline.points.Last().Position + _spline.points.Last().Direction.WithLength(Length - length), 
                    _spline.points.Last().Direction.normalized);
            }

            int subcurveIndex = 0;
            float subcurveLength = SubcurveLength(subcurveIndex);

            while (length > subcurveLength) {
                length -= subcurveLength;
                subcurveIndex++;
                subcurveLength = SubcurveLength(subcurveIndex);
            }

            int segmentIndex = 0;
            float segmentLength = SegmentLength(subcurveIndex, segmentIndex);
            while (length > segmentLength) {
                length -= segmentLength;
                segmentIndex++;
                segmentLength = SegmentLength(subcurveIndex, segmentIndex);
            }

            Vector3 segmentStart = _spline.GetSegmentStart(subcurveIndex, segmentIndex, _density);
            Vector3 segmentEnd = _spline.GetSegmentEnd(subcurveIndex, segmentIndex, _density);
            return (
                segmentStart + (segmentEnd - segmentStart).WithLength(length),
                _spline.GetSegmentDirection(subcurveIndex, segmentIndex, _density));
        }

        public Vector3 Bezier(int subcurveIndex, float partition) => _spline.Bezier(subcurveIndex, partition);
    }
}