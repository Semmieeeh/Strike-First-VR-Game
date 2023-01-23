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
        Vector3 posDif;
        
        posDif.z = target.transform.position.z;
        posDif.x = target.transform.position.x;
        posDif.y = target.transform.position.y -1f;

        gameObject.transform.position = posDif;
        
    }
}
