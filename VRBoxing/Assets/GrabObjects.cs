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
    public GameObject hand, physicsHand;
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
    public bool canPunch;
    public float cooldown;
    public AudioManager audioManager;
    public bool canPlay = true;
    public GrabObjects otherHand;
    

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
            blocking = true;
        }
    }
    public void StopBlocking(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
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
            speed = speed / 2;
            speed = speed * 100;
            

        }
        if(gameObject.tag == "LeftFist")
        {
            if (speed > 5 && hardened == true && audioManager.IsPlaying(0) == false && canPlay == true)
            {
                audioManager.PlayArmAudio(0, speed * 0.1f, 0.5f);
                canPlay = false;
            }
            if (speed < 5 || hardened == false && otherHand.hardened == false)
            {
                audioManager.StopAudio(0);
                canPlay = true;
            }
            if (audioManager.IsPlaying(0) == true)
            {
                audioManager.SetPitch(0, 0.2f, speed * 0.2f);
            }
        }

        if (gameObject.tag == "RightFist")
        {
            if (speed > 5 && hardened == true && audioManager.IsPlaying(1) == false && canPlay == true)
            {
                audioManager.PlayArmAudio(1, speed * 0.1f, 0.5f);
                canPlay = false;
            }
            if (speed < 5 || hardened == false && otherHand.hardened == false)
            {
                audioManager.StopAudio(1);
                canPlay = true;
            }
            if (audioManager.IsPlaying(1) == true)
            {
                audioManager.SetPitch(1, 0.2f, speed * 0.2f);
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

        if (hardened == true)
        {
            hand.GetComponent<BoxCollider>().isTrigger = false;
            physicsHand.GetComponent<BoxCollider>().isTrigger = false;

        }
        else
        {
            hand.GetComponent<BoxCollider>().isTrigger = true;
            physicsHand.GetComponent<BoxCollider>().isTrigger = true;
        }

        cooldown -= 1 * Time.deltaTime;

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
    void PlaySound()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.tag == "LeftFist")
        {
            if (collision.gameObject.tag == "Head" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {
                audioManager.PlayAudio(2, 0.8f, 1.2f);
                Server.DamageEnemy(speed);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("You Hit The Head And Did " + speed + " Damage");

                ApplyBlood(collision);
                Server.CheckHitEffect(collision.GetContact(0).point, speed);

            }

            if (collision.gameObject.tag == "LeftFist" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {
                audioManager.PlayAudio(2, 0.8f, 1.2f);
                Server.DamageEnemy(speed / 4);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("Your punch got Blocked! You did" + speed / 4 + " damage");
                Server.CheckHitEffect(collision.GetContact(0).point, speed / 4);

            }

            if (collision.gameObject.tag == "RightFist" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {
                audioManager.PlayAudio(2, 0.8f, 1.2f);
                Server.DamageEnemy(speed / 4);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("Your punch got Blocked! You did" + speed / 4 + " damage");
                Server.CheckHitEffect(collision.GetContact(0).point, speed / 4);
            }
            if (collision.gameObject.tag == "Body" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {

                Server.DamageEnemy(speed / 2);
                audioManager.PlayAudio(2, 0.8f, 1.2f);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("Body hit for " + speed / 2 + " damage");
                ApplyBlood(collision);
                Server.CheckHitEffect(collision.GetContact(0).point, speed / 2);
            }
        }
        if(gameObject.tag == "RightFist")
        {
            if (collision.gameObject.tag == "Head" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {
                audioManager.PlayAudio(3, 0.8f, 1.2f);
                Server.DamageEnemy(speed);
                cooldown = 1;

                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);

                print("You Hit The Head And Did " + speed + " Damage");
                ApplyBlood(collision);
                Server.CheckHitEffect(collision.GetContact(0).point, speed);

            }

            if (collision.gameObject.tag == "LeftFist" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {
                audioManager.PlayAudio(3, 0.8f, 1.2f);
                Server.DamageEnemy(speed / 4);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("Your punch got Blocked! You did" + speed / 4 + " damage");
                Server.CheckHitEffect(collision.GetContact(0).point, speed / 4);
            }

            if (collision.gameObject.tag == "RightFist" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {
                audioManager.PlayAudio(3, 0.8f, 1.2f);
                Server.DamageEnemy(speed / 4);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("Your punch got Blocked! You did" + speed / 4 + " damage");
                Server.CheckHitEffect(collision.GetContact(0).point, speed / 4);
            }
            if (collision.gameObject.tag == "Body" && collision.gameObject.GetComponent<PhotonView>().IsMine == false && cooldown <= 0)
            {

                Server.DamageEnemy(speed / 2);
                audioManager.PlayAudio(3, 0.8f, 1.2f);
                cooldown = 1;
                //canHarden = false;
                //pv.RPC(nameof(ReAppear), RpcTarget.All);
                print("Body hit for " + speed / 2 + " damage");
                ApplyBlood(collision);
                Server.CheckHitEffect(collision.GetContact(0).point, speed / 2);
            }
        }
    }

    public void ApplyBlood(Collision collision)
    {
        var contact = collision.GetContact(0);

        Vector3 position = contact.point;
        Vector3 normal = contact.normal;

        Server.ApplyBlood(position, normal);
    }
}


