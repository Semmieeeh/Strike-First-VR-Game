using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BaseInteraction : Selectable
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
        if (canRenewRaycast)
        {
            Transform rayOrigin = GameManager.instance.selectionManager.CurrentSelector;
            Physics.Raycast(rayOrigin.position, rayOrigin.forward, out currentRay);
            canRenewRaycast = false;
        }
    }
    void LateUpdate() => canRenewRaycast = true;


}
