//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Crawler : Enemy
//{
//    //float timer;
//    //[SerializeField] private float flipWaitTime;
//    [SerializeField] private float ledgeCheckX;
//    [SerializeField] private float ledgeCheckY;
//    [SerializeField] private LayerMask whatIsGround;
//    //[SerializeField] private float patrolDirection = 1; // Default patrol direction (right)
//    //[SerializeField] private Vector2 patrolStartPosition; // Initial position of the Crawler when patrolling starts
//    //[SerializeField] private float patrolRange = 5f; // Distance the Crawler can patrol from its initial position
//    //[SerializeField] private float detectionRange = 10f;
//    private Vector3 originalScale;

//    // Start is called before the first frame update
//    protected override void Start()
//    {
//        base.Start();
//        rb.gravityScale = 12f;
//        originalScale = transform.localScale;
//    }
//    //Update is called once per frame
//    protected override void Update()
//    {
//        if (health <= 0)
//        {
//            Death(0.05f);
//        }
//        base.Update();

//    }

//    protected override void UpdateEnemyStates()
//    {
//        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
//        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
//        float stopDistance = 3;
//        Flip();
//        switch (GetCurrentEnemyState)
//        {
//            case EnemyStates.Crawler_Idle:

//                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
//                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
//                {
//                    //Debug.Log("Detected and change direction");
//                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
//                }

//                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
//                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
//                {

//                    ChangeStates(EnemyStates.Crawler_Chase);

//                }
//                if (transform.localScale.x > 0)
//                {
//                    rb.velocity = new Vector2(-speed, rb.velocity.y);

//                }
//                else if(transform.localScale.x <= 0)
//                {
//                    rb.velocity = new Vector2(speed, rb.velocity.y);

//                }
//                break;

//            case EnemyStates.Crawler_Chase:
//                float moveDirection = transform.localScale.x > 0 ? 1 : -1;
//                rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
//                Flip();
//                if (!Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
//                {
//                    rb.velocity = new Vector2(0, rb.velocity.y);
//                }

//                // Check the distance to the player
//                float distanceToPlayer = Mathf.Abs(transform.position.x - PlayerController.Instance.transform.position.x);
//                if (distanceToPlayer <= stopDistance)
//                {
//                    rb.velocity = Vector2.zero;
//                    ChangeStates(EnemyStates.Charger_Idle);
//                }
//                break;
//                //float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
//                //if (distanceToPlayer > detectionRange)
//                //{
//                //    Patrol();
//                //}
//                //else
//                //{
//                //    // Player is within range, change state to chase the player
//                //    ChangeStates(EnemyStates.Crawler_Idle);
//                //}
//                // Patrolling behavior
//                //rb.velocity = new Vector2(-speed * transform.localScale.x, rb.velocity.y);
//                //break;

//            case EnemyStates.Crawler_Flip:
//                Flip();
//                ChangeStates(EnemyStates.Crawler_Idle);
//                //timer += Time.deltaTime;
//                //if (timer > flipWaitTime)
//                //{
//                //    timer = 0;
//                //    Flip();
//                //    ChangeStates(EnemyStates.Crawler_Idle);
//                //}
//                break;
//        }
//    }

//    private void Flip()
//    {
//        if (rb.velocity.x > 0)
//        {
//            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
//            // Debug.Log("facing right");
//        }
//        else if (rb.velocity.x < 0)
//        {
//            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
//            // Debug.Log("facing left");
//        }
//    }
//    //private void Patrol()
//    //{
//    //    // Move the Crawler horizontally based on its current direction
//    //    rb.velocity = new Vector2(speed * patrolDirection, rb.velocity.y);

//    //    // Check if the Crawler reaches the edge of the patrol range, then flip direction
//    //    if (transform.position.x <= patrolStartPosition.x - patrolRange || transform.position.x >= patrolStartPosition.x + patrolRange)
//    //    {
//    //        patrolDirection *= -1; // Reverse the direction
//    //        ChangeStates(EnemyStates.Crawler_Flip);
//    //    }
//    //}
//}


////public class Crawler : Enemy
////{
////    float timer;
////    [SerializeField] private float flipWaitTime = 0.5f;
////    [SerializeField] private float ledgeCheckDistance = 1f;
////    [SerializeField] private LayerMask whatIsGround;
////    [SerializeField] private float patrolDirection = 1; // Default patrol direction (right)
////    [SerializeField] private Vector2 patrolStartPosition; // Initial position of the Crawler when patrolling starts
////    [SerializeField] private float patrolRange = 5f; // Distance the Crawler can patrol from its initial position
////    [SerializeField] private float detectionRange = 10f;

////    // Start is called before the first frame update
////    protected override void Start()
////    {
////        base.Start();
////        rb.gravityScale = 12f;
////        patrolStartPosition = transform.position; // Set initial patrol start position
////        ChangeStates(EnemyStates.Crawler_Idle);
////    }

