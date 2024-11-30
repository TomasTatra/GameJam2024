using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    private int dir = 0;
    private bool jumping = false;
    private Rigidbody2D _body;

    private double recentlyTouched = 0;
    private double recentlyJumped = 0;

    public Camera camera = null;


    [Header("Base Movement")]
    public float speed = 4;
    public float jumpHigh = 9;
    public float coyoteTime = 1; //0.7
    public float jumpBuffer = 1; //0.7
    [Space(10)]

    [Header("Small jump tween")]
    public float jumpVelocityCut = 2;
    public float jumpCutMulltiplier = 2;
    public float jumpForce = 40;
    public float jumpFraction = 8;
    [Space(10)]

    [Header("Small camera tween")]
    public float time = 1;


    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        if (camera == null)
        {
            throw new System.Exception("Camera on Player is missing.");
        }
    }

    void Update()
    {
        //camera.transform.position = new Vector3(_body.transform.position.x, _body.transform.position.y, -10);
        camera.transform.DOMove(new Vector3(_body.transform.position.x, _body.transform.position.y, -10), 1);
        recentlyJumped -= Time.deltaTime;
        recentlyTouched -= Time.deltaTime;

        if (_body.velocity.y < jumpVelocityCut && _body.velocity.y > 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y / jumpCutMulltiplier);

        if (_body.velocity.y > -jumpVelocityCut && _body.velocity.y < 0)
            _body.velocity = new Vector2(_body.velocity.x, _body.velocity.y * jumpCutMulltiplier);
        
        if (_body.velocity.y == 0)
        {
            recentlyTouched = coyoteTime;
            _body.velocity = new Vector2(dir * speed, _body.velocity.y);
        }

        if (recentlyJumped > 0)
            Jump();
        
        else if (dir != 0)
        {
             _body.AddForce(new Vector2(dir * jumpForce, 0));
            if (_body.velocity.x > speed)
            {
                _body.velocity = new Vector2(speed, _body.velocity.y);
            } else if (_body.velocity.x < -speed)
            {
                _body.velocity = new Vector2(-speed, _body.velocity.y);
            }
        }
        else
        {
            if (_body.velocity.x > 0)
            {
                _body.AddForce(new Vector2(-jumpForce/ jumpFraction, 0));
            }
            if (_body.velocity.x < 0)
            {
                _body.AddForce(new Vector2(jumpForce/ jumpFraction, 0));
            }
        }
    }

    private void Jump()
    {
        if (_body.velocity.y == 0 || _body.velocity.y < 0 && recentlyTouched > 0)
        {
            _body.velocity = new Vector2(_body.velocity.x, jumpHigh);
            recentlyJumped = 0;
            recentlyTouched = 0;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        if (direction.x < 0)
        {
            dir = -1;
        }
        else if (direction.x > 0)
        {
            dir = 1;
        }
        else
        {
            dir = 0;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            recentlyJumped = jumpBuffer;
        }
    }
}
