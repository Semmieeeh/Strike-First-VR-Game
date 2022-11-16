using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public Transform rightHand, leftHand, VRCam;

    public bool useCam;
    public bool useRightHand;
    public Transform CurrentSelector
    {
        get
        {
            if (!useCam)
            {
                if (useRightHand) return rightHand;

                return leftHand;
            }
            return VRCam;
        }
    }
}
