using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfHitScript : MonoBehaviour
{
    private bool holdingStick;
    public GameObject rightHand;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            transform.position = rightHand.transform.position - new Vector3(0, 0, 0.3f);
            transform.rotation = rightHand.transform.rotation;
            holdingStick = true;
            rightHand.SetActive(false);
            gameObject.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.E) && holdingStick)
        {
            gameObject.SetActive(false);
            rightHand.SetActive(true);
        }

    }
}
