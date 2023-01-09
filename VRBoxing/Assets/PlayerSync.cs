using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviour
{
    // Reference to the PhotonView component.
    public PhotonView photonView;

    // Stores the past and future positions of the player controller.
    Vector3 previousPosition;
    Vector3 currentPosition;
    Vector3 nextPosition;

    // Stores the time at which the past and future positions were received.
    float previousTime;
    float currentTime;
    float nextTime;

    void Start()
    {
        // Initialize the past and future positions of the player controller.
        previousPosition = transform.position;
        currentPosition = transform.position;
        nextPosition = transform.position;

        // Initialize the time at which the past and future positions were received.
        previousTime = Time.time;
        currentTime = Time.time;
        nextTime = Time.time;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            // Synchronize the player controller's transform across the network.
            photonView.RPC(nameof(UpdatePlayerTransform), RpcTarget.Others, transform.position, Time.time);
        }
        else
        {
            // This is a remote player's player controller.
            // Smooth out movement over the network using interpolation.
            float lerpTime = (Time.time - previousTime) / (nextTime - previousTime);
            transform.position = Vector3.Lerp(previousPosition, nextPosition, lerpTime);
        }
    }

    // This function is called on all clients when the transform of the player controller is updated over the network.
    [PunRPC]
    void UpdatePlayerTransform(Vector3 position, float time)
    {
        // Store the past position of the player controller.
        previousPosition = transform.position;
        previousTime = Time.time;

        // Update the current and future positions of the player controller.
        currentPosition = position;
        currentTime = time;
        nextPosition = position;
        nextTime = time;
    }
}

//public class PlayerSync : MonoBehaviourPun
//{
//    // The transform component of the player's body
//    public Transform bodyTransform;

//    // The transform component of the player's left hand
//    public Transform leftHandTransform;

//    // The transform component of the player's right hand
//    public Transform rightHandTransform;


//    [Space(8)]
//    public Transform networkBodyTransform, networkLeftHandTransform, networkRightHandTransform;

//    // The current position of the player's body
//    private Vector3 bodyPosition;

//    // The current rotation of the player's body
//    private Quaternion bodyRotation;

//    // The current position of the player's left hand
//    private Vector3 leftHandPosition;

//    // The current rotation of the player's left hand
//    private Quaternion leftHandRotation;

//    // The current position of the player's right hand
//    private Vector3 rightHandPosition;

//    // The current rotation of the player's right hand
//    private Quaternion rightHandRotation;

//    public GameObject playerNetwork;
//    public GameObject[] thisPlayer;

//    void Update()
//    {
//        // If this script is not being executed on the master client, do not update the player's body and hand transforms
//        if (photonView.IsMine == false)
//        {
//            playerNetwork.SetActive(true);

//            foreach (GameObject gameObject in thisPlayer)
//            {
//                gameObject.SetActive(false);
//            }
//            return;
//        }

//        // Get the current position and rotation of the player's body and hands

//        bodyPosition = bodyTransform.position;
//        bodyRotation = bodyTransform.rotation;
//        leftHandPosition = leftHandTransform.position;
//        leftHandRotation = leftHandTransform.rotation;
//        rightHandPosition = rightHandTransform.position;
//        rightHandRotation = rightHandTransform.rotation;

//        // Set the position and rotation of the player's body and hands on the Photon server
//        photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, networkBodyTransform, bodyPosition, bodyRotation);
//        photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, networkLeftHandTransform, leftHandPosition, leftHandRotation);
//        photonView.RPC(nameof(SyncBodyPart), RpcTarget.Others, networkRightHandTransform, rightHandPosition, rightHandRotation);
//    }


//    //RPC method to synchronize objects across the photon network
//    [PunRPC]
//    public void SyncBodyPart(Transform bodyPart, Vector3 pos, Quaternion rot)
//    {
//        bodyPart.SetPositionAndRotation(pos, rot);
//    }
//}