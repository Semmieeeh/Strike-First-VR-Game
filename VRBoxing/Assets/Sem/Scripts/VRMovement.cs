using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;


public class VRMovement : MonoBehaviour
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
    public GrabObjects[] grab;

    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }
    

    public void Look(InputAction.CallbackContext context)
    {
        Debug.Log("Looking");

    }
    public void StopMove(InputAction.CallbackContext context)
    {
        
    }
    
    
    
    public void ZaWarudo(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
            
        }
    }
    public void Disable()
    {
        grab[0].multiplier = false;
        grab[1].multiplier = false;
    }

    public void RemoveBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Double damage !");
            
            grab[1].multiplier = true;
            grab[0].multiplier = true;
            Invoke(nameof(Disable), 3f);
        }
    }
    private void Update()
    {
        

    }
    public void Movement(InputAction.CallbackContext context)
    {
        
        UnityEngine.XR.InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out inputAxis);
        speed += 8 * Time.deltaTime;
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
