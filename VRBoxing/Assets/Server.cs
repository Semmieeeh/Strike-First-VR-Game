using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Server : MonoBehaviourPunCallbacks
{
    public PlayerMaterialManager localPlayerMaterialManager;
    public UniversalHealthBar healthBar;

    [Space(10)]
    [Header("Particles")]
    public ParticleSystem bloodParticle;
    public ParticleSystem[] HitEffects;

    public float minDamageParticleEffect;
    

    public const string kDamage = "DMG";
    public const string kHealing = "HEAL";
    public const string kHealingApplied = "HEALA";
    public const string kDamageApplied = "DMGA";
    public const string kHealth = "HP";
    public const string kRoundsWon = "ROW";
    public const string kCanFight = "CAF";
    public const string kPlayerPosition = "PLP";

    public const string kDamageLevel = "DMGL";
    public const string kSkinColor = "SKCL";
    public const string kGlovesColor = "GCL";
    public const string kShortsColor = "SCL";
    public const string kHairCut = "HC";
    public const string kHairCutColor = "HCCL";


    public static Server server;

    bool roomInitialized;
    private void Awake()
    {
        server = this;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player entered the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    private void Start()
    {
        Update();
        ShotgunDisappear();
    }
    private void Update()
    {
        if (MyPlayer == null)
        {
            MyPlayer = PhotonNetwork.LocalPlayer;
            if (!myPlayerInitialized && MyPlayer != null)
            {
                var props = MyPlayer.CustomProperties;
                props.Add(kDamage,0f);
                props.Add(kDamageApplied, true);
                props.Add(kHealth, 1000f);
                props.Add(kHealingApplied, true);
                props.Add(kHealing, 0f);
                props.Add(kPlayerPosition, Vector3.zero);
                props.Add(kRoundsWon, 0);

                MyPlayer.SetCustomProperties(props);

                InitializeMyPlayerMaterials();


                myPlayerInitialized = true;
            }
            if (!roomInitialized)
            {
                var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

                roomProperties.Add(kCanFight, false);
                roomInitialized = true;
            }
        }
        if (OtherPlayer == null)
        {
            if (PhotonNetwork.PlayerListOthers.Length >= 1)
                OtherPlayer = PhotonNetwork.PlayerListOthers[0];
        }
        print(MyPlayer);
        print(OtherPlayer);

        if(MyPlayer != null)
        ManageMyPlayer();

        
        CheckForWinner();
    }

    public static Player MyPlayer;
    bool myPlayerInitialized;
    public static Player OtherPlayer;

    void CheckForWinner()
    {
        if (healthBar.health <= 0)
        {
            //RPC shit dat ik verlies
            //RPC shit dat andere wint
            
        }
    }
    void InitializeMyPlayerMaterials()
    {
        var props = MyPlayer.CustomProperties;

        localPlayerMaterialManager.damageLevel = (int)props[kDamageLevel];
        localPlayerMaterialManager.glovesColorIndex = (int)props[kGlovesColor];
        localPlayerMaterialManager.skinColorIndex = (int)props[kSkinColor];
        localPlayerMaterialManager.hairCutIndex = (int)props[kHairCut];
        localPlayerMaterialManager.hairCutColorIndex = (int)props[kHairCutColor];
        localPlayerMaterialManager.shortsColorIndex = (int)props[kShortsColor];
    }
    void UpdateMyPlayerMaterial()
    {
        var props = OtherPlayer.CustomProperties;

        localPlayerMaterialManager.damageLevel = (int)props[kDamageLevel];
    }
    void UpdateOtherPlayerMaterials()
    {
        var props = OtherPlayer.CustomProperties;

        localPlayerMaterialManager.damageLevel = (int)props[kDamageLevel];
    }
    void ManageMyPlayer()
    {
        Hashtable properties = MyPlayer.CustomProperties;
        properties[kPlayerPosition] = localPlayerMaterialManager.transform.position;

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

                if(newHealth <= 0)
                {
                    var roomProps =PhotonNetwork.CurrentRoom.CustomProperties;
                    roomProps[kCanFight] = false;
                }
                localPlayerMaterialManager.damageLevel = DetermineDamageLevel(newHealth);


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
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if ((bool)roomProps[kCanFight] == false) return;

        Hashtable properties = OtherPlayer.CustomProperties;
        if (!properties.ContainsKey(kDamage))
        {
            properties.Add(kDamage, damage);
            //{ "DMG" , 35 }
        }
        else
        {
            var originalDamage = properties[kDamage];
            properties[kDamage] = (float)originalDamage + damage;

            properties[kDamageLevel] = DetermineDamageLevel((float)properties[kHealth]);

           

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

    public static void CheckHitEffect(Vector3 position, float damage)
    {
        if (damage > server.minDamageParticleEffect)
        {
            var particle = server.HitEffects[Random.Range(0, server.HitEffects.Length)];
            Destroy(Instantiate(particle, position, Quaternion.identity),1f);
        }
    }

    /// <summary>
    /// healthToAdd Can be negative to remove health
    /// </summary>
    /// <param name="healthToAdd"></param>
    public static void UpdatePlayerHealth(float healthToAdd)
    {
        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        if ((bool)roomProps[kCanFight] == false) return;

        var properties = MyPlayer.CustomProperties;
        SetMyPlayerProperty(kHealth, (float)properties[kHealth] + healthToAdd);
    }

    /// <summary>
    /// Function used to enable the players movement and damage systems. This function will be executed by the host of the game
    /// </summary>
    public static void SetMovementActive(bool active)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            roomProperties[kCanFight] = active;
        }
    }

    /// <summary>
    /// Used to reset both player's properties to fight again. This function will be executed by the host of the game
    /// </summary>
    public static void ResetPlayersProperties()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ResetPlayerProperties(MyPlayer);
            ResetPlayerProperties(OtherPlayer);
        }
    }

    static void ResetPlayerProperties(Player player)
    {
        var properties = player.CustomProperties;

        properties[kDamage] = 0f;
        properties[kHealing] = 0f;
        properties[kHealingApplied] = true;
        properties[kDamageApplied] = true;
        properties[kHealth] = 1000f;
        properties[kCanFight] = false;
    }

    public static void SetMyPlayerProperty(string key, object value)
    {
        var properties = MyPlayer.CustomProperties;
        if (!properties.ContainsKey(key))
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
        Hashtable properties = MyPlayer.CustomProperties;
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

        MyPlayer.SetCustomProperties(properties);
    }

    public static void ApplyBlood(Vector3 position, Vector3 normal)
    {
        print("Blood Applied");
        DestroyParticle(PhotonNetwork.Instantiate(server.bloodParticle.name, position, Quaternion.FromToRotation(Vector3.forward, normal)));
    }

    async static void DestroyParticle(GameObject particle)
    {
        await System.Threading.Tasks.Task.Delay(3000);
        print("Blood Gone");
        PhotonNetwork.Destroy(particle);
    }

    static int DetermineDamageLevel(float newHealth)
    {
        int lvl = 0;
        if (newHealth < 70) lvl = 1;
        if (newHealth < 30) lvl = 2;
        return lvl;
    }

    public static void ShotgunAppear()
    {
        GameObject shotgun = GameObject.FindGameObjectWithTag("Shotgun");
        if (shotgun == null)
        {
            return;
        }
        shotgun.SetActive(true);
    }

    public static void ShotgunDisappear()
    {
        GameObject shotgun = GameObject.FindGameObjectWithTag("Shotgun");
        if (shotgun == null)
        {
            return;
        }
        shotgun.SetActive(false);
    }
}
