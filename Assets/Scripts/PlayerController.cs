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
    private double recentlyTouched = 0;
    private double recentlyJumped = 0;

    public Camera camera = null;

    [Header("Base Movement")]
    private int dir = 0;
    private bool jumping = false;
    private double recentlyTouched = 0;
    private double recentlyJumped = 0;
    private bool lastFrameZero = false; 
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
    public bool doubleJump = false;
    [Space(5)]
    public bool dash = false;
    public float dashSpeed = 20;
    public float dashTime = 1;
    public float dashColldown = 3;
    [Space(10)]
    public bool strike = false;
    private bool charged = true;
    private bool dashCall = false;
    private float dashDuration = 0;
    private float dashReset = 0;


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

        if (dash && dashReset > 0)
        {
            dashReset -= Time.deltaTime;
            if (dashReset <= 0)
                charged = true;
        }

        if (dashCall)
        {
            if (_body.velocity == new Vector2(0, 0) || dashDuration < 0)
            { 
                dashCall = false;
                dashReset = dashColldown;
                _body.gravityScale = gravity;
            }
            float direction = _body.velocity.x;
            direction = direction > 0 ? 1 : -1;
            _body.velocity = new Vector2(direction * dashSpeed, 0);
            return;
        }

        if (_body.velocity.y < jumpVelocityCut && _body.velocity.y > 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y / jumpCutMulltiplier);

        if (_body.velocity.y > -jumpVelocityCut && _body.velocity.y < 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y * jumpCutMulltiplier);

        if (lastFrameZero && _body.velocity.y == 0)
            recentlyTouched = coyoteTime;

        if (_body.velocity.y == 0)
        {
            lastFrameZero = true;
            _body.velocity = new Vector2(dir * speed, _body.velocity.y);
        }
        else
            lastFrameZero = false;

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
        if (dashCall)
            return;
        if (_body.velocity.y == 0 || _body.velocity.y < 0 && recentlyTouched > 0)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpHigh);
            recentlyJumped = 0;
            recentlyTouched = 0;
            if (doubleJump)
                charged = true;
        }
        else if (doubleJump && charged)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpHigh);
            recentlyJumped = 0;
            recentlyTouched = 0;
            charged = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
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
        if (dashCall)
            return;
        if (context.started)
        {
            recentlyJumped = jumpBuffer;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && dash && !dashCall && charged && _body.velocity.x != 0)
        {
            dashCall = true;
            dashDuration = dashTime;
            charged = false;
            _body.gravityScale = 0;
        }
    }
}
