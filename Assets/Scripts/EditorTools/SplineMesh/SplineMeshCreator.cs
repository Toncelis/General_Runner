using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMeshCreator : MonoBehaviour {
    public SplineCreator splineCreator;
    public Material roadMaterial;
    public float density;

    public void GenerateMesh() {
        var spline = splineCreator.Spline;
        if (spline.count < 2) {
            return;
        }
        
    }
    
}
