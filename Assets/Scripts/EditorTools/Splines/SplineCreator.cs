using DefaultNamespace.Interfaces.DataAccessors;
using UnityEngine;

public class SplineCreator : MonoBehaviour, ISplineHolder {
    [SerializeField]
    private Spline _spline;
    public Spline Spline => _spline;

    public void CreateSpline() {
        _spline = new();
    }
}