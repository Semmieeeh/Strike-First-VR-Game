using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Server : MonoBehaviourPunCallbacks
{
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player entered the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    private void Update()
    {
        if (MyPlayer == null)
        {
            MyPlayer = PhotonNetwork.LocalPlayer;
        }
        if (OtherPlayer == null)
        {
            OtherPlayer = PhotonNetwork.PlayerListOthers[0];
        }
        print(MyPlayer);
        print(OtherPlayer);
        ManageMyPlayer();
    }

    public static Player MyPlayer;
    public static Player OtherPlayer;

    void ManageMyPlayer()
    {
        Hashtable properties = MyPlayer.CustomProperties;
        if (properties.ContainsKey("DMGA"))
        {
            if (!(bool)properties["DMGA"])
            {
                print("You took" + (float)properties["DMG"] + " damage");
                properties["DMGA"] = true;
            }
        }

        MyPlayer.SetCustomProperties(properties);
    }
    public static void DamageEnemy(float damage)
    {
        Hashtable properties = OtherPlayer.CustomProperties;
        if (!properties.ContainsKey("DMG"))
        {
            properties.Add("DMG", damage);
            //{ "DMG" , 35 }
        }
        else
        {
            properties["DMG"] = damage;
        }
        if (!properties.ContainsKey("DMGA"))
        {
            properties.Add("DMGA", false);
        }
        else
        {
            properties["DMGA"] = false;
        }

        OtherPlayer.SetCustomProperties(properties);
    }
}
