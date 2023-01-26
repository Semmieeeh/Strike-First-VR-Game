using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Unity.VisualScripting;
using OVR;
using static UnityEngine.ParticleSystem;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;


public class VRMovement : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    public bool isHardened;
    public InputMaster input;
    Vector2 inputAxis;
    CharacterController characterController;
    public XRNode inputSource;
    public XROrigin rig;
    public LayerMask ground;
    public float speed,maxSpeed;
    public float gravity, fallingspeed;
    public float cameraOffset;
    public UniversalHealthBar healthBar;
    public float healResetTime;
    public bool canHeal;
    public GameObject cameraOrigin;
    public GameObject cam;
    public bool shotgunActive;
    public GameObject shotgun;
    public Animator anim;
    public float damage;
    public AudioManager sound;
    public ParticleSystem particle;
    public PhotonView pv;
    public GameObject gunTip;
    public RaycastHit hit;
    public AudioManager aud;
    private GameObject audioCheck;
    public float disconnectCooldown;
    public float disconnectButtonPresses;
    public Slider slider;
    public GrabObjects grab;

    void Start()
    {
        PhotonNetwork.JoinRandomRoom();
        audioCheck = GameObject.Find("AudioOn");
        if(audioCheck.tag != "NPC")
        {
            aud.PlayAudio(7, 1, 1);
            
        }
        rb = gameObject.GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
        healthBar = GetComponentInParent<UniversalHealthBar>();
        healResetTime = 10f;
        pv = GetComponent<PhotonView>();
    }
    

    public void Look(InputAction.CallbackContext context)
    {
        Debug.Log("Looking");

    }
    public void ResetHeight(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            cam.transform.position = cameraOrigin.transform.position;
            Debug.Log("Resetting Cam");
        }
    }
    public void ActivateShotgun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Shotgun");
        }
        if(context.started)
        {            
            shotgunActive = true;
            if (pv.IsMine)
            {
                Server.ShotgunAppear();
            }
            
        }
        if (context.canceled)
        {
            shotgunActive = false;
            if (pv.IsMine)
            {
                Server.ShotgunDisappear();
            }
        }
        
        
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (shotgunActive == true)
            {
                Debug.Log("Shot!");
                Instantiate(particle, gunTip.transform.position, gunTip.transform.rotation);
                sound.PlayAudio(4, 0.8f, 1.2f);
                if (Physics.Raycast(gunTip.transform.position, gunTip.transform.forward, out hit, 10f))
                {
                    if (hit.transform.gameObject.tag == "Head" || hit.transform.gameObject.tag == "Enemy" || hit.transform.gameObject.tag == "RightFist" || hit.transform.gameObject.tag == "LeftFist")
                    {
                        Server.DamageEnemy(1000);
                    }
                }
            }
        }
    }
    public void DisconnectInGame(InputAction.CallbackContext context)
    {
        if (context.canceled && grab.hardened == true )
        {
            
            disconnectButtonPresses += 0.5f;
            if (disconnectButtonPresses == 1)
            {
                disconnectCooldown = 3;
            }
            if (disconnectButtonPresses > 4)
            {
                
                //PhotonNetwork.Disconnect();
                Disconnect();
            }
        }
    }
    public void Disconnect()
    {
        
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LeaveLobby();
        
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        
    }
    public void Heal(InputAction.CallbackContext context)
    {
        if (context.started && canHeal)
        {
            healthBar.Heal();
        }
    }
    
    private void Update()
    {
        if (healResetTime > 0)
        {
            canHeal = true;
        }
        else
        {
            canHeal = false;
        }
        
        anim.SetBool("Shotgun", shotgunActive);
        disconnectCooldown -= 1 * Time.deltaTime;
        if(disconnectCooldown < 0)
        {
            disconnectButtonPresses = 0;
        }
        slider.value = disconnectButtonPresses;
    }
    public void Movement(InputAction.CallbackContext context)
    {
        if (!Server.CanMove())
        { 
            speed = 0;
            return;
        }
        
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out inputAxis);
        speed += 3 * Time.deltaTime;
        if(speed > maxSpeed)
        {
            speed = maxSpeed;
        }

        if (context.canceled)
        {
            speed = 0;
        }
    }
    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        Quaternion headDirection = Quaternion.Euler(x: 0, rig.Camera.transform.eulerAngles.y, z: 0);
        Vector3 direction = headDirection* new Vector3(inputAxis.x, y: 0, inputAxis.y);
        characterController.Move(direction * speed * Time.deltaTime);

        //bool grounded = CheckIfGrounded();

        
    }
    public bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(characterController.center);
        float rayLenth = characterController.center.y + cameraOffset + 0.001f;
        bool hasHit = Physics.SphereCast(rayStart, characterController.radius, Vector3.down, out RaycastHit hit, rayLenth, ground);
        return hasHit;
    }
    void CapsuleFollowHeadset()
    {
        float offset = 0.2f;
        characterController.height = rig.CameraInOriginSpaceHeight+ offset;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        characterController.center = new Vector3(capsuleCenter.x, characterController.height / 1.5f + characterController.skinWidth, capsuleCenter.z);
    }
    
    
}
