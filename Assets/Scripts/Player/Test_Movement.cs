// Some stupid rigidbody based movement by Dani

using System;
using System.Collections;
using UnityEngine;

public class Test_Movement : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;
    [SerializeField] protected Transform body;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 50;
    [SerializeField] private float moveMaxSpeed = 25f;
    [SerializeField] private bool isStatic = true;

    [Header("Couter Movement")]
    [SerializeField] private float counterMovementRate = 0.3f;
    [SerializeField] private float staticDesacellerationTime = 0.35f;
    private float _staticTime = 0f;

    [Header("Jump")]
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float jumpForce = 20f;

    [SerializeField] private bool isGrounded = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float changeIsGroundedStateDelay = 2f;
    private Coroutine _cancellingGrounded;
    [SerializeField] private Vector3 normalVector;

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
        // Movement
        keyboard.x = Input.GetAxisRaw("Horizontal");
        keyboard.y = Input.GetAxisRaw("Vertical");
        keyboard.Normalize();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();
    }

    void Jump()
    {
        Vector3 planeOrientationForward = orientation.transform.forward * 0.1f;
        planeOrientationForward.y = 0;
        rb.AddForce((body.transform.up + planeOrientationForward) * jumpForce, ForceMode.Impulse);
        rb.AddForce(normalVector * 0.5f * jumpForce, ForceMode.Impulse);
    }

    #region Movement

    private Vector3 _staticStartVelocity;
    void StaticStop()
    {
        // Check if player is 'static-intetion'
        if (!(isStatic = keyboard == new Vector2()) || !isGrounded)
        {
            _staticTime = 0;
            return;
        }

        // Stop player logic
        if (_staticTime == 0)
            _staticStartVelocity = rb.velocity;

        _staticTime += Time.deltaTime;
        rb.velocity = Vector3.Lerp(_staticStartVelocity, new Vector3(0, rb.velocity.y, 0), Mathf.Clamp(_staticTime / staticDesacellerationTime, 0, 1));
    }

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

        if (!isGrounded) return;

        LimitSpeed();
    }

    void CounterMove()
    {
        if (!isGrounded) return;

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

    void LimitSpeed()
    {
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, -moveMaxSpeed, moveMaxSpeed), 
            rb.velocity.y, 
            Mathf.Clamp(rb.velocity.z, -moveMaxSpeed, moveMaxSpeed)
        );
    }
    #endregion

    private void OnCollisionStay(Collision other)
    {
        // Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (groundLayer != (groundLayer | (1 << layer))) return;

        // get if ground is working as a floor
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                isGrounded = true;
                normalVector = normal;
                if (_cancellingGrounded != null)
                    StopCoroutine(_cancellingGrounded);
            }
        }

        // Invoke ground/wall cancel, since we can't check normals with CollisionExit
        _cancellingGrounded = StartCoroutine(ChangeIsGroundedState(changeIsGroundedStateDelay, false));
    }

    bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < 75;
    }

    IEnumerator ChangeIsGroundedState(float delay, bool newState)
    {
        yield return new WaitForSeconds(delay);
        isGrounded = newState;
    }
}