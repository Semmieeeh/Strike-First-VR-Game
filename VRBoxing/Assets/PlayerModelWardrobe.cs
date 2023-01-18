using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelWardrobe : MonoBehaviour
{
    public PlayerMaterialManager playerManager;
    public GameObject[] haircuts;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerManager)
        {
            for (int i = 0; i < haircuts.Length; i++)
            {
                if (i == playerManager.hairCutIndex)
                {
                    haircuts[i].SetActive(true);
                    continue;
                }
                haircuts[i].SetActive(false);
            }
        }
    }
}
