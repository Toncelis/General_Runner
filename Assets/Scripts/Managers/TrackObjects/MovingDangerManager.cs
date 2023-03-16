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
        
        var offset = points[1] - points[0];
        AddMoveAndLookToSequence(in sequence, points[1], offset.magnitude, Ease.InSine);
        for (int i = 2; i < length; i++) {
            offset = points[i] - points[i - 1];
            AddMoveAndLookToSequence(in sequence, points[i], offset.magnitude, Ease.Linear);
        }
        offset = points[length] - points[length - 1];
        AddMoveAndLookToSequence(in sequence, points[length], offset.magnitude, Ease.OutSine);
        
        
        sequence.Append(transform.DOLookAt(points[length-1], stopTime));
        offset = points[length - 1] - points[length];
        AddMoveAndLookToSequence(in sequence, points[length-1], offset.magnitude, Ease.InSine);
        for (int i = length-2; i > 0; i--) {
             offset = points[i] - points[i + 1];
            AddMoveAndLookToSequence(in sequence, points[i], offset.magnitude, Ease.Linear);
        }
        offset = points[1] - points[0];
        AddMoveAndLookToSequence(in sequence, points[0], offset.magnitude, Ease.OutSine);
        
        sequence.Append(transform.DOLookAt(points[1], stopTime));
        
        sequence.SetLoops(-1);
    }

    private void AddMoveAndLookToSequence(in Sequence sequence, Vector3 position, float distance, Ease easeType) {
        sequence.Append(transform.DOMove(position, distance / speed).SetEase(easeType));
        sequence.Join(transform.DOLookAt(position, rotationTime));
    }
    
}
