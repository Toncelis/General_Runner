using System;
using System.Collections.Generic;
using DataTypes;
using Sirenix.OdinInspector;
using UnityEngine;
using World.Model;
using World.RoomBehaviour;

[CreateAssetMenu(menuName = "Configs/Room", order = 0, fileName = "Room_NAME")]
public class RoomSettings : ScriptableObject {
    [SerializeField] private List<RoomChangingRule> NextRoomVariants;
    [SerializeField] private Sprite RoomSprite;
    [SerializeField] private RoomScriptableBehaviour Behaviour;

    [PropertySpace(4), Title("Generation settings")]
    [SerializeField] private TileConfig StartRoadTile;

    public List<RoomChangingRule> nextRoomVariants => NextRoomVariants;
    public Sprite roomSprite => RoomSprite;

    public TileConfig startRoadTile => StartRoadTile;
    public RoomScriptableBehaviour behaviour => Behaviour;
}

[Serializable]
public class RoomChangingRule {
    [SerializeField]
    private RoomSettings NextRoom;
    [SerializeField]
    private List<RoomChangeRequirement> Requirements;
    [SerializeField]
    private TileConfig ChangerTile;

    public RoomSettings nextRoom => NextRoom;
    public List<RoomChangeRequirement> requirements => Requirements;
    public TileConfig changerTile => ChangerTile;
}

[Serializable]
public class RoomChangeRequirement {
    [SerializeField, HorizontalGroup]
    private CollectableTypesEnum CollectableType;
    [SerializeField, HorizontalGroup, LabelWidth(40)]
    private int RequiredAmount;

    public CollectableTypesEnum collectableType => CollectableType;
    public int requiredAmount => RequiredAmount;
}