using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public string RoomName;
    public Text RoomNameEnterAlarm;
    public int PlayerColor;
    public int ReadyPlayerNumber;

    public static GameManager Instance;

    void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        NetworkManager.Instance.OnEnteredRoom += EnteredRoom;
    }

    public void EnterRoomName(InputField inputField) {
        RoomName = inputField.text;
    }

    public void EnterRoom() {
        if (RoomName == "") {
            RoomNameEnterAlarm.text = "Please Enter A Room Name";
            return;
        } else {
            RoomNameEnterAlarm.text = "";
        }
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOption, TypedLobby.Default);
        PhotonNetwork.LoadLevel("Room");
    }

    void EnteredRoom() {
        //PhotonNetwork.LoadLevel("Room");
        Transform playerReadyPanel = GameObject.Find("PlayerReadyPanel").transform;
        int currentPlayerCount = PhotonNetwork.room.playerCount;
        Transform playerBackGround = playerReadyPanel.GetChild(currentPlayerCount - 1);
        PhotonNetwork.Instantiate("HeroInRoom", playerBackGround.position, playerBackGround.rotation, 0);
    }

    void OnLevelWasLoaded(int level) {
        if (level == 2) {
            Transform SpawnPoints = GameObject.Find("SpawnPoints").transform;
            Transform spawnPoint = SpawnPoints.GetChild(Random.Range(0, SpawnPoints.childCount));
            GameObject player = PhotonNetwork.Instantiate("Hero", spawnPoint.position, spawnPoint.rotation, 0);
        }
    }
}
