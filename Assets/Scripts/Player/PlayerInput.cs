using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Move keys")]
    public Action<Vector2> keyboard;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    [Header("Jump key")]
    public Action jump;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Dash key")]
    public Action dash;
    public KeyCode dashKey = KeyCode.LeftShift;

    void Update()
    {
        MoveKeys();
        Jump();
        Dash();
    }

    void MoveKeys()
    {
        Vector2 move = Vector2.zero;

        if (Input.GetKey(up))
            move.y = 1;
        if (Input.GetKey(down))
            move.y = -1;
        if (Input.GetKey(left))
            move.x = -1;
        if (Input.GetKey(right))
            move.x = 1;

        move.Normalize();
        keyboard?.Invoke(move);
    }

    void Jump()
    {
        if (Input.GetKeyDown(jumpKey))
            jump?.Invoke();
    }

    void Dash()
    {
        if (Input.GetKeyDown(dashKey))
            dash?.Invoke();
    }
}
