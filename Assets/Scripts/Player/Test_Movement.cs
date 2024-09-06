using System;
using System.Collections;
using UnityEngine;

public class Test_Movement : MonoBehaviour
{
    [Serializable]
    private class InterpolatedFloat
    {
        [SerializeField] protected float startValue;
        [SerializeField] private float target;
        private float _time;
        [SerializeField] private float duration;

        public InterpolatedFloat(float target, float duration, float startValue = 0)
        {
            this.startValue = startValue;
            SetTarget(target);
            this.duration = duration;
        }

        public void Update(float time) => _time += time;

        public void SetTarget(float target)
        {
            this.target = target;
            _time = 0;
        }

        public void Reset() => _time = 0;

        public float GetValue() => Mathf.Lerp(startValue, target, _time / duration);
    }

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;
    [SerializeField] protected Transform body;

    [Header("Ground Movemnent")]
    [SerializeField] private float groundMoveSpeed = 10;
    [SerializeField] private InterpolatedFloat groundMoveTime = new(1, 0.2f);

    // Counter movement
    [SerializeField] private bool isStatic = true;
    [SerializeField] private InterpolatedFloat staticDesacellerationTime = new(1, 0.35f);

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 50;

    // Counter movement
    [SerializeField] private float counterMovementRate = 0.3f;

    [Header("Jump")]
    // Forces
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private float jumpForce = 20f;

    // Checks
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float changeIsGroundedStateDelay = 2f;
    private Coroutine _cancellingGrounded;
    [SerializeField] private Vector3 normalVector;

    [Header("Inputs")]
    [SerializeField] private Vector2 keyboard;

    [Header("Orientation")]
    // tirar [SerializeField] private depois
    [SerializeField] private Vector3 _planeOrientationForward;
    [SerializeField] private Vector3 _planeOrientationRight;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Vector2 A;
    private void Update()
    {
        GetInputs();
        GetOrientation();

        // Check if player is 'static-intetion'
        if (isGrounded)
        {
            if (!(isStatic = keyboard == Vector2.zero))
            {
                staticDesacellerationTime.Reset();
                GroundMove();
            }
            else
            {
                groundMoveTime.Reset();
                StaticStop();
            }
        }
        
        Vector2 _planeVel = new Vector2(rb.velocity.x, rb.velocity.z);
        float beta = (_planeOrientationRight.x * _planeVel.y - _planeOrientationRight.y * _planeVel.x) / (_planeOrientationRight.x * _planeOrientationForward.y - _planeOrientationRight.y * _planeOrientationForward.x);
        float alpha = (_planeVel.x - beta * _planeOrientationForward.x) / _planeVel.x;
        A = new Vector2(alpha, beta);
    }

    void FixedUpdate()
    {
        Gravity();
        AirMove();
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

    void GetOrientation()
    {
        // Get orientation without y
        _planeOrientationForward = RemoveYFromVector(orientation.transform.forward);
        _planeOrientationRight = RemoveYFromVector(orientation.transform.right);

    }

    #region Jump

    void Jump()
    {
        rb.AddForce(body.transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(normalVector * 0.5f * jumpForce, ForceMode.Impulse);
    }

    void Gravity() => rb.AddForce(Physics.gravity * gravityMultiplier);

    #endregion

    #region GroundMovement

    void GroundMove()
    {
        groundMoveTime.Update(Time.deltaTime);
        Vector3 maxVelocity = (_planeOrientationForward * keyboard.y + _planeOrientationRight * keyboard.x) * groundMoveSpeed;
        Vector3 moveVelocity = Vector3.Lerp(Vector3.zero, maxVelocity, groundMoveTime.GetValue());
        rb.velocity = moveVelocity + new Vector3(0, rb.velocity.y, 0);
    }

    private Vector3 _staticStartVelocity;
    void StaticStop()
    {
        // Stop player logic
        if (staticDesacellerationTime.GetValue() == 0)
            _staticStartVelocity = rb.velocity;

        staticDesacellerationTime.Update(Time.deltaTime);
        rb.velocity = Vector3.Lerp(_staticStartVelocity, new Vector3(0, rb.velocity.y, 0), staticDesacellerationTime.GetValue());
    }

    #endregion

    #region AirMovement
    private void AirMove()
    {
        if (isGrounded) return;

        // Needs input to continue
        if (isStatic) return;

        // Work as friction
        CounterMove();
        
        // Foward
        rb.AddForce(_planeOrientationForward * keyboard.y * airMoveSpeed);
        // Sideways
        rb.AddForce(_planeOrientationRight * keyboard.x * airMoveSpeed * 0.4f);
    }
    
    void CounterMove()
    {
        if (!isGrounded) return;

        Vector3 moveDir = rb.velocity;
        Vector3 counterMove = -moveDir.normalized * airMoveSpeed * counterMovementRate;

        if (Math.Abs(moveDir.x * rb.mass / counterMove.x) < Time.fixedDeltaTime)
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