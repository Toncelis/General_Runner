using DataTypes;
using UnityEngine;
using World.Model;

[CreateAssetMenu(menuName = "Configs/Room", order = 0 ,fileName = "Room_NAME")]
public class RoomSettings : ScriptableObject {
    public TileConfig StartRoadTile;

    public float MinTrackObjectBlanks;
    public float MaxTrackObjectBlanks;
}
