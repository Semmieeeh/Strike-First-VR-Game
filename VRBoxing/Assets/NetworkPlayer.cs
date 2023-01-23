using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.UI;
public class NetworkPlayer : MonoBehaviour
{
    UniversalHealthBar healthBar;
    GrabObjects grab;
    public PlayerMaterialManager materialManager;
    // Reference to the PhotonView component.
    public PhotonView photonView;

    // References to the head and hand transforms.
    public Transform headTransform;
    public Transform shotgunTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform bodyTransform;

    // Reference to the local player's camera and hand transforms.
    public Transform localCameraTransform;
    public Transform localLeftHandTransform;
    public Transform localRightHandTransform;
    public Transform LocalShotgunTransform;
    public Transform localBodyTransform;

    public float playerHealth;

    public Slider healthSlider; 
    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("Player").GetComponent<UniversalHealthBar>();
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            // Find Local Transforms

            localCameraTransform = Camera.main.transform;
            localLeftHandTransform = GameObject.Find("LeftHand Controller").transform;
            localRightHandTransform = GameObject.Find("RightHand Controller").transform;
            LocalShotgunTransform = GameObject.Find("Snotgun").transform;
            localBodyTransform = GameObject.Find("Body Controller").transform;
            grab = localRightHandTransform.GetComponent<GrabObjects>();


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
            bodyTransform.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            // This is the local player's network player.
            // Synchronize the head and hand transforms with the local player's camera and hand transforms.
            playerHealth = healthBar.health;

            photonView.RPC(nameof(MapHeadPosition), RpcTarget.Others, localCameraTransform.position, localCameraTransform.rotation);
            photonView.RPC(nameof(MapLeftHandPosition), RpcTarget.Others,  localLeftHandTransform.position, localLeftHandTransform.rotation);
            photonView.RPC(nameof(MapRightHandPosition), RpcTarget.Others, localRightHandTransform.position, localRightHandTransform.rotation);
            photonView.RPC(nameof(MapBodyPosition), RpcTarget.Others, localBodyTransform.position, localBodyTransform.rotation);
            photonView.RPC(nameof(MapShotgunPosition), RpcTarget.Others, localRightHandTransform.position, localRightHandTransform.localEulerAngles);
            photonView.RPC(nameof(SetSliderValue), RpcTarget.Others, Mathf.InverseLerp(0, 100, playerHealth));
        }
        else
        {
            var props = Server.OtherPlayer.CustomProperties;

            materialManager.damageLevel = (int)props[Server.kDamageLevel];
            materialManager.glovesColorIndex = (int)props[Server.kGlovesColor];
            materialManager.skinColorIndex = (int)props[Server.kSkinColor];
            materialManager.hairCutIndex = (int)props[Server.kHairCut];
            materialManager.hairCutColorIndex = (int)props[Server.kHairCutColor];
            materialManager.shortsColorIndex = (int)props[Server.kShortsColor];
        }
    }

    [PunRPC]
    void SetSliderValue(float value)
    {
        healthSlider.value = value;
    }

    [PunRPC]
    void MapBodyPosition(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine) return;

        bodyTransform.position = position;
        bodyTransform.rotation = rotation;
    }

    [PunRPC]
    void MapHeadPosition(Vector3 position, Quaternion rotation)
    {
        if (photonView.IsMine) return;

        headTransform.position = position;
        headTransform.rotation = rotation;
    }
    [PunRPC]
    void MapShotgunPosition(Vector3 position, Vector3 rotation)
    {
        if (photonView.IsMine) return;
        position.z += 0.00406f;
        shotgunTransform.position = position;
        
        
        Vector3 newShotgunRot;
        
        newShotgunRot.y = 180;
        rotation.y += newShotgunRot.y;
        shotgunTransform.localEulerAngles = rotation;

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
