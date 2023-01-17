using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Server : MonoBehaviourPunCallbacks
{
    public UniversalHealthBar healthBar;

    public const string kDamage = "DMG";
    public const string kHealing = "HEAL";
    public const string kHealingApplied = "HEALA";
    public const string kDamageApplied = "DMGA";
    public const string kHealth = "HP";

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
            if (!myPlayerInitialized && MyPlayer != null)
            {
                var props = MyPlayer.CustomProperties;
                props.Add(kDamage,0);
                props.Add(kDamageApplied, true);
                props.Add(kHealth, 100f);

                MyPlayer.SetCustomProperties(props);
                myPlayerInitialized = true;
            }
        }
        if (OtherPlayer == null)
        {
            OtherPlayer = PhotonNetwork.PlayerListOthers[0];
        }
        print(MyPlayer);
        print(OtherPlayer);

        if(MyPlayer != null)
        ManageMyPlayer();
        
    }

    public static Player MyPlayer;
    bool myPlayerInitialized;
    public static Player OtherPlayer;

    void ManageMyPlayer()
    {
        Hashtable properties = MyPlayer.CustomProperties;
        if (properties.ContainsKey(kDamageApplied))
        {
            if (!(bool)properties[kDamageApplied])
            {
                print("You took" + (float)properties[kDamage] + " damage");
                properties[kDamageApplied] = true;

                float newHealth = (float)properties[kHealth];

                newHealth -= (float)properties[kDamage];

                properties[kHealth] = newHealth;
                healthBar.health = newHealth;
                MyPlayer.SetCustomProperties(properties);
                
                print(properties[kHealth] + " Health");
                print(properties[kDamage] + " Damage");
            }
            if (!(bool)properties[kHealingApplied])
            {
                print("You healed" + (float)properties[kHealing]);
                properties[kHealingApplied] = true;

                float newHealth = (float)properties[kHealth];

                newHealth+=(float)properties[kHealing];

                properties[kHealth] = newHealth;
                healthBar.health = newHealth;
                MyPlayer.SetCustomProperties(properties);

                print(properties[kHealth] + " Health");
                print(properties[kHealing] + " Healing");

            }
        }

       
    }
    public static void DamageEnemy(float damage)
    {
        Hashtable properties = OtherPlayer.CustomProperties;
        if (!properties.ContainsKey(kDamage))
        {
            properties.Add(kDamage, damage);
            //{ "DMG" , 35 }
        }
        else
        {
            properties[kDamage] = damage;
        }
        if (!properties.ContainsKey(kDamageApplied))
        {
            properties.Add(kDamageApplied, false);
        }
        else
        {
            properties[kDamageApplied] = false;
        }

        OtherPlayer.SetCustomProperties(properties);
    }

    /// <summary>
    /// healthToAdd Can be negative to remove health
    /// </summary>
    /// <param name="healthToAdd"></param>
    public static void UpdatePlayerHealth(float healthToAdd)
    {
        var properties = MyPlayer.CustomProperties;
        SetMyPlayerProperty(kHealth, (float)properties[kHealth] + healthToAdd);
    }

    public static void SetMyPlayerProperty(string key, object value)
    {
        var properties = MyPlayer.CustomProperties;
        if (properties.ContainsKey(key))
        {
            properties.Add(key, value);

        }
        properties[key] = value;

        SetPlayerProperties(MyPlayer, properties);
    }

    public static void SetOtherPlayerProperty(string key, object value)
    {
        var properties = OtherPlayer.CustomProperties;
        if (properties.ContainsKey(key))
        {
            properties.Add(key, value);

        }
        properties[key] = value;

        SetPlayerProperties(OtherPlayer, properties);
    }

    public static void SetPlayerProperties(Player player, Hashtable properties)
    {
        player.SetCustomProperties(properties);
    }

    public static void ApplyHealth(float healthToAdd)
    {
        Hashtable properties = OtherPlayer.CustomProperties;
        if (!properties.ContainsKey(kHealing))
        {
            properties.Add(kHealing, healthToAdd);
            //{ "HEAL" , 35 }
        }
        else
        {
            properties[kHealing] = healthToAdd;
        }
        if (!properties.ContainsKey(kHealingApplied))
        {
            properties.Add(kHealingApplied, false);
        }
        else
        {
            properties[kHealingApplied] = false;
        }

        OtherPlayer.SetCustomProperties(properties);
    }
}
