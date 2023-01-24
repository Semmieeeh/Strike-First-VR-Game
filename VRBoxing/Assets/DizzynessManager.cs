using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DizzynessManager : MonoBehaviour
{
    public Image dizzyImage;

    [Range(0,1)]
    public float dizzyNess;
    public float dizzytoAddOnHit;
    public float dizzynessRemoveSpeed;
    // Update is called once per frame
    void Update()
    {
        dizzyImage.color = new(dizzyImage.color.r,dizzyImage.color.g,dizzyImage.color.b, dizzyNess);
        dizzyNess -= dizzynessRemoveSpeed * Time.deltaTime;
    }

    public void ApplyDizzy()
    {
        dizzyNess = dizzytoAddOnHit;
    }
}
