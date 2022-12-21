using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun
{
    // The transform component of the player's body
    public Transform bodyTransform;

    // The transform component of the player's left hand
    public Transform leftHandTransform;

    // The transform component of the player's right hand
    public Transform rightHandTransform;

    // The current position of the player's body
    private Vector3 bodyPosition;

    // The current rotation of the player's body
    private Quaternion bodyRotation;

    // The current position of the player's left hand
    private Vector3 leftHandPosition;

    // The current rotation of the player's left hand
    private Quaternion leftHandRotation;

    // The current position of the player's right hand
    private Vector3 rightHandPosition;

    // The current rotation of the player's right hand
    private Quaternion rightHandRotation;

    void Update()
    {
        // If this script is not being executed on the master client, do not update the player's body and hand transforms
        if (!photonView.IsMine)
        {
            return;
        }

        // Get the current position and rotation of the player's body and hands
        bodyPosition = bodyTransform.position;
        bodyRotation = bodyTransform.rotation;
        leftHandPosition = leftHandTransform.position;
        leftHandRotation = leftHandTransform.rotation;
        rightHandPosition = rightHandTransform.position;
        rightHandRotation = rightHandTransform.rotation;

        // Set the position and rotation of the player's body and hands on the Photon server
        //photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, bodyTransform, bodyPosition, bodyRotation);
        //photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, leftHandTransform, leftHandPosition, leftHandRotation);
        //photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, rightHandTransform, rightHandPosition, rightHandRotation);
    }


    //RPC method to synchronize objects across the photon network
    [PunRPC]
    public void SyncBodyPart(Transform bodyPart, Vector3 pos, Quaternion rot)
    {
        bodyPart.position = pos;
        bodyPart.rotation = rot;
    }
}