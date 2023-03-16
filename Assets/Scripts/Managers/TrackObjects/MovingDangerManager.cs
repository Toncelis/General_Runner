using System.Collections.Generic;
using DefaultNamespace.Managers;
using DefaultNamespace.World.View;
using DG.Tweening;
using Services;
using UnityEngine;

public class MovingDangerManager : TrackObjectManager {
    public float speed;
    public float rotationTime;
    public float stopTime;
    public List<float> travelPointsViaLength;

    public override void Setup(TileView tile, float positionViaLength) {
        List<Vector3> points = new();
        foreach (var pointViaLength in travelPointsViaLength) {
            var point = tile.GetPositionAndDirectionFromLength(positionViaLength + pointViaLength).Item1;
            points.Add(point);
        }
        transform.position = points[0];
        transform.LookAt(points[1]);

        int length = points.Count-1;
        
        var sequence = DOTween.Sequence();
        for (int i = 1; i <= length; i++) {
            var offset = points[i] - points[i - 1];
            sequence.Append(transform.DOMove(points[i], offset.magnitude / speed));
            sequence.Join(transform.DOLookAt(points[i], rotationTime));
        }
        sequence.Append(transform.DOLookAt(points[length-1], stopTime));
        for (int i = length-1; i >= 0; i--) {
            var offset = points[i] - points[i + 1];
            sequence.Append(transform.DOMove(points[i], offset.magnitude / speed));
            sequence.Join(transform.DOLookAt(points[i], rotationTime));
        }
        sequence.Append(transform.DOLookAt(points[1], stopTime));
        sequence.SetLoops(-1);
        sequence.Restart();
    }
    
    
    
}
