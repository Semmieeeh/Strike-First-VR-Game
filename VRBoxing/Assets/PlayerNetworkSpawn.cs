using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworkSpawn : MonoBehaviourPunCallbacks
{
    public GameObject spawnPrefab;

    public void Start()
    {
        spawnPrefab = PhotonNetwork.Instantiate(spawnPrefab.name, transform.position, transform.rotation);
        print("SpawnOld 2");
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        spawnPrefab = PhotonNetwork.Instantiate(spawnPrefab.name, transform.position, transform.rotation);
        print("SpawnOld 1");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnPrefab);
    }

}
