using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float timer;

    public static Shade Instance;
  
    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        SaveData.Instance.SaveShadeData();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeStates(EnemyStates.Shade_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeStates(EnemyStates.Shade_Idle);
        }
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Shade_Idle:
                if (_dist < chaseDistance)
                {
                    ChangeStates(EnemyStates.Shade_Chase);
                }
                else
                {
                    //Wander();
                    FlipShade();
                }
                break;

            case EnemyStates.Shade_Chase:
                if (GetCurrentEnemyState != EnemyStates.Shade_Stunned)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                    FlipShade();
                }
                break;

            case EnemyStates.Shade_Stunned:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeStates(EnemyStates.Shade_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Shade_Death:
                Death(0);
                break;
        }
    }


    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        Debug.Log("Health: "+health);
        if (health > 0)
        {
            ChangeStates(EnemyStates.Shade_Stunned);
        }
        else
        {
            Debug.Log("Death state");
            ChangeStates(EnemyStates.Shade_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
        //Destroy(gameObject, _destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        if(GetCurrentEnemyState == EnemyStates.Shade_Idle)
        {
            anim.Play("Player_Idle");
        }
        //anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Shade_Idle);
        anim.SetBool("Walking", GetCurrentEnemyState == EnemyStates.Shade_Chase);
        //anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Shade_Stunned);
        if (GetCurrentEnemyState == EnemyStates.Shade_Death)
        {
            PlayerController.Instance.RestoreMana();
            SaveData.Instance.SavePlayerData();
            anim.SetTrigger("Death");
            Destroy(gameObject);
        }

    }

    protected override void Attack()
    {
        anim.SetTrigger("Attacking");
        PlayerController.Instance.TakeDamage(damage);
    }

    void FlipShade()
    {
        if (PlayerController.Instance.transform.position.x < transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
}
