using System;
using System.Collections.Generic;
using DataTypes;
using UnityEngine;
using World.Model;

[CreateAssetMenu(menuName = "Configs/Room", order = 0, fileName = "Room_NAME")]
public class RoomSettings : ScriptableObject {
    public TileConfig StartRoadTile;

    public float MinTrackObjectBlanks;
    public float MaxTrackObjectBlanks;

    public List<RoomChangingRule> NextRooms;
    public Sprite RoomSprite;
}

[Serializable]
public class RoomChangingRule {
    [SerializeField]
    private RoomSettings nextRoom;
    [SerializeField]
    private List<(CollectableTypes, int)> requirements;
    [SerializeField]
    private TileConfig changerTile;

    public RoomSettings NextRoom => nextRoom;
    public List<(CollectableTypes, int)> Requirements => requirements;
    public TileConfig ChangerTile => changerTile;
}


public enum CollectableTypes {
    primal,
    secondary_1,
    secondary_2,
    secondary_3,
    secondary_4
}