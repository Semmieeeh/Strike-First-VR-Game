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

    public PlayerMaterialManager PlayerMaterialManager;

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.ConnectUsingSettings();
    }

    public async void Disconnect()
    {
        ClearServerList();
        GameManager.MainMenu.Disconnect();

        await Task.Delay(1500);
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        if (!PhotonNetwork.InLobby)
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
        ClearServerList();

        for (int i = 0; i < roomList.Count; i++)
        {
            var room = roomList[i];

            ServerData obj = Instantiate(serverDataPrefab, canvas).GetComponent<ServerData>();

            activeServers.Add(obj);

            //Determine if the serer is full
            bool isRoomFull = room.PlayerCount >= room.MaxPlayers;

            obj.SetServerColor(isRoomFull);
            
            int mapIndex = ReadMapIndexFromName(room.Name);

            var roomSprite = roomCreator.mapSprites[mapIndex-1];

            obj.Initialize(room.Name,roomSprite ,mapIndex);
        }
    }

    public int ReadMapIndexFromName(string name)
    {
        int current = -1;

        char[] maps = new char[]
        {
            '1',
            '2',
            '3'
        };

        for (int i = 0; i < maps.Length; i++)
        {
            if (name.StartsWith(maps[i]))
            {
                current = i + 1;

                break;
            }
        }

        return current;
    }

    public override void OnDisable()
    {
        ClearServerList();
    }

    public void ClearServerList()
    {
        print("");
        for (int i = 0; i < activeServers.Count; i++)
        {
            Destroy(activeServers[i].gameObject);
        }
        activeServers.Clear();
    }
}
