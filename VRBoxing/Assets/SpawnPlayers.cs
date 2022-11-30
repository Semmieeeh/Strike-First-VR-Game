using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPun
{
    public float minX, maxX, minY, maxY;
    public float yHeight;
    public GameObject playerPrefab;
    private void Start()
    {
        Vector3 randomSpawnPos = PhotonNetwork.CurrentRoom.Players.Count <= 1 ? new Vector3(minX, yHeight, minY) : new Vector3(maxX, yHeight, maxY);

        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
    }
}
