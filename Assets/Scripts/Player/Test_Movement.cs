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
    [SerializeField] private bool isStatic = true;

    [Header("Couter Movement")]
    [SerializeField] private float counterMovementRate = 0.175f;
    [SerializeField] private float staticDesacellerationTime = 0.35f;
    private float _staticTime = 0f;

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
        StaticStop();
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

    private Vector3 _staticStartVelocity;
    void StaticStop()
    {
        if(!(isStatic = keyboard == new Vector2()) || isJumping)
        {
            _staticTime = 0;
            return;
        }

        if(_staticTime == 0)
            _staticStartVelocity = rb.velocity;

        _staticTime += Time.deltaTime;
        rb.velocity = Vector3.Lerp(_staticStartVelocity, new Vector3(0, rb.velocity.y, 0), Mathf.Clamp(_staticTime / staticDesacellerationTime, 0, 1));
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
        // Extra gravity
        rb.AddForce(Physics.gravity * gravityMultiplier);

        // Needs input to continue
        if (isStatic) return;

        // Get orientation without y
        Vector3 planeOrientationForward = RemoveYFromVector(orientation.transform.forward);
        Vector3 planeOrientationRight = RemoveYFromVector(orientation.transform.right);

        // Work as friction
        CounterMove();
        
        // Foward
        rb.AddForce(planeOrientationForward * keyboard.y * moveSpeed);
        // Sideways
        rb.AddForce(planeOrientationRight * keyboard.x * moveSpeed);
    }

    void CounterMove()
    {
        if (isJumping) return;

        Vector3 moveDir = rb.velocity;
        Vector3 counterMove = -moveDir.normalized * moveSpeed * counterMovementRate;

        if(Math.Abs(moveDir.x * rb.mass / counterMove.x) < Time.fixedDeltaTime)
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        else
            rb.AddForce(new Vector3(counterMove.x, 0, 0));

        if (Math.Abs(moveDir.z * rb.mass / counterMove.z) < Time.fixedDeltaTime)
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        else
            rb.AddForce(new Vector3(0, 0, counterMove.z));
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