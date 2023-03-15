using System;
using System.Collections.Generic;
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
    private List<RoomChangeRequirement> requirements;
    [SerializeField]
    private TileConfig changerTile;

    public RoomSettings NextRoom => nextRoom;
    public List<RoomChangeRequirement> Requirements => requirements;
    public TileConfig ChangerTile => changerTile;
}

[Serializable]
public class RoomChangeRequirement {
    public CollectableTypes collectable;
    public int amount;
}

public enum CollectableTypes {
    primal,
    secondary_1,
    secondary_2,
    secondary_3,
    secondary_4
}