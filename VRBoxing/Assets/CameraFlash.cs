using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlash : MonoBehaviour
{
    public ParticleSystem flash;

    public Transform[] positions;

    public bool useNPCs;
    public string npcTag;
    float timer = 1;
    // Update is called once per frame

    private void Start()
    {
        if (useNPCs)
        {
            var objs = GameObject.FindGameObjectsWithTag(npcTag);
            positions = new Transform[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                positions[i] = objs[i].transform;
            }
        }
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (flash.isStopped)
        {
            transform.position = positions[Random.Range(0, positions.Length)].position;
            flash.Play();
            timer = Random.Range(1f,100f) / 100;
        }
    }
}
