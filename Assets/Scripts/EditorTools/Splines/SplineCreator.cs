using UnityEngine;

public class SplineCreator : MonoBehaviour {
    [SerializeField]
    private Spline _spline;
    public Spline Spline => _spline;

    public void CreateSpline() {
        _spline = new();
    }
}