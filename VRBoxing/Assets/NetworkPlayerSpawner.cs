using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon. Pun;
public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;
    public GameObject networkPlayerPrefab;

    public Transform networkPlayerParent;

    public void Start()
    {
        spawnedPlayerPrefab = PhotonNetwork.Instantiate(networkPlayerPrefab.name, transform.position, transform.rotation);
        print("spawn 1");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnedPlayerPrefab = PhotonNetwork.Instantiate(networkPlayerPrefab.name, transform.position, transform.rotation);
        print("spawn2");
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}