using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Start", order = 10, fileName = "Start_Settings")]
public class StartSettings : ScriptableObject {
    [SerializeField]
    private RoomSettings startingRoom;
    public RoomSettings StartingRoom => startingRoom;

    public void SetStartingRoom(RoomSettings room) {
        startingRoom = room;
    }
}
