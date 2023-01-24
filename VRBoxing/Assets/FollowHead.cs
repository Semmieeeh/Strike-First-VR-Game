using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform target;
    public GameObject head;
    private Rigidbody rb;
    public bool isLeft;

    // Start is called before the first frame update
    void Start()
    {

        

        rb = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        Position();
        Rotation();
        
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
        Quaternion headRot = target.rotation;
        if (headRot.z - gameObject.transform.rotation.z < -30f || headRot.z- gameObject.transform.rotation.z > 30f)
        {
            headRot.z = target.rotation.z - 30f;
            headRot.y = gameObject.transform.rotation.y;
            headRot.x = gameObject.transform.rotation.x;
            gameObject.transform.rotation = headRot;
        }
    }
}
