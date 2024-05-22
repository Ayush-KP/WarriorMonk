using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrame;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [Space(5)]

    [Header("Wall Jump Settings")]
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpDuration;
    [SerializeField] private Vector2 wallJumpPower;
    float wallJumpDirection;
    bool isWallJumping;
    bool isWallSliding;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    [Header("Attacking")]
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;
    [SerializeField] private Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] private Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject attackEffect;
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepXRecoiled, stepYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int heartShards;
    [SerializeField] GameObject BloodSpur;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate OnHealthChangedCallback;
    [Space(5)]

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Mana Settings")]
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;
    [SerializeField] Image manaStorage;
    public bool halfMana;

    public ManaOrbHandler manaOrbHandler;
    public int orbShards;
    public int manaOrbs;
    [Space(5)]

    [Header("Spell Settings")]
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    float timeSinceCast;
    float castOrHealTimer;
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;
    [Space(5)]

    [Header("Camera Settings")]
    [SerializeField] private float playerFallSpeedThreshold = -10f;

    [HideInInspector]public PlayerStateList pState;
    public Rigidbody2D rb;
    private SpriteRenderer sr;

    private float xAxis;
    private float yAxis;
    private float gravity;
    private Animator anim;
    private bool canDash = true;
    private bool dashed;
    bool openMap;
    private bool canFlash = true;

    //unlocking
    public bool unlockedWallJump;
    public bool unlockedDash;
    public bool unlockedSideCast;
    public bool unlockedUpCast;

    private static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerController>();
                if (instance == null)
                {
                    Debug.LogError("No PlayerController instance found in the scene.");
                }
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        manaOrbHandler = FindObjectOfType<ManaOrbHandler>();        
        
        //SaveData.Instance.LoadPlayerData();
        gravity = rb.gravityScale;
        Mana = mana;
        manaStorage.fillAmount = Mana;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
        if (pState.cutscene) return;
        if (pState.alive)
        {
            GetInputs();
            ToggleMap();
        }

        UpdateJumpVariables();
        RestoreTimeScale();
        UpdateCameraYDampForPlayerFall();

        if (pState.dashing) return;
        if (pState.alive)
        {
            if (!isWallJumping)
            {
                Flip();
                Move();
                Jump();
            }
            if (unlockedWallJump)
            {
                WallSlide();
                WallJump();
            }
            if (unlockedDash)
            {
                StartDash();
            }
            Attack();
            Heal();
            CastSpell();
        }
        FlashWhileInvincible();

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Death());
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && pState.casting)
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }
    }
    private void Awake()
    {
        // Ensure there's only one instance of PlayerController
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this && instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Health = maxHealth;
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return;
        Recoil();
    }
    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        openMap = Input.GetButton("Map");
        if (Input.GetButton("Cast/Heal"))
        {
            castOrHealTimer += Time.deltaTime;
        }
        else
        {
            castOrHealTimer = 0;
        }
    }

    void ToggleMap()
    {
        Debug.Log("Toggle Map"+openMap);
        if (openMap) 
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }

    private void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if(xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2 (walkSpeed * xAxis,rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void UpdateCameraYDampForPlayerFall()
    {
        //if falling past a certain speed threshold
        if(rb.velocity.y < playerFallSpeedThreshold && !CameraManager.Instance.isLerpingYDamp && !CameraManager.Instance.hasLerpingYDamp)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }

        else if(rb.velocity.y >= 0 && CameraManager.Instance.isLerpingYDamp && !CameraManager.Instance.hasLerpingYDamp)
        {
             CameraManager.Instance.hasLerpingYDamp = false;
             StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }

    void StartDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        if(_exitDir.y > 0)
        {
            Debug.Log("Jump");
            rb.velocity = jumpForce * _exitDir;
        }

        if(_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0? 1 : -1;
            Move();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if(yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
                Hit(SideAttackTransform, SideAttackArea,ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(attackEffect, SideAttackTransform);
            }

            else if(yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                AttackEffectAngle(attackEffect,80,UpAttackTransform);
            }

            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                AttackEffectAngle(attackEffect, -90, DownAttackTransform);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir,float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if(objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    if(Mana < 1)
                    {
                        Mana += manaGain;
                    }
                    else
                    {
                        manaOrbHandler.UpdateMana(manaGain * 3);
                    }
                }
            }
        }
    }

    void AttackEffectAngle(GameObject _attackEffect, int _effectangle, Transform _attackTransform)
    {
        _attackEffect = Instantiate(_attackEffect, _attackTransform);
        _attackEffect.transform.eulerAngles = new Vector3(0, 0, _effectangle);
        _attackEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil() 
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }
        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        if(pState.recoilingX && stepXRecoiled < recoilXSteps)
        {
            stepXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepYRecoiled < recoilYSteps)
        {
            stepYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    void StopRecoilX()
    {
        stepXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        if (Health <= 0)
        {
            Health = 0;
            StartCoroutine(Death());
        }
        else
        {
            StartCoroutine(StopTakingDamage());
        }
    }

    IEnumerator StopTakingDamage() 
    {
        pState.invincible = true;
        GameObject _bloodSpurParticles = Instantiate(BloodSpur, transform.position, Quaternion.identity);
        Destroy(_bloodSpurParticles, 1.5f);
        anim.SetTrigger("takeDamage");
        yield return new WaitForSeconds(0.5f);
        pState.invincible = false;  
    }

    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.05f);
        canFlash = true;
    }
    void FlashWhileInvincible() 
    {
        if (pState.invincible)
        {
            if(Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);

    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1;
        GameObject _bloodSpurParticles = Instantiate(BloodSpur, transform.position, Quaternion.identity);
        Destroy(_bloodSpurParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScene());

        yield return new WaitForSeconds(0.9f);
        Debug.Log("Shade spawned");
        Vector3 offset = new Vector3(2.0f, 0.0f, 0.0f);
        Instantiate(GameManager.Instance.shade,transform.position + offset, Quaternion.identity);
    }

    public void Respawned()
    {
        if (!pState.alive)
        {
            pState.alive = true;
            halfMana = true;
            UIManager.Instance.SwitchMana(UIManager.ManaState.HalfMana);
            Mana = 0;
            Health = maxHealth;
            anim.Play("Player_Idle");
        }
    }

    public void RestoreMana()
    {
        halfMana = false;
        UIManager.Instance.SwitchMana(UIManager.ManaState.FullMana);
    }
    public int Health
    {
        get{ return health; }
        set
        {
            if (health != value) 
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if(OnHealthChangedCallback != null)
                {
                    OnHealthChangedCallback.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        if(Input.GetButton("Cast/Heal") && castOrHealTimer > 0.05f && Health < maxHealth && Mana > 0 && !pState.jumping && !pState.dashing)
        {
            //Debug.Log("Working!!");
            pState.healing = true;
            //healing
            anim.SetBool("Healing",true);
            healTimer += Time.deltaTime;
            //Debug.Log("Working!!"+ healTimer);
            if (healTimer >= timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            //drain mana
            manaOrbHandler.usedMana = true;
            manaOrbHandler.countDown = 3f;
            Mana -= Time.deltaTime * manaDrainSpeed; 
        }

        else
        {
            pState.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            //if mana changes
            if (mana != value)
            {
                if (!halfMana)
                {
                    mana = Mathf.Clamp(value, 0, 1);
                }
                else
                {
                    mana = Mathf.Clamp(value, 0, 0.5f);
                }
                manaStorage.fillAmount = Mana;
            }
        }
    }

    void CastSpell()
    {
        if(Input.GetButtonUp("Cast/Spell") && castOrHealTimer <= 0.05f && timeSinceCast >= timeBetweenCast && Mana >= manaSpellCost)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }

        if (Grounded())
        {
            downSpellFireball.SetActive(false);
        }

        if (downSpellFireball.activeInHierarchy)
        {
            rb.velocity += downSpellForce * Vector2.down;
           // rb.velocity += downSpellForce * Vector2.down;
        }
    }

    IEnumerator CastCoroutine()
    {
       

        //side cast
        if((yAxis == 0 || (yAxis < 0 && Grounded())) && unlockedSideCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.1f);
            GameObject _fireball = Instantiate(sideSpellFireball, SideAttackTransform.position, Quaternion.identity);
            if (pState.lookingRight)
            {
                _fireball.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _fireball.transform.eulerAngles = new Vector2(_fireball.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
            Mana -= manaSpellCost;
            manaOrbHandler.usedMana = true;
            manaOrbHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        //up cast
        else if(yAxis > 0 && unlockedUpCast)
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.1f);
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
            Mana -= manaSpellCost;
            manaOrbHandler.usedMana = true;
            manaOrbHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        //down cast
        else if(yAxis < 0 && !Grounded())
        {
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.1f);
            downSpellFireball.SetActive(true);
            Mana -= manaSpellCost;
            manaOrbHandler.usedMana = true;
            manaOrbHandler.countDown = 3f;
            yield return new WaitForSeconds(0.35f);
        }

        
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    public bool Grounded()
    {
        if(Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) 
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX,0,0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !pState.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
        }
        else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            pState.jumping = true;
            airJumpCounter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }
        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrame;
        }
        else
        {
            jumpBufferCounter--;    
        }
    }

    private bool Walled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    void WallSlide()
    {
        if(Walled() && !Grounded() && xAxis != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = !pState.lookingRight ? 1 : -1;
            CancelInvoke(nameof(StopWallJumping));
        }
        if(Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            dashed = false;
            airJumpCounter = 0;

            //if((pState.lookingRight && transform.eulerAngles.x ==0) || (!pState.lookingRight && transform.eulerAngles.y != 0))
            //{
            //    pState.lookingRight = !pState.lookingRight;
            //    Debug.Log("lookingright: " + pState.lookingRight);
            //    int _yRotation = pState.lookingRight? 0 : 180;

            //    transform.eulerAngles = new Vector2(transform.eulerAngles.x, _yRotation);
            //}
            Invoke(nameof(StopWallJumping), wallJumpDuration);
        }
    }

    void StopWallJumping()
    {
        isWallJumping = false;
    }
}

