using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
    public float rotateTime;
    public float rotateSpeed;
    float currentTimer;

    // Start is called before the first frame update
    void Start()
    {
        currentTimer = rotateTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;
        if(currentTimer <= 0)
        {
            transform.Rotate(rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime);
        }
    }
}
