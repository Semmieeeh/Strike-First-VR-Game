using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class RegionSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }
    public void OnValueChanged()
    {
        var value = dropdown.value;

        string region = value switch
        {
            1 => "eu",
            2 => "us",
            3 => "sa",
            4 => "asia",
            5 => "au",
            6 => "za",
            _ => "eu"
        };

        PhotonNetwork.ConnectToRegion(region);
    }
}
