using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon. Pun;
using Photon.Realtime;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;
    public GameObject networkPlayerPrefab;
    public GameObject networkPlayer;
    public GameObject audioShit;


    public void Start()
    {
        audioShit = PhotonNetwork.Instantiate(audioShit.name, transform.position, Quaternion.identity);
        spawnedPlayerPrefab = PhotonNetwork.Instantiate(networkPlayerPrefab.name, transform.position, transform.rotation);
        print("spawn 1");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        audioShit = PhotonNetwork.Instantiate(audioShit.name, transform.position, Quaternion.identity);
        spawnedPlayerPrefab = PhotonNetwork.Instantiate(networkPlayerPrefab.name, transform.position, transform.rotation);
        print("spawn2");
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
        

    }

    
}