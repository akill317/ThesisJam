using UnityEngine;
using System.Collections;

public class NetworkManager : Photon.MonoBehaviour {

    public Transform SpawnPoints;

    // Use this for initialization
    void Start() {
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.offlineMode = false;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings("Ver001");
    }

    public virtual void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby(). This client is connected and does get a room-list, which gets stored as PhotonNetwork.GetRoomList(). This script now calls: PhotonNetwork.JoinRandomRoom();");
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
        Collider2D col;
        Transform spawnPoint;
        do {
            spawnPoint = SpawnPoints.GetChild(Random.Range(0, SpawnPoints.childCount));
            col = Physics2D.OverlapPoint(spawnPoint.position, LayerMask.GetMask(new string[] { "Player" }));
        } while (col != null);

        GameObject player = PhotonNetwork.Instantiate("Hero", spawnPoint.position, Quaternion.identity, 0);
        player.GetComponent<PlayerMovement>().SetFacingDirection(spawnPoint.right);
    }

    public void OnPhotonCreateRoomFailed() {
    }

}
