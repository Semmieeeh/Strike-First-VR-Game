using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WinningSpotLight : MonoBehaviour
{
    public Light spotlight;

    public InGameDisplay InGameDisplay;
    public Color red, cyan;
    // Start is called before the first frame update
    void Start()
    {
        InGameDisplay = GameObject.FindGameObjectWithTag("InGameDisplay").GetComponent<InGameDisplay>();

        spotlight.color = InGameDisplay.FindWinner() == PhotonNetwork.MasterClient ? red : cyan;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
