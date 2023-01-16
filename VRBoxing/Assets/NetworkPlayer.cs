using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
public class NetworkPlayer : MonoBehaviour
{
    // Reference to the PhotonView component.
    public PhotonView photonView;
    public PhotonView leftHandPhotonView;
    public PhotonView rightHandPhotonView;

    // References to the head and hand transforms.
    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    // Reference to the local player's camera and hand transforms.
    public Transform localCameraTransform;
    public Transform localLeftHandTransform;
    public Transform localRightHandTransform;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            // Find Local Transforms
            localCameraTransform = Camera.main.transform;
            localLeftHandTransform = GameObject.Find("LeftHand Controller").transform;
            localRightHandTransform = GameObject.Find("RightHand Controller").transform;

            // Set the head and hand transforms to the local player's camera and hand transforms.
            headTransform.position = localCameraTransform.position;
            headTransform.rotation = localCameraTransform.rotation;
            leftHandTransform.position = localLeftHandTransform.position;
            leftHandTransform.rotation = localLeftHandTransform.rotation;
            rightHandTransform.position = localRightHandTransform.position;
            rightHandTransform.rotation = localRightHandTransform.rotation;

            leftHandTransform.gameObject.SetActive(false);
            rightHandTransform.gameObject.SetActive(false);
            headTransform.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            // This is the local player's network player.
            // Synchronize the head and hand transforms with the local player's camera and hand transforms.
            photonView.RPC(nameof(MapHeadPosition), RpcTarget.Others, localCameraTransform.position, localCameraTransform.rotation);
            leftHandPhotonView.RPC(nameof(MapLeftHandPosition), RpcTarget.Others,  localLeftHandTransform.position, localLeftHandTransform.rotation);
            rightHandPhotonView.RPC(nameof(MapRightHandPosition), RpcTarget.Others, localRightHandTransform.position, localRightHandTransform.rotation);
        }
    }

    [PunRPC]
    void MapHeadPosition(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine) return;

        headTransform.position = position;
        headTransform.rotation = rotation;
    }
    [PunRPC]
    void MapLeftHandPosition(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine) return;

        leftHandTransform.position = position;
        leftHandTransform.rotation = rotation;
    }
    [PunRPC]
    void MapRightHandPosition(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine) return;

        rightHandTransform.position = position;
        rightHandTransform.rotation = rotation;
    }

    //public Transform networkHead;
    //public Transform xrHead;
    //public Transform networkLeftHand;
    //public Transform xrLeftHand;
    //public Transform networkRightHand;
    //public Transform xrRightHand;
    //private PhotonView photonView;

    //public Transform parent;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    photonView = GetComponent<PhotonView>();
    //    parent = GameObject.Find("XR Origin").transform;

    //    if (photonView.IsMine)
    //    {
    //        xrHead = Camera.main.transform;
    //        xrLeftHand = GameObject.Find("LeftHand Controller").transform;
    //        xrRightHand = GameObject.Find("RightHand Controller").transform;
    //    }
    //}
    //// Update is called once per frame
    //void Update()
    //{
    //    if (photonView.IsMine)
    //    {
    //        networkHead.gameObject.SetActive(false);
    //        networkRightHand.gameObject.SetActive(false);
    //        networkLeftHand.gameObject.SetActive(false);

    //        photonView.RPC(nameof(MapPosition),RpcTarget.Others,networkHead, xrHead.position,xrHead.rotation );
    //        photonView.RPC(nameof(MapPosition), RpcTarget.Others, networkLeftHand, xrLeftHand.position, xrLeftHand.rotation);
    //        photonView.RPC(nameof(MapPosition), RpcTarget.Others, networkRightHand, xrRightHand.position,xrRightHand.rotation);
    //    }
    //}

    //[PunRPC]
    //void MapPosition(Transform target, Vector3 position, Quaternion rotation)
    //{ 
    //    target.position = position;
    //    target.rotation = rotation;
    //}
}
