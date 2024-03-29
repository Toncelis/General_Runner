using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

[Serializable]
public class Spline {
    public List<SplinePoint> points;

    public int count => points.Count;

    public Spline() {
        points = new List<SplinePoint> {
            new()
        };
    }

    public void AddPoint(Vector3 position) {
        if (points.IsNullOrEmpty()) {
            points = new List<SplinePoint> {
                new(position, Vector3.up)
            };
            return;
        }
        
        points.Add(new (position, (position - points.Last().guide).normalized));
    }

    public Spline(Spline otherSpline) {
        points = new();
        foreach (var point in otherSpline.points) {
            points.Add(new SplinePoint(point));
        }
    }
}

[Serializable]
public class SplinePoint {
    public Vector3 Position;
    public Vector3 Direction;
    
    public Vector3 guide {
        get => Position + Direction;
        set => Direction = value - Position;
    }
    
    public Vector3 tailGuide {
        get => Position - Direction;
        set => Direction = Position - value;
    }

    public SplinePoint() {
        Position = Vector3.zero;
        Direction = Vector3.forward;
    }
    
    public SplinePoint(Vector3 newPosition, Vector3 newDirection) {
        Position = newPosition;
        Direction = newDirection;
    }

    public SplinePoint(SplinePoint oldPoint) {
        Position = oldPoint.Position;
        Direction = oldPoint.Direction;
    }
}