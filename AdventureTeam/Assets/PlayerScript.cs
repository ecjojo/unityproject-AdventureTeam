using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//isRun isJump isDoubleJump isHit isDead
public class PlayerScript : NetworkBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    [SyncVar] public int PlayerIndex;
    [SyncVar] public int Severdeadcount = 0;
    [SyncVar] public string PlayerName;
    [SyncVar] public Color PlayerColor;

    [SyncVar] public int HP = 5;
    [SyncVar] public bool isDead = false;

    //Game UI
    public Text name;
    public GameObject HPSlider;

    //Player Movement
    public float speed;
    bool isGround;
    bool CanJump;
    public int JumpCount;
    public float JumpForce;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        name.text = PlayerName;
        name.color = PlayerColor;

        if (isLocalPlayer)
        {
            MainGameController.instance.localPlayerManager = this;
        }

        //Reset
        JumpCount = 0;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (rb.velocity.x > 0.01f)
            {
                sr.flipX = false;
            }
            else if (rb.velocity.x < -0.01f)
            {
                sr.flipX = true;
            }

            //Movement
            float move = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(move * speed, rb.velocity.y);

            if ((rb.velocity.x > 0.01f || rb.velocity.x < -0.01f) && isGround == true)
            {
                anim.Play("Run");
            }

            if (rb.velocity.y < -0.01f && isGround == false)
            {
                anim.SetBool("isFall", true);
            }

            //Jump
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
            {
                JumpCount = JumpCount + 1;

                if (JumpCount == 1)
                {
                    anim.SetBool("isJump", true);
                    rb.AddForce(new Vector2(0, JumpForce));
                }
                else if (JumpCount == 2)
                {
                    anim.SetBool("isDoubleJump", true);
                    rb.AddForce(new Vector2(0, JumpForce * 1.2f));
                }
            }
            //isHit 

            //isDead
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //JumpReset
        if (isLocalPlayer)
        {
            if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform" ||
                collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player")
            {
                JumpCount = 0;
                isGround = true;
                anim.SetBool("isJump", false);
                anim.SetBool("isDoubleJump", false);
                anim.SetBool("isFall", false);
                anim.SetBool("isRun", false);
            }

            else if (collision.gameObject.tag == "Enemy" && 
                collision.gameObject.GetComponent<NetworkEnemyAI>().CanAttackPlayerbyCollision &&
                collision.gameObject.GetComponent<NetworkEnemyAI>().AttackCountDown ==0 )
            {
                JumpCount = 0;
                anim.Play("Hit");
                CmdDamage(collision.gameObject.GetComponent<NetworkEnemyAI>().Atk);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //JumpReset
        if (isLocalPlayer)
        {
            if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform" ||
                collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Player")
            {
                isGround = false;
            }
        }
    }

    [Command]
    public void CmdDamage(int v)
    {
        HP -= v;
        HPSlider.GetComponent<Slider>().value = HP;

        if (HP <= 0)
        {
            anim.SetBool("isDead", true);
            Invoke("TuneDead", 0.5f);
        }
        else if (HP < 0)
        {
            anim.Play("Hit");
        }
    }

    public void TuneDead()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        SceneController.instance.LoseResult.SetActive(true);

        if (MainGameController.instance.AllPlayerPrefabsObjs[0].GetComponent<PlayerScript>().isDead)
        {
            if (MainGameController.instance.AllPlayerPrefabsObjs[1].GetComponent<PlayerScript>().isDead)
            {
                CmdDead();
            }
        }
    }

    [Command]
    public void CmdDead()
    {
        RpcDead();
    }

    [ClientRpc]
    void RpcDead()
    {
        SceneController.instance.LoseResult.SetActive(true);
    }

}

