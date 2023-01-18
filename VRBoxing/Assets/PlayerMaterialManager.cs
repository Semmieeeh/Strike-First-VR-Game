using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaterialManager : MonoBehaviour
{
    public Material hands, body, head, hair;
    [Space(8)]

    public MeshRenderer bodyRenderer;
    public MeshRenderer leftHandRenderer, rightHandRenderer, headRenderer, hairRenderer;

    public int damageLevel;
    public int skinColorIndex;
    public int glovesColorIndex;

    public int hairCutIndex;
    public int hairCutColorIndex;

    public GameObject[] haircuts;

    public SkinColorTextures[] skinColors;

    public Texture2D[] handsColors;

    [System.Serializable]
    public struct SkinColorTextures
    {
        public string colorName;
        public Texture2D[] textures;
    }


    void Update()
    {
        UpdatePlayerMaterials();
    }

    void UpdatePlayerMaterials()
    {
        if (skinColorIndex >= skinColors.Length) skinColorIndex = 0;
        else if (skinColorIndex < 0) skinColorIndex = skinColors.Length - 1;

        if (damageLevel > 2) damageLevel = 0;
        else if (damageLevel < 0) damageLevel = 2;

        if (glovesColorIndex >= handsColors.Length) glovesColorIndex = 0;
        else if (glovesColorIndex < 0) glovesColorIndex = handsColors.Length - 1;

        if(hairCutIndex >= haircuts.Length) hairCutIndex = 0;
        else if(hairCutIndex < 0) hairCutIndex = haircuts.Length - 1;

        //if(hairCutColorIndex >= hai)

        head.mainTexture = skinColors[skinColorIndex].textures[damageLevel];
        body.mainTexture = skinColors[skinColorIndex].textures[damageLevel];
        hands.mainTexture = handsColors[glovesColorIndex];

        for (int i = 0; i < haircuts.Length; i++)
        {
            if(i == hairCutIndex)
            {
                haircuts[i].SetActive(true);
                continue;
            }
            haircuts[i].SetActive(false);
        }
    }

    public void ScrollThroughSkinColor(int amountToAdd)
    {
        skinColorIndex += amountToAdd;
    }

    public void ScrollThroughHandsColor(int amoundToAdd)
    {
        glovesColorIndex += amoundToAdd;
    }

    public void ScrollThroughHaircut(int amountToAdd)
    {
        hairCutIndex += amountToAdd;
    }

    public void ScrollThroughHairColor(int amountToAdd)
    {
        hairCutColorIndex += amountToAdd;
    }
}
