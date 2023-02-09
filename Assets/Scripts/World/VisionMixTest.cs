using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisionMixTest : MonoBehaviour {
    public Image viewImage;
    private Material ViewRenderMaterial;
    public float CameraMixingFrequency;
    public float CameraMixingAmplitude;
    private void Start() {
        ViewRenderMaterial = viewImage.material;
        Debug.Log(ViewRenderMaterial.HasProperty("_CameraBalance"));
    }
    private void Update() {
        ViewRenderMaterial.SetFloat("_CameraBalance", CameraMixingAmplitude*Mathf.Sin(Time.time * CameraMixingFrequency));
    }
}
