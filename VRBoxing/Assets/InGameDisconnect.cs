using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDisconnect : MonoBehaviour
{
    public float rotateTreshold;

    Vector3 defaultScale;

    public bool disconnected;

    private void Start()
    {
        defaultScale = transform.localScale;
    }
    void Update()
    {
        if(transform.parent.rotation.z <= rotateTreshold) // de disconnect menu kan aan
        {
            transform.localScale = defaultScale;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }

        if (disconnected)
        {
            transform.root.position = new Vector3(0, -100, 0);
        }

    }

    public void Disconnect()
    {
        disconnected = true;
    }
}
