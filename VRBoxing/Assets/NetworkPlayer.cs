using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
public class NetworkPlayer : MonoBehaviour
{
    public Transform networkHead;
    public Transform networkLeftHand;
    public Transform networkRightHand;
    private PhotonView photonView;

    public Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        parent = GameObject.Find("XR Origin").transform;
    }
    // Update is called once per frame
    void Update()
    {
        //if (photonView.IsMine)
        //{
        //    head.gameObject.SetActive(false);
        //    rightHand.gameObject.SetActive(false);
        //    leftHand.gameObject.SetActive(false);
            
        //    MapPosition(head, );
        //    MapPosition(leftHand, );
        //    MapPosition(rightHand,);
        //}
    }
    void MapPosition(Transform target, Vector3 position, Quaternion rotation)
    { 
        target.position = position;
        target.rotation = rotation;
    }
}
