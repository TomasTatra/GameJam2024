using MarkusSecundus.Utils.Behaviors.Cosmetics;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _body;
    private Animator animator;
    private int lastAnimation = 0;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private List<Sprite> _sprites;

    [SerializeField]
    private List<AnimatorController> _animatorControllers;

    public Camera camera = null;

    [Header("Fading screen and respawning")]
    public float fadeTime = 0.33f;
    public bool _alive = false;
    [SerializeField] private FadeEffect _fadeTween;
    private Vector2 _lastCheckpoint;
    [Space(10)]

    [Header("Base Movement")]
    public float speed = 4;
    public float jumpHigh = 9;
    public float coyoteTime = 1; //0.7
    public float jumpBuffer = 1; //0.7
    public float gravity = 1;
    private int dir = 0;
    private bool jumping = false;
    private double recentlyTouched = 0;
    private double recentlyJumped = 0;
    private float timeOnGround = 0;
    [Space(10)]

    [Header("Small jump tween")]
    public float jumpVelocityCut = 2;
    public float jumpCutMulltiplier = 2;
    public float jumpForce = 40;
    public float jumpFraction = 8;
    [Space(10)]

    [Header("Small camera tween")]
    public float time = 1;
    [Space(10)]

    [Header("Special powers")]
    public float timeToRestartAbilities = 1;
    [Space(5)]
    public bool doubleJump = false;
    private bool chargedDouble = true;
    [Space(5)]
    public bool dash = false;
    public float dashSpeed = 20;
    public float dashTime = 1;
    public float dashColddown = 3;
    private bool chargedDash = true;
    private bool dashCall = false;
    private float dashDuration = 0;
    private float dashReset = 0;
    [Space(10)]
    public bool strike = false;
    public float strikeTime = 1;
    public float strikeOffset = 10;
    private bool strikeCall = false;
    public float strikeColddown = 3;
    private float strikeDuration = 0;
    private float strikeReset = 0;
    private bool chargedStrike = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _body = GetComponent<Rigidbody2D>();
        
        if (camera == null)
        {
            throw new System.Exception("Camera on Player is missing.");
        }
        _body.gravityScale = gravity;
    }

    void Update()
    {
        ChangeAnimation();
        camera.transform.position = new Vector3(_body.transform.position.x, _body.transform.position.y, -10);
        //camera.transform.DOMove(new Vector3(_body.transform.position.x, _body.transform.position.y, -10), 1);
        recentlyJumped -= Time.deltaTime;
        recentlyTouched -= Time.deltaTime;
        dashDuration -= Time.deltaTime;
        strikeDuration -= Time.deltaTime;

        //if (dashReset > 0)
        //{
        //    dashReset -= Time.deltaTime;
        //    if (dashReset <= 0)
        //        chargedDash = true;
        //}

        //if (strikeReset > 0)
        //{
        //    strikeReset -= Time.deltaTime;
        //    if (strikeReset <= 0)
        //        chargedStrike = true;
        //}

        if (dashCall && !strikeCall)
        {
            float direction = _body.velocity.x;
            direction = spriteRenderer.flipX ? -1 : 1;
            _body.velocity = new Vector2(direction * dashSpeed, 0);
            if (_body.velocity == new Vector2(0, 0) || dashDuration < 0)
            { 
                dashCall = false;
                dashReset = dashColddown;
                _body.gravityScale = gravity;
            }
            return;
        }

        if (strikeCall && !dashCall)
        {
            _body.velocity = new Vector2(0, 0);
            if (strikeDuration < 0)
            {
                strikeCall = false;
                strikeReset = strikeColddown;
                _body.gravityScale = gravity;
            }
            return;
        }

        if (_body.velocity.y < jumpVelocityCut && _body.velocity.y > 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y / jumpCutMulltiplier);

        if (_body.velocity.y > -jumpVelocityCut && _body.velocity.y < 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y * jumpCutMulltiplier);

        if (timeOnGround > timeToRestartAbilities && _body.velocity.y == 0)
        {
            recentlyTouched = coyoteTime;
            chargedDash = true;
            chargedDouble = true;
            chargedStrike = true;
        }
        if (_body.velocity.y == 0)
        {
            timeOnGround += Time.deltaTime;
            _body.velocity = new Vector2(dir * speed, _body.velocity.y);
        }
        else
            timeOnGround = 0;

        if (recentlyJumped > 0)
            Jump();

        else if (dir != 0)
        {
            _body.AddForce(new Vector2(dir * jumpForce, 0));
            if (_body.velocity.x > speed)
            {
                _body.velocity = new Vector2(speed, _body.velocity.y);
            }
            else if (_body.velocity.x < -speed)
            {
                _body.velocity = new Vector2(-speed, _body.velocity.y);
            }
        }
        else
        {
            if (_body.velocity.x > 0)
            {
                _body.AddForce(new Vector2(-jumpForce / jumpFraction, 0));
            }
            if (_body.velocity.x < 0)
            {
                _body.AddForce(new Vector2(jumpForce / jumpFraction, 0));
            }
        }
    }

    private void ChangeAnimation()
    {
        int animation = -1;
        if (dashCall)
            animation = 1;
        else if (strikeCall)
            animation = 2;
        else if (_body.velocity.y > 0.5)
            animation = 3;
        else if (_body.velocity.y < -0.5)
            animation = 4;
        else if (_body.velocity.x > 0.5 || _body.velocity.x < -0.5)
            animation = 5;
        else
            animation = 6;

        //if (lastAnimation == animation)
        //    return;

        lastAnimation = animation;
        animator.SetBool("Dashing", false);
        animator.SetBool("Attacking", false);
        animator.SetBool("Jumping", false);
        animator.SetBool("Falling", false);
        animator.SetFloat("Speed", 0);

        switch (lastAnimation)
        {
            case 1:
                animator.SetBool("Dashing", true);
                break;
            case 2:
                animator.SetBool("Attacking", true);
                break;
            case 3:
                animator.SetBool("Jumping", true);
                break;
            case 4:
                animator.SetBool("Falling", true);
                break;
            case 5:
                animator.SetFloat("Speed", 1);
                break;
            default:
                animator.SetFloat("Speed", 0);
                break;
        }
    }

    private void Jump()
    {
        if (dashCall || strikeCall)
            return;
        if (_body.velocity.y == 0 || _body.velocity.y < 0 && recentlyTouched > 0)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpHigh);
            recentlyJumped = 0;
            recentlyTouched = 0;
        }
        else if (doubleJump && chargedDouble)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpHigh);
            recentlyJumped = 0;
            recentlyTouched = 0;
            chargedDouble = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_alive && (dashCall || strikeCall))
            return;
        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.x < 0)
        {
            dir = -1;
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            dir = 1;
            spriteRenderer.flipX = false;
        }
        else
        {
            dir = 0;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_alive && (dashCall || strikeCall))
            return;
        if (context.started)
        {
            recentlyJumped = jumpBuffer;
        }
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.started && _alive)
        {
            if (GameManager.Instance.GetIndex() == 1)
                OnDash();
            if (GameManager.Instance.GetIndex() == 2)
                OnStrike();
        }
    }

    public void OnDash()
    {
        if (dash && !dashCall && chargedDash)
        {
            dashCall = true;
            dashDuration = dashTime;
            chargedDash = false;
            _body.gravityScale = 0;
        }
    }

    public void OnStrike()
    {
        if (strike && !strikeCall && chargedStrike)
        {
            strikeCall = true;
            strikeDuration = strikeTime;
            chargedStrike = false;
            _body.gravityScale = 0;
            Fire();
        }
    }

    [SerializeField] Projectile projectilePrefab;
    private void Fire()
    {
        var projectile = Instantiate(projectilePrefab);
        projectile.transform.position = _body.transform.position + new Vector3(strikeOffset * (spriteRenderer.flipX?-1:1), 0, 0);
        projectile.SetDirection(spriteRenderer.flipX ? -1 : 1);
        
    }

    public void KillPlayer()
    {
        if (!_alive) return;
        _alive = false;
        TurnOffCamera();
    }

    public void SetCharacter(int index)
    {
        spriteRenderer.sprite = _sprites[index];
        animator.runtimeAnimatorController = _animatorControllers[index];
    }

    public void SetLastCheckpoint(Vector2 position)
    {
        _lastCheckpoint = new Vector2(position.x, position.y);
    }

    private void TurnOffCamera()
    {
        if (_alive)
            return;

        if(_fadeTween) _fadeTween.FadeIn(Respawn, fadeTime);
    }

    private void Respawn()
    {
        _body.position = _lastCheckpoint;
        _alive = true;
        if (_fadeTween) _fadeTween.FadeOut(fadeTime);
    }
}
