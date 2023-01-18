using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;

public class CubeSpawners : MonoBehaviourPunCallbacks
{
    public GameObject cube;
    public int amount;

    async void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient) {
            for (int i = 0; i < amount; i++)
            {
                Vector3 pos = new(Random.Range(-5, 5), Random.Range(5, 7), Random.Range(-5, 5));

                PhotonNetwork.Instantiate(cube.name, pos, Quaternion.identity);

                await Task.Yield();
            }
        }

        for(int i = 0; i < amount; i++)
        {
            Vector3 pos = new(Random.Range(-5, 5), Random.Range(5, 7), Random.Range(-5, 5));
            Instantiate(cube, pos, Quaternion.identity);

        }
    }
   
}
