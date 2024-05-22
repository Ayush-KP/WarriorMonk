using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    float timer;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float stopDistance;
    private Vector3 originalScale;
    //private int patrolDirection;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeStates(EnemyStates.Charger_Idle);
        rb.gravityScale = 12f;
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeStates(EnemyStates.Charger_Idle);
        }
    }
   

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        Flip();
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:


                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    // Flip();
                    Debug.Log("Detected and change direction");
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                   
                    ChangeStates(EnemyStates.Charger_Suprised);
                  
                }
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                   
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                   
                }
                //Flip();
                break;

            case EnemyStates.Charger_Suprised:
                rb.velocity = new Vector2(0, jumpForce);
                ChangeStates(EnemyStates.Charger_Charged);
                timer = 0;
                break;

            case EnemyStates.Charger_Charged:
                timer = Time.deltaTime;
                if (timer < chargeDuration)
                {
                    //if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    //{
                    //    if (transform.localScale.x > 0)
                    //    {
                    //        Debug.Log("going right");
                    //        //Flip();
                    //        rb.velocity = new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y);
                    //    }
                    //    else
                    //    {
                    //        Debug.Log("going left");
                    //       // Flip();
                    //        rb.velocity = new Vector2(-(speed * chargeSpeedMultiplier), rb.velocity.y);
                    //    }
                    //}
                    //else
                    //{
                    //    rb.velocity = new Vector2(1, rb.velocity.y);
                    //}
                    float moveDirection = transform.localScale.x > 0 ? 1 : -1;
                    rb.velocity = new Vector2(moveDirection * speed * chargeSpeedMultiplier, rb.velocity.y);

                    if (!Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }

                    // Check the distance to the player
                    float distanceToPlayer = Mathf.Abs(transform.position.x - PlayerController.Instance.transform.position.x);
                    if (distanceToPlayer <= stopDistance)
                    {
                        rb.velocity = Vector2.zero;
                        ChangeStates(EnemyStates.Charger_Idle);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeStates(EnemyStates.Charger_Idle);
                }
                Flip();
                break;
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        if (GetCurrentEnemyState == EnemyStates.Charger_Idle)
        {
            anim.speed = 1;
        }
        if (GetCurrentEnemyState == EnemyStates.Charger_Charged)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }

    private void Flip()
    {
        //timer += Time.deltaTime;
        //if (timer > 0.5f)
        //{
        //    Debug.Log("facing right");
        //    timer = 0;
        //    patrolDirection *= -1; // Reverse the direction
        //    if (rb.velocity.x > 0)
        //    {
        //        sr.flipX = true;
        //        Debug.Log("facing right");
        //    }
        //    else if (rb.velocity.x <= 0)
        //    {
        //        sr.flipX = false;
        //        Debug.Log("facing left");
        //    }
        //    //if (PlayerController.Instance.transform.position.x < transform.position.x)
        //    //{

        //    //    sr.flipX = true;
        //    //}
        //    //else
        //    //{
        //    //    sr.flipX = false;
        //    //}
        //    ChangeStates(EnemyStates.Charger_Idle);
        //}
        //Debug.Log("X-velocity: " + rb.velocity.x);
        //if (rb.velocity.x > 0)
        //{
        //    sr.flipX = false;
        //    Debug.Log("facing right");
        //}
        //else if (rb.velocity.x < 0)
        //{
        //    sr.flipX = true;
        //    Debug.Log("facing left");
        //}
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
           // Debug.Log("facing right");
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
           // Debug.Log("facing left");
        }
    }
}

