using DataTypes;
using UnityEngine;
using World.Model;

[CreateAssetMenu(menuName = "Settings/SurroundingsGeneration", fileName = "SurroundingsGenerationSettings")]
public class SurroundingsGenerationSettings : ScriptableObject {
    public Vector2 MainDirection;
    public float RoadWidth;
    public float MaxSurroundingWidth;
    public float MinSpaceFilling;
    public float MaxSpaceFilling;

    public GameObject RoadTile;
    public WeightedList<LandscapeObject> Objects;
    public WeightedList<TrackObject> TrackObjects;

    public LandscapeObject GetObject() {
        return Objects.GetRandomObject();
    }

    public TrackObject GetTrackObject() {
        return TrackObjects.GetRandomObject();
    }

}
