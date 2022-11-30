using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class RemoveIfNotMine : MonoBehaviourPun
{
    PhotonView view;
    public Object[] toRemove;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            for (int i = 0; i < toRemove.Length; i++)
            {
                PhotonNetwork.Destroy(toRemove[i]);
            }

            Debug.Log("Removed Succesfully!");
        }
    }
}
