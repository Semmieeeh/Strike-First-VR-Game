using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlash : MonoBehaviour
{
    public ParticleSystem flash;

    public Transform[] positions;
    // Update is called once per frame
    void Update()
    {
        if (flash.isStopped)
        {
            transform.position = positions[Random.Range(0, positions.Length)].position;
            flash.Play();
        }
    }
}
