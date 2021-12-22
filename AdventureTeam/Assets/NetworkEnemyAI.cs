using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum NetworkEnemyState
{
    Idle, Target, Back, Attack, ShootBullet, TakeHit, Dead
}

public class NetworkEnemyAI : NetworkBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    Vector2 OriPos;
    SpriteRenderer sr;

    //Set Value
    //[SyncVar] public int Hp = 5;
    public bool CanMove; //Pig
    public bool CanMovebyFly; //Bee
    public bool CanShootBullet; //Bee
    public bool CanAttackPlayerbyCollision;
    public bool HaveAttackAnim;

    public int AttackMist;
    public int ShootMist;

    public int Atk = 2;
    public float AttackCountDown = 0;

    public GameObject EnemyBulletPrefabs; //Poison,Bullet
    public float ShootCountDown = 0;
    public NetworkEnemyState CurEnemyState = NetworkEnemyState.Idle;

    public GameObject TargetPlayer = null;
    [SyncVar] Vector2 TargetPos;
    float minDist;
    Vector2 CurPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        OriPos = rb.position;
        minDist = 9999;
    }

    public void changeEnemyState(NetworkEnemyState NewState)
    {
        CurEnemyState = NewState;
    }

    void Update()
    {
        // change flipX
        if (TargetPos.x - rb.position.x > 0.01f)
        {
            sr.flipX = true;
        }
        else if (TargetPos.x - rb.position.x < -0.01f)
        {
            sr.flipX = false;
        }

        if (isServer)
        {
            //Find Cur Player
            for (int i = 0; i < MainGameController.instance.AllPlayerPrefabsObjs.Count; i++)
            {
                if (!MainGameController.instance.AllPlayerPrefabsObjs[i].GetComponent<PlayerScript>().isDead)
                {
                    TargetPos = MainGameController.instance.AllPlayerPrefabsObjs[i].GetComponent<Rigidbody2D>().position;
                    CurPos = rb.position;

                    if (Vector2.Distance(CurPos, TargetPos) < minDist)
                    {
                        TargetPlayer = MainGameController.instance.AllPlayerPrefabsObjs[i];
                        minDist = Vector2.Distance(CurPos, TargetPos);
                    }
                }
                else
                {
                    changeEnemyState(NetworkEnemyState.Idle);
                }
            }

            //ShootCountDown 
            if (AttackCountDown > 0)
            {
                AttackCountDown -= Time.deltaTime;
            }
            if (ShootCountDown > 0)
            {
                ShootCountDown -= Time.deltaTime;
            }

            if (CurEnemyState == NetworkEnemyState.Idle)
            {
                IdleAI();
            }
            else if (CurEnemyState == NetworkEnemyState.Target)
            {
                TargetAI();
            }
            else if (CurEnemyState == NetworkEnemyState.Back)
            {
                BackAI();
            }
            else if (CurEnemyState == NetworkEnemyState.ShootBullet)
            {
                AttackAI();
            }
            else if (CurEnemyState == NetworkEnemyState.ShootBullet)
            {
                ShootBulletAI();
            }
            else if (CurEnemyState == NetworkEnemyState.TakeHit)
            {
                TakeHitAI();
            }
            else if (CurEnemyState == NetworkEnemyState.Dead)
            {
                DeadAI();
            }
        }
    }

    public void IdleAI() //No movement
    {
        rb.velocity = Vector2.zero;

        if (minDist < AttackMist)
        {
            if(CanAttackPlayerbyCollision)
            {
                changeEnemyState(NetworkEnemyState.Target);
            }
            else if(CanShootBullet)
            {
                changeEnemyState(NetworkEnemyState.ShootBullet);
            }
            else if(HaveAttackAnim)
            {
                changeEnemyState(NetworkEnemyState.Attack);
            }
        }
    }

    public void TargetAI() //
    {
        anim.Play("Run");
        rb.velocity = new Vector2((TargetPos.x - rb.position.x) * 1, 0);

        changeEnemyState(NetworkEnemyState.Idle);
    }

    public void BackAI()
    {
        anim.Play("Back");
        rb.velocity = (OriPos - rb.position).normalized * 5;

        if (Vector2.Distance(OriPos, rb.position) < 0.1f)
        {
            changeEnemyState(NetworkEnemyState.Idle);
        }
    }

    public void AttackAI()
    {
        anim.Play("Attack");
    }

    public void ShootBulletAI() //
    {
        if (ShootCountDown <= 0)
        {
            if (isServer)
            {
                ShootCountDown = 5;

                GameObject b = Instantiate(EnemyBulletPrefabs, transform.position, Quaternion.identity);
                b.GetComponent<Rigidbody2D>().velocity = (TargetPlayer.GetComponent<Rigidbody2D>().position - rb.position).normalized * 10;

                NetworkServer.Spawn(b);

            }
        }
        changeEnemyState(NetworkEnemyState.Idle);
    }


    public void TakeHitAI()
    {
        anim.Play("Hit");
        Invoke("DeadAI", 1f);
    }

    public void DeadAI()
    {
        anim.SetBool("isDead", true);
        Invoke("DestoryEnemy", 1f);
    }

    public void DestoryEnemy()
    {
        NetworkServer.Destroy(gameObject);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerTrigger")
        {
            CurEnemyState = NetworkEnemyState.TakeHit;
            AttackCountDown = 5;
            Invoke("BackAI", 1f);
        }
        else if (collision.gameObject.tag == "Player")
        {
            changeEnemyState(NetworkEnemyState.Idle);
        }
    }
}