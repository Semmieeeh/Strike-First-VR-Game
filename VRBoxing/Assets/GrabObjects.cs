using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class GrabObjects : MonoBehaviourPunCallbacks
{
    public bool canGrab;
    public bool multiplier;
    public bool grabbed;
    public GameObject hand;
    public GameObject grabObject;
    Animator anim;
    public bool hardened;
    public bool canHarden;
    public Vector3[] handPos = new Vector3[4];
    public float timer;
    int handPosCount = 0;
    public float speed;
    public GameObject mesh;
    public GameObject leftHandDouble,rightHandDouble,normalHandLeft,normalHandRight;
    public GameObject enemy;
    public UniversalHealthBar healthBar;
    public PhotonView pv;

    void Start()
    {
        healthBar = transform.GetComponentInParent<UniversalHealthBar>();
        hand = gameObject;
        canHarden = true;
        anim = GetComponent<Animator>();
        pv = transform.parent.transform.parent.GetComponent<PhotonView>();
        
    }

    public void HardenFist(InputAction.CallbackContext context)
    {
        if (context.started && canHarden == true)
        {
            hardened = true;
            hand.GetComponent<BoxCollider>().isTrigger = false;
            Debug.Log("Hardened");
        }
    }
    public void ReleaseHarden(InputAction.CallbackContext context)
    {
        if (context.canceled && hardened == true)
        {
            hardened = false;
            hand.GetComponent<BoxCollider>().isTrigger = true;
            Debug.Log("Un-hardened");
        }
    }

    

    
    private void Update()
    {
        anim.SetBool("Hardened", hardened);
        timer += Time.deltaTime;
        if(timer > 0.1f)
        {
            if(handPosCount < 4)
            {
                handPosCount++;
            }
            timer = 0;
            handPos[3] = handPos[2];
            handPos[2] = handPos[1];
            handPos[1] = handPos[0];
            handPos[0] = transform.position;
        }
        
        if(handPosCount == 4)
        {
            speed = Vector3.Distance(handPos[3], handPos[2]) + Vector3.Distance(handPos[2], handPos[1]) + Vector3.Distance(handPos[1], handPos[0]);
            speed = speed / 5;
            speed = speed * 100;
            if (speed > 10)
            {
                speed = 10;
            }

        }

        //if(multiplier == true)
        //{
        //    leftHandDouble.SetActive(true);
        //    rightHandDouble.SetActive(true);
        //    normalHandLeft.SetActive(false);
        //    normalHandRight.SetActive(false);
        //}
        //else if(multiplier == false)
        //{
        //    leftHandDouble.SetActive(false);
        //    rightHandDouble.SetActive(false);
        //    normalHandLeft.SetActive(true);
        //    normalHandRight.SetActive(true);
        //}


    }
    public IEnumerator ReAppear()
    {
        if(canHarden == false)
        {
            mesh.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            yield return new WaitForSeconds(3);
            GetComponent<BoxCollider>().enabled = true;
            mesh.SetActive(true);
            hardened = false;
            canHarden = true;
        }
    }

    [PunRPC]
    public void Punch()
    {
        gameObject.transform.parent.GetComponent<UniversalHealthBar>().TakeDamage(speed);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            canHarden = false;
            //pv.RPC(nameof(ReAppear), RpcTarget.All);
            Server.DamageEnemy(speed);
            print("You Did "+speed + " Damage");
            
        }

        //if(collision.gameObject.tag == "Fist")
        //{

        //}
    }
}
