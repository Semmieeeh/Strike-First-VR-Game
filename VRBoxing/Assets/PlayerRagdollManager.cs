using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdollManager : MonoBehaviour
{
    public Transform ragdollHead, ragdollRight, ragdollLeft, ragdollBody;
    public Transform localHead, localRight, localLeft, localBody;

    public bool isOn;
    public void EnableRagdoll(bool value)
    {
        SetPositionAndRotation(ragdollHead, localHead, value);
        SetPositionAndRotation(ragdollRight, localRight, value);
        SetPositionAndRotation(ragdollLeft, localLeft, value);
        SetPositionAndRotation(ragdollBody, localBody, value);

        localHead.gameObject.SetActive(!value);
        localRight.gameObject.SetActive(!value);
        localLeft.gameObject.SetActive(!value);
        localBody.gameObject.SetActive(!value);

        isOn = value;
    }

    void SetPositionAndRotation(Transform current, Transform Target, bool active)
    {
        current.position = Target.position;
        current.rotation = Target.rotation;

        current.gameObject.SetActive(active);
        ragdollHead.GetComponent<Rigidbody>().isKinematic = !active;
    }
}
