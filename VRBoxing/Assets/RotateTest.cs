using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RotateTest : MonoBehaviour
{
    public float rotateTime;
    public float rotateSpeed;
    float currentTimer;
    float fovTimer = 60;

    float x, y, z;

    float targetFOV;
    public float fovSpeed;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        currentTimer = rotateTime;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;
        if(currentTimer <= 0)
        {
            currentTimer = rotateTime;
            x = Random.Range(-3, 3);
            y = Random.Range(-3, 3);
            z = Random.Range(-3, 3);
        }


        transform.Rotate(new Vector3(x * Mathf.Sin(Time.time -3), y * Mathf.Sin(Time.time - 1f * 0.8f), Mathf.Sin(Time.timeScale * 1.3f)) * rotateSpeed * Time.deltaTime);

        fovTimer -= Time.deltaTime;

        if (fovTimer <= 0)
        {
            fovTimer = Random.Range(0.3f, 1);

            targetFOV = Random.Range(-1, 3);
        }

        XRDevice.fovZoomFactor = Mathf.MoveTowards(XRDevice.fovZoomFactor, targetFOV, fovSpeed * Time.deltaTime);
        print(XRDevice.fovZoomFactor);
    }
}
