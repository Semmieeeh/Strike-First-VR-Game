using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsManager : MonoBehaviour
{
    public float minY;

    private void Start()
    {
        InvokeRepeating(nameof(CheckBounds), 1, 1);
    }
    void CheckBounds()
    {
        foreach (Transform transform in Resources.FindObjectsOfTypeAll(typeof(Transform)))
        {
            if(transform.position.y <= minY)
            {
                transform.position = new(Random.Range(-5, 5), Random.Range(5, 7), Random.Range(-5, 5));

                Rigidbody body = transform.GetComponent<Rigidbody>();
                if(body != null)
                {
                    body.velocity = Vector3.zero;
                }
            }
        }
    }
}
