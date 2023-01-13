using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        startY = pos.y;
    }

    //adjust this to change speed
    public float speed;
    //adjust this to change how high it goes
    float startY;
    public float height;
    public bool loop = true;
    Vector3 pos;

    void Update()
    {
        float newY = startY + height * ((Mathf.Sin(Time.time * speed) + 1) / 2);
        transform.position = new Vector3(pos.x, newY, pos.z);
    }
}
