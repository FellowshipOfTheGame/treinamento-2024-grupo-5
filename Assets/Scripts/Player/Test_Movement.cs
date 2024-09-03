// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;

public class Test_Movement : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;
    [SerializeField] protected Transform body;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4500;
    [SerializeField] private float moveMaxSpeed = 20f;
    [SerializeField] private float moveMinSpeed = 0.1f;

    [Header("Couter Movement")]
    [SerializeField] private float counterMovementRate = 0.175f;

    [Header("Jump")]
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private bool isJumping = false;

    [Header("Inputs")]
    [SerializeField] private Vector2 keyboard;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInputs();
        Vector3 coisa = new Vector3();
        coisa.Normalize();
        Debug.Log(coisa);
    }

    private void FixedUpdate()
    {
        Move();
    }
    void GetInputs()
    {
        keyboard.x = Input.GetAxisRaw("Horizontal");
        keyboard.y = Input.GetAxisRaw("Vertical");
        keyboard.Normalize();
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    void Jump()
    {
        Vector3 planeOrientationForward = orientation.transform.forward * 0.1f;
        planeOrientationForward.y = 0;
        rb.AddForce((body.transform.up + planeOrientationForward) * jumpForce, ForceMode.Impulse);
    }

    #region Movement

    private void Move()
    {
        // Get orientation without y
        Vector3 planeOrientationForward = RemoveYFromVector(orientation.transform.forward);
        Vector3 planeOrientationRight = RemoveYFromVector(orientation.transform.right);

        //CounterMove();

        // Extra gravity
        rb.AddForce(Physics.gravity * gravityMultiplier);
        // Foward
        rb.AddForce(planeOrientationForward * keyboard.y * moveSpeed);
        // Sideways
        rb.AddForce(planeOrientationRight * keyboard.x * moveSpeed);
    }

    void CounterMove()
    {
        if (isJumping) return;

        Vector3 moveDir = rb.velocity;
        /*if (moveDir.magnitude < moveMinSpeed)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }*/

        moveDir.Normalize();
        rb.AddForce(-moveDir * moveSpeed * counterMovementRate);
    }

    Vector3 RemoveYFromVector(Vector3 vector)
    {
        float magnitude = Vector3.Magnitude(vector);
        vector.y = 0;
        vector.Normalize();
        vector *= magnitude;
        return vector;
    }
    #endregion
}