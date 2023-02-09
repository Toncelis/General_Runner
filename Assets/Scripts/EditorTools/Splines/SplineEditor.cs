using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineCreator))]
public class SplineEditor : Editor {
    private const float POINT_SIZE = 1f;
    private const float GUIDE_SIZE = 0.8f;

    private const float GUIDE_LINE_THICKNESS = 1f;
    private const float CURVE_THICKNESS =2f;

    private static Color POINTS_COLOR = Color.red;
    private static Color GUIDES_COLOR = new (0.2f,0.2f,0.5f);
    private static Color TAIL_GUIDES_COLOR = new (1,1,0);
    private static Color CURVE_COLOR = Color.green;

    private SplineCreator _creator;
    private Spline _spline;
    private Vector3 localPosition => _creator.transform.position;

    private void OnEnable() {
        _creator = (SplineCreator)target;
        if (_creator.Spline == null) {
            _creator.CreateSpline();
        }
        _spline = _creator.Spline;
    }

    private void OnSceneGUI() {
        Input();
        Draw();
    }

    private void Input() {
        Event guiEvent = Event.current;
        var ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        var camera = SceneView.lastActiveSceneView.camera;
        var plane = new Plane(camera.transform.forward, FromLocal(_spline.points.Last().Position));
        plane.Raycast(ray, out float distance);
        Vector3 clickPos = ray.origin + ray.direction * distance;
        
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
            Undo.RecordObject(_creator, "Add point to spline");
            _spline.AddPoint(ToLocal(clickPos));
        }
    }

    private void Draw() {
        DrawPoints();
        DrawGuides();
        DrawTailGuides();
        DrawCurve();
    }

    private void DrawPoints() {
        Handles.color = POINTS_COLOR;
        foreach (var point in _spline.points) {
            Vector3 newPos = Handles.FreeMoveHandle(FromLocal(point.Position), POINT_SIZE, Vector3.one, Handles.SphereHandleCap);
            if (newPos != FromLocal(point.Position)) {
                Undo.RecordObject(_creator, "Moving spline point");
                point.Position = ToLocal(newPos);
            }
        }
    }

    private void DrawGuides() {
        Handles.color = GUIDES_COLOR;
        foreach (var point in _spline.points) {
            Vector3 newPos = Handles.FreeMoveHandle(FromLocal(point.guide), GUIDE_SIZE, Vector3.one, Handles.SphereHandleCap);

            if (newPos != FromLocal(point.guide)) {
                Undo.RecordObject(_creator, "Moving spline point guide");
                point.guide = ToLocal(newPos);
            }
            Handles.DrawLine(FromLocal(point.Position), FromLocal(point.guide), GUIDE_LINE_THICKNESS);
        }
    }

    private void DrawTailGuides() {
        Handles.color = TAIL_GUIDES_COLOR;
        foreach (var point in _spline.points) {
            Vector3 newPos = Handles.FreeMoveHandle(FromLocal(point.tailGuide), GUIDE_SIZE, Vector3.one, Handles.SphereHandleCap);
            
            if (newPos != FromLocal(point.tailGuide)) {
                Undo.RecordObject(_creator, "Moving spline point tail guide");
                point.tailGuide = ToLocal(newPos);
            }
            Handles.DrawLine(FromLocal(point.Position), FromLocal(point.tailGuide), GUIDE_LINE_THICKNESS);
        }
    }

    private void DrawCurve() {
        for (int i = 0; i < _spline.count - 1; i++) {
            Handles.DrawBezier(
                FromLocal(_spline.points[i].Position), FromLocal(_spline.points[i+1].Position), 
                FromLocal(_spline.points[i].guide), FromLocal(_spline.points[i+1].tailGuide), 
                CURVE_COLOR, null, CURVE_THICKNESS);
        }
    }

    private Vector3 FromLocal(Vector3 vector) => vector + localPosition;
    private Vector3 ToLocal(Vector3 vector) => vector - localPosition;
}