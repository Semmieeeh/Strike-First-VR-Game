using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BaseInteraction : UIBehaviour
{
    public static RaycastHit currentRay;
    static bool canRenewRaycast;
    public bool Hovered
    {
        get
        {
            return false;
            
        }
    }


    private void Update()
    {
        if (!Application.isPlaying) return;

        if (canRenewRaycast)
        {
            Transform rayOrigin = GameManager.instance.selectionManager.CurrentSelector;
            Physics.Raycast(rayOrigin.position, rayOrigin.forward, out currentRay);
            canRenewRaycast = false;
        }
    }
    void LateUpdate() => canRenewRaycast = true;


}
