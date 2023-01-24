using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform target;
    public GameObject head;
    private Rigidbody rb;
    public float rotDif;
    
    public bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {

        

        rb = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayer == false)
        {
            Position();
        }
        else if (isPlayer == true)
        {
            Rotation();
        }
        
    }
    public void Position()
    {
        Vector3 posDif;
        posDif.z = target.transform.position.z;
        posDif.x = target.transform.position.x;
        posDif.y = gameObject.transform.position.y;
        gameObject.transform.position = posDif;
    }
    public void Rotation()
    {
        //Quaternion myRot = gameObject.transform.rotation;
        //Quaternion headRot = head.transform.rotation;
        //float threshHold = 30f;
        //// 90-0 < 30
        //// 90+0 >-30
        //if(headRot.y + myRot.y > threshHold)
        //{
        //    myRot.y = headRot.y;
        //    myRot.x = gameObject.transform.rotation.x;
        //    myRot.z = gameObject.transform.rotation.z;

        //    gameObject.transform.rotation = myRot;
        //    rotDif = headRot.y + myRot.y;
        //}
        //else if (headRot.y + myRot.y > -threshHold)
        //{

        //    myRot.y = headRot.y;
        //    myRot.x = gameObject.transform.rotation.x;
        //    myRot.z = gameObject.transform.rotation.z;

        //    gameObject.transform.rotation = myRot;
        //    rotDif = headRot.y + myRot.y;
        //}


        Quaternion myRot = gameObject.transform.rotation;
        Quaternion headRot = head.transform.rotation;
        float threshold = 45f;
        float angle = Quaternion.Angle(myRot, headRot);
        rotDif = angle;
        if(angle > threshold)
        {
            Debug.Log("Rotating");
            Quaternion newRot = Quaternion.RotateTowards(transform.rotation, head.transform.rotation, 5*Time.deltaTime);
            myRot.y = newRot.y;
            transform.rotation = myRot;
        }
        
    }
}
