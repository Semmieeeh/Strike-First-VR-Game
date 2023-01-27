using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;

public class InGameDisconnect : MonoBehaviourPunCallbacks
{
    public Vector2 rotateTreshold;

    Vector3 defaultScale;

    public bool disconnected;
    public Slider progressSlider;

    private void Start()
    {
        defaultScale = transform.localScale;
    }
    void Update()
    {
        if (transform.eulerAngles.x >= rotateTreshold.x && transform.eulerAngles.x <= rotateTreshold.y) // de disconnect menu kan aan
        {
            transform.localScale = defaultScale;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }

        if (transform.localScale == Vector3.zero)
        {
            progressSlider.value = 0;
        }
        else
        {
            if (false) // waneer de trigger is vastgehouden 
            {
                if (false) //en de b knop is 5 keer gedrukt, leaved de speler dfe game
                {
                    progressSlider.value += 5;

                    if (progressSlider.value == 5)
                    {
                        Disconnect();
                    }
                }
            }
            else
            {
                progressSlider.value = 0;
            }

        }
    }

    public void Disconnect()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
