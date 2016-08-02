using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {

    public static NetworkManager Instance;
    public Transform SpawnPoints;

    public delegate void EnterRoomEventHandler();
    public event EnterRoomEventHandler OnEnteredRoom;

    public delegate void PlayerLeftRoomEventHandler(int playerId);
    public event PlayerLeftRoomEventHandler OnPlayerLeftRoom;

    void Awake() {
        Instance = this;
    }

    // Use this for initialization
    void Start() {
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.offlineMode = false;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings("Ver001");
    }

    public virtual void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        //PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        //PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 4 }, null);
    }

    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
        if(OnEnteredRoom != null) {
            OnEnteredRoom();
        }
    }

    void OnPhotonCreateRoomFailed(object[] codeAndMsg) { // codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg. 
        Debug.Log(codeAndMsg[1]);
    }

    void OnPhotonJoinRoomFailed(object[] codeAndMsg) { // codeAndMsg[0] is short ErrorCode. codeAndMsg[1] is string debug msg. 
        Debug.Log(codeAndMsg[1]);
    }

    //public virtual void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
    //    if(OnPlayerLeftRoom != null) {
    //        OnPlayerLeftRoom(otherPlayer.ID);
    //    }
    //}
}
