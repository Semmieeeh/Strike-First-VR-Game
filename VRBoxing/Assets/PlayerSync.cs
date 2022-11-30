using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerSync : MonoBehaviour
{
    public Transform head, leftHand, rightHand;
    public PhotonView photon;
    // Start is called before the first frame update
    void Start()
    {
        photon = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photon.IsMine)
        {
            //head.gameObject.SetActive(false);
            //leftHand.gameObject.SetActive(false);
            //rightHand.gameObject.SetActive(false);


            Pos(head, XRNode.Head);
            Pos(leftHand, XRNode.LeftHand);
            Pos(rightHand, XRNode.RightHand);
        }
    }

    void Pos(Transform target, XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);

        target.position = pos;
        target.rotation = rot;

    }
}
