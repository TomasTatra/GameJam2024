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

    public float speed = 250;
    public float jumpHigh = 10;
    public float coyoteTime = 2;
    public float jumpBuffer = 2;


    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        recentlyJumped -= Time.deltaTime;
        recentlyTouched -= Time.deltaTime;

        if (recentlyJumped > 0)
            Jump();

        if (_body.velocity.y == 0)
            recentlyTouched = coyoteTime;

        _body.velocity = speed * Time.deltaTime * new Vector2(dir, 0) + new Vector2(0, _body.velocity.y);
        
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
