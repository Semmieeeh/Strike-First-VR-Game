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
    public float minRotX;
    public float minRotY;
    public float maxRotX;
    public float maxRotY;
    public float currentRotX;
    public float currentRotY;
    public bool blocking;
    public Server server;

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
    public void Blocking(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            server.isBlocking = true;
            blocking = true;
        }
    }
    public void StopBlocking(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            server.isBlocking = false;
            blocking = false;
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

        if(hardened == true)
        {
            hand.GetComponent<BoxCollider>().isTrigger = false;
        }
        else
        {
            hand.GetComponent<BoxCollider>().isTrigger = true;
        }

        

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
        if (collision.gameObject.tag == "Enemy" && collision.gameObject.GetComponent<PhotonView>().IsMine == false)
        {
            
            Server.DamageEnemy(speed);
            //canHarden = false;
            //pv.RPC(nameof(ReAppear), RpcTarget.All);
            print("You Did "+ speed + " Damage");
            
        }

        if (collision.gameObject.tag == "LeftFist" && collision.gameObject.GetComponent<PhotonView>().IsMine == false)
        {
            
            Server.DamageEnemy(speed / 4);
            //canHarden = false;
            //pv.RPC(nameof(ReAppear), RpcTarget.All);
            print("Your punch got Blocked! You did" + speed / 4 + " damage");
        }

        if (collision.gameObject.tag == "RightFist" && collision.gameObject.GetComponent<PhotonView>().IsMine == false)
        {
            
            Server.DamageEnemy(speed / 4);
            //canHarden = false;
            //pv.RPC(nameof(ReAppear), RpcTarget.All);
            print("Your punch got Blocked! You did" + speed / 4 +" damage");
        }
        if (collision.gameObject.tag == "Body" && collision.gameObject.GetComponent<PhotonView>().IsMine == false)
        {

            Server.DamageEnemy(speed / 2);
            //canHarden = false;
            //pv.RPC(nameof(ReAppear), RpcTarget.All);
            print("Body hit for " + speed / 2 + " damage");
        }
    }
}
