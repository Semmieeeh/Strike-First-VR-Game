using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using Photon.Pun.Demo.Cockpit;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject serverDataPrefab;
    public Transform canvas;
    public List<ServerData> activeServers = new();

    public RoomCreator roomCreator;

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.ConnectUsingSettings();
    }

    public async void Disconnect()
    {
        GameManager.MainMenu.Disconnect();

        await Task.Delay(1500);
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("disconnected!");
        GameManager.MainMenu.OnExitStart();
        PhotonNetwork.LeaveLobby();
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

            var room = roomList[i];

            int mapIndex = (int)room.CustomProperties[ServerData.mapIndexProperty];
            var roomSprite = roomCreator.mapSprites[mapIndex-1];

            obj.Initialize(room.Name,roomSprite ,mapIndex);
        }
    }
}
