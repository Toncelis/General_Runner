using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Start", order = 10, fileName = "Start_Settings")]
public class StartSettings : ScriptableObject {
    [SerializeField] private RoomSettings StartingRoom;
    public RoomSettings startingRoom => StartingRoom;

    public void SetStartingRoom(RoomSettings room) {
        StartingRoom = room;
    }
}
