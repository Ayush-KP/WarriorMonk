using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float timer;

    //Wandering
    [SerializeField] private float wanderRange = 5f; // Range within which the bat can wander
    [SerializeField] private float wanderSpeed = 2f; // Speed at which the bat wanders
    private Vector2 wanderTarget; // The current target position for wandering

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeStates(EnemyStates.Bat_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeStates(EnemyStates.Bat_Idle);
        }
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position); 
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                if(_dist < chaseDistance)
                {
                    ChangeStates(EnemyStates.Bat_Chase);
                }
                else
                {
                    Wander();
                    FlipBat();
                }
                break;

            case EnemyStates.Bat_Chase:
                if (GetCurrentEnemyState != EnemyStates.Bat_Stunned)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                    FlipBat();
                }
                break;

            case EnemyStates.Bat_Stunned:
                timer += Time.deltaTime;
                if(timer > stunDuration)
                {
                    ChangeStates(EnemyStates.Bat_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Bat_Death:
                Death(Random.Range(5, 10));
                break;
        }
    }


    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        if(health > 0)
        {
            ChangeStates(EnemyStates.Bat_Stunned);
        }
        else
        {
            ChangeStates(EnemyStates.Bat_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle",GetCurrentEnemyState == EnemyStates.Bat_Idle);
        anim.SetBool("Chase",GetCurrentEnemyState == EnemyStates.Bat_Chase);
        anim.SetBool("Stunned",GetCurrentEnemyState == EnemyStates.Bat_Stunned);
        if(GetCurrentEnemyState == EnemyStates.Bat_Death)
        {
            anim.SetTrigger("Death");
        }

    }
    private void SetNewWanderTarget()
    {
        // Choose a random position within the wander range
        float wanderX = transform.position.x + Random.Range(-wanderRange, wanderRange);
        float wanderY = transform.position.y + Random.Range(-wanderRange, wanderRange);
        wanderTarget = new Vector2(wanderX, wanderY);
    }
    private void Wander()
    {
        if (Vector2.Distance(transform.position, wanderTarget) < 0.1f)
        {
            SetNewWanderTarget();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, wanderTarget, wanderSpeed * Time.deltaTime);
        }
    }
    void FlipBat()
    {
        if(PlayerController.Instance.transform.position.x < transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
}
