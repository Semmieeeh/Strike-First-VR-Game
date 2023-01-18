using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysics : MonoBehaviour
{
    public Transform target;
    public GameObject handToFollow;
    private Rigidbody rb;
    public bool isLeft;

    // Start is called before the first frame update
    void Start()
    {
        
        handToFollow.GetComponent<GrabObjects>().physicsHand = gameObject;
        
        rb = GetComponent<Rigidbody>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotationDif = target.rotation;


        gameObject.transform.rotation = rotationDif;

        rb.velocity = (target.position - transform.position) / Time.deltaTime;
    }
}
