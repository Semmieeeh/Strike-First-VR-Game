using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject serverDataPrefab;
    public Transform canvas;
    public List<ServerData> activeServers = new();

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("ewgfaewgaesgtaesg");
        for (int i = 0; i < activeServers.Count; i++)
        {
            Destroy(activeServers[i].gameObject);
        }
        activeServers.Clear();

        for (int i = 0; i < roomList.Count; i++)
        {
            var obj = Instantiate(serverDataPrefab, canvas).GetComponent<ServerData>();
            activeServers.Add(obj);

            obj.roomName = roomList[i].Name;
        }
    }
}
