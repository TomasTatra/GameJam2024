using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _body;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public Camera camera = null;

    [Header("Base Movement")]
    private int dir = 0;
    private bool jumping = false;
    private double recentlyTouched = 0;
    private double recentlyJumped = 0;
    private float timeOnGround = 0; 
    public float speed = 4;
    public float jumpHigh = 9;
    public float coyoteTime = 1; //0.7
    public float jumpBuffer = 1; //0.7
    public float gravity = 1;
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
        if (strikeCall || dashCall)
            return;
        Vector2 direction = context.ReadValue<Vector2>();
        animator.SetFloat("Speed", Mathf.Abs(direction.x));
        print(direction);
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
        if (dashCall || strikeCall)
            return;
        if (context.started)
        {
            recentlyJumped = jumpBuffer;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && dash && !dashCall && chargedDash)
        {
            dashCall = true;
            dashDuration = dashTime;
            chargedDash = false;
            _body.gravityScale = 0;
        }
    }

    public void OnStrike(InputAction.CallbackContext context)
    {
        if (context.started && strike && !strikeCall && chargedStrike)
        {
            strikeCall = true;
            strikeDuration = strikeTime;
            chargedStrike = false;
            _body.gravityScale = 0;
            Fire();
        }
    }

    private void Fire()
    {
        return;
    }
}
