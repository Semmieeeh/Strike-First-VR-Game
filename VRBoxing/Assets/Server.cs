using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.InputSystem;

public class Server : MonoBehaviourPunCallbacks
{
    public PlayerMaterialManager localPlayerMaterialManager;
    public PlayerRagdollManager ragdollmanager;

    public UniversalHealthBar healthBar;

    public PlayerInput input;
    public InputAction action;

    [Space(10)]
    [Header("Particles")]
    public ParticleSystem bloodParticle;
    public ParticleSystem[] HitEffects;

    public float minDamageParticleEffect;
    public float resetHealthAmount = 1000;
    public float damagelvl1Amount = 700, damagelvl2Amount = 300;

    public const string kDamage = "DMG";
    public const string kHealing = "HEAL";
    public const string kHealingApplied = "HEALA";
    public const string kDamageApplied = "DMGA";
    public const string kHealth = "HP";
    public const string kHealthReset = "HPR";
    public const string kRoundsWon = "ROW";
    public const string kCanFight = "CAF";
    public const string kPlayerPosition = "PLP";
    public const string kPlayerDead = "PLDE";

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
                props.Add(kHealth, resetHealthAmount);
                props.Add(kHealingApplied, true);
                props.Add(kHealing, 0f);
                props.Add(kPlayerPosition, Vector3.zero);
                props.Add(kRoundsWon, 0);
                props.Add(kHealthReset, true);
                props.Add(kPlayerDead, false);

                MyPlayer.SetCustomProperties(props);

                InitializeMyPlayerMaterials();


                myPlayerInitialized = true;
            }
            if (!roomInitialized)
            {
                var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

                roomProperties.Add(kCanFight, true);
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

        Debug.LogWarning(PhotonNetwork.CurrentRoom.CustomProperties[kCanFight]);
    }

    public static Player MyPlayer { get; set; }
    bool myPlayerInitialized;
    public static Player OtherPlayer;

    public static Player Winner { get; set; }

    public void DamagePlayerDebug()
    {
        DamageEnemy(100f);
        print("Enemy Took Damage");
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

        if (!(bool)properties[kHealthReset])
        {
            properties[kHealth]= resetHealthAmount;
            properties[kHealthReset] = true;
        }
        if (properties.ContainsKey(kDamageApplied))
        {
            if (!(bool)properties[kDamageApplied] && CanMove())
            {
                print("You took" + (float)properties[kDamage] + " damage");
                properties[kDamageApplied] = true;

                float newHealth = (float)properties[kHealth];

                newHealth -= (float)properties[kDamage];

                properties[kHealth] = newHealth;

                if(newHealth <= 0)
                {
                    SetMovementActive(false);
                    properties[kPlayerDead] = true;
                    ragdollmanager.EnableRagdoll(true);
                    newHealth = 0;
                }

                localPlayerMaterialManager.damageLevel = DetermineDamageLevel(newHealth);


                MyPlayer.SetCustomProperties(properties);
                
                print(properties[kHealth] + " Health");
                print(properties[kDamage] + " Damage");
            }
            if (!(bool)properties[kHealingApplied] & CanMove())
            {
                print("You healed" + (float)properties[kHealing]);
                properties[kHealingApplied] = true;

                float newHealth = (float)properties[kHealth];

                newHealth+=(float)properties[kHealing];

                properties[kHealth] = newHealth;

                MyPlayer.SetCustomProperties(properties);

                print(properties[kHealth] + " Health");
                print(properties[kHealing] + " Healing");

            }
        }

       
    }
    public static void DamageEnemy(float damage)
    {
        if (CanMove() == false) return;

        Hashtable properties = OtherPlayer.CustomProperties;
        if (!properties.ContainsKey(kDamage))
        {
            properties.Add(kDamage, damage);
            //{ "DMG" , 35 }
        }
        else
        {
            var originalDamage = properties[kDamage];
            properties[kDamage] = /*(float)originalDamage +*/ damage;
        
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

    public static void ResetHealth()
    {
        var props1 = MyPlayer.CustomProperties;

        props1[kHealthReset] = false;
        props1[kHealth] = server.resetHealthAmount;

        MyPlayer.SetCustomProperties(props1);

        var props2 = OtherPlayer.CustomProperties;

        props2[kHealthReset] = false;
        props2[kHealth] = server.resetHealthAmount;

        OtherPlayer.SetCustomProperties(props2);
    }

    /// <summary>
    /// Function used to enable the players movement and damage systems. This function will be executed by the host of the game
    /// </summary>
    public static void SetMovementActive(bool active)
    {
        var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[kCanFight] = active;

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        print("Player Cannot Fight Anymore");
    }

    /// <summary>
    /// Used to reset both player's properties to fight again. This function will be executed by the host of the game
    /// </summary>
    public static void ResetPlayersProperties()
    {
        var props1 = MyPlayer.CustomProperties;

        props1[kDamage] = 0f;
        props1[kHealing] = 0f;
        props1[kHealingApplied] = true;
        props1[kDamageApplied] = true;
        props1[kHealth] = server.resetHealthAmount;
        props1[kDamageLevel] = 0;
        props1[kPlayerDead] = false;

        server.ragdollmanager.EnableRagdoll(false);

        MyPlayer.SetCustomProperties(props1);

        var props2 = OtherPlayer.CustomProperties;

        props2[kDamage] = 0f;
        props2[kHealing] = 0f;
        props2[kHealingApplied] = true;
        props2[kDamageApplied] = true;
        props2[kHealth] = server.resetHealthAmount;
        props2[kDamageLevel] = 0;
        props1[kPlayerDead] = false;

        OtherPlayer.SetCustomProperties(props2);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        var props = otherPlayer.CustomProperties;

        props.Clear();

        otherPlayer.SetCustomProperties(props);
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
        print(newHealth + " Is your new health");
        int lvl = 0;
        if (newHealth < server.damagelvl1Amount) lvl = 1;
        if (newHealth < server.damagelvl2Amount) lvl = 2;
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

    public static bool CanMove()
    {
        var room = PhotonNetwork.CurrentRoom;

        if(room != null)
        {
            var roomProps = room.CustomProperties;
            if (roomProps.ContainsKey(kCanFight))
            {
                return (bool)roomProps[kCanFight];
            }
        }
        return true;
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

    float previousHealth1, previousHealth2;
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        float health = (float)changedProps[kHealth];

        if (targetPlayer == MyPlayer) {
            if (previousHealth1 != health)
            {
                previousHealth1 = health;
                Debug.LogError($"(Neppe Error)(optie 1) {targetPlayer.NickName} 's health changed to {health}");
            }
        }
        else
        {
            if(previousHealth2 != health)
            {
                previousHealth2 = health;
                Debug.LogError($"(Neppe Error)(optie 2) {targetPlayer.NickName} 's health changed to {health}");
                
            }
        }

    }
}