////    // Update is called once per frame
////    protected override void Update()
////    {
////        base.Update();
////        if (!PlayerController.Instance.pState.alive)
////        {
////            ChangeStates(EnemyStates.Crawler_Idle);
////        }
////    }
////    private void OnCollisionEnter2D(Collision2D collision)
////    {
////        if (collision.gameObject.CompareTag("Enemy"))
////        {
////            ChangeStates(EnemyStates.Crawler_Flip); ;
////        }
////    }

////    protected override void UpdateEnemyStates()
////    {
////        if (health <= 0)
////        {
////            Death(0.05f);
////            return;
////        }

////        Vector2 ledgeCheckStart = transform.position + new Vector3(ledgeCheckDistance * patrolDirection, 0);
////        Vector2 wallCheckDirection = Vector2.right * patrolDirection;

////        switch (GetCurrentEnemyState)
////        {
////            case EnemyStates.Crawler_Idle:
////                // Check for ledges and walls
////                if (!Physics2D.Raycast(ledgeCheckStart, Vector2.down, ledgeCheckDistance, whatIsGround) ||
////                    Physics2D.Raycast(transform.position, wallCheckDirection, ledgeCheckDistance, whatIsGround))
////                {
////                    ChangeStates(EnemyStates.Crawler_Flip);
////                }
////                else
////                {
////                    // Check for player within detection range
////                    float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
////                    if (distanceToPlayer <= detectionRange)
////                    {
////                        ChasePlayer();
////                    }
////                    else
////                    {
////                        Patrol();
////                    }
////                }
////                break;

////            case EnemyStates.Crawler_Flip:
////                Flip();
////                break;
////        }
////    }

////    private void Patrol()
////    {
////        // Move the Crawler horizontally based on its current direction
////        rb.velocity = new Vector2(speed * patrolDirection, rb.velocity.y);

////        // Check if the Crawler reaches the edge of the patrol range, then flip direction
////        if (transform.position.x <= patrolStartPosition.x - patrolRange || transform.position.x >= patrolStartPosition.x + patrolRange)
////        {
////            ChangeStates(EnemyStates.Crawler_Flip);
////        }
////    }

////    private void ChasePlayer()
////    {
////        // Move towards the player
////        transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
////    }

////    private void Flip()
////    {
////        if (rb.velocity.x > 0)
////        {
////            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
////            Debug.Log("facing right");
////        }
////        else if (rb.velocity.x < 0)
////        {
////            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
////            Debug.Log("facing left");
////        }
////    }
////}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*public class Crawler : Enemy
{
    //float timer;
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
        ChangeStates(EnemyStates.Crawler_Idle);
        rb.gravityScale = 12f;
        originalScale = transform.localScale;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Flip();
        }
    }
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeStates(EnemyStates.Crawler_Idle);
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
        //Flip();
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Crawler_Idle:


                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    // Flip();
                    //Debug.Log("Detected and change direction");
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {

                    ChangeStates(EnemyStates.Crawler_Chase);

                }
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);

                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);

                }
                Flip();
                break;

            case EnemyStates.Crawler_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                //timer = 0;
                //Flip();
                break;
               
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        if (GetCurrentEnemyState == EnemyStates.Crawler_Idle)
        {
            anim.speed = 1;
        }
        if (GetCurrentEnemyState == EnemyStates.Crawler_Chase)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }

    private void Flip()
    {
        if (rb.velocity.x > 0)
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
             Debug.Log("facing right");
        }
        else
        {
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
             Debug.Log("facing left");
        }
    }
}*/

public class Crawler : Enemy
{
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float stopDistance;
    private Vector3 originalScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeStates(EnemyStates.Crawler_Idle);
        rb.gravityScale = 12f;
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Flip();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeStates(EnemyStates.Crawler_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
            return;
        }

        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        float resumePatrolRange = 3f;
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Crawler_Idle:
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    Flip();
                   //sr.flipX = true;
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeStates(EnemyStates.Crawler_Chase);
                }

                rb.velocity = new Vector2(transform.localScale.x > 0 ? speed : -speed, rb.velocity.y);
                break;

            case EnemyStates.Crawler_Chase:
                if(resumePatrolRange >= Vector2.Distance(transform.position, PlayerController.Instance.transform.position))
                {
                    Debug.Log("Chasing");
                    rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                    if (PlayerController.Instance.transform.position.x < transform.position.x)
                    {
                        sr.flipX = true;
                    }
                    else
                    {
                        sr.flipX = false;
                    }
                }
                else
                {
                    ChangeStates(EnemyStates.Crawler_Idle);
                }
                
                break;
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        if (GetCurrentEnemyState == EnemyStates.Crawler_Idle)
        {
            anim.speed = 1;
        }
        if (GetCurrentEnemyState == EnemyStates.Crawler_Chase)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Debug.Log(transform.localScale.x > 0 ? "facing right" : "facing left");
        //if(transform.localScale.x > 0)
        //{
        //    sr.flipX = true;
        //}
        //else if(transform.localScale.x < 0)
        //{
        //    sr.flipX = false;
        //}
    }
}
