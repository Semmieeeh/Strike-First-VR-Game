using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUserInterface : MonoBehaviour
{

    public GameObject userInterface;
    public Transform t;
    public bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        t = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive == false && t.rotation.z < 80 || t.rotation.z < -80)
        {
            isActive = true;
            userInterface.SetActive(true);
        }
        else if(isActive == true)
        {
            isActive = false;
            if(isActive == false)
            {
                userInterface.SetActive(false);
            }
        }
        
        
    }
}
