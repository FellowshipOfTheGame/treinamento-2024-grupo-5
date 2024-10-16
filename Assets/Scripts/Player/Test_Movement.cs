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

        public void SetDuration(float duration) => this.duration = duration;

        public float GetDuration() => duration;

        public void Reset() => _time = 0;

        public float GetValue() => Mathf.Lerp(startValue, target, _time / duration);
    }

    [Serializable]
    private class Timer
    {
        [SerializeField] private float duration;
        private float _time;

        public Timer(float duration)
        {
            this.duration = duration;
        }

        public void Update(float time) => _time += time;

        public void Reset() => _time = 0;

        public float GetTime() => _time;

        public bool IsOver() => _time >= duration;
    }

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;
    [SerializeField] protected Transform body;
    [SerializeField] protected Transform foot;

    [Header("Ground Movemnent")]
    [SerializeField] private float groundMoveSpeed = 10;
    [SerializeField] private InterpolatedFloat groundMoveTime = new(1, 0.2f);

    // Counter movement
    private bool _isStatic = true;
    [SerializeField] private InterpolatedFloat staticDesacellerationTime = new(1, 0.35f);

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 20;

    [Header("Jump")]
    // Forces
    [SerializeField] private float gravityMultiplier = 1;
    [Range(0, 1)]
    [SerializeField] private float gravityPercentWhileJumping = 0.75f;
    [SerializeField] private float jumpForce = 20;
    private bool _isJumping = false;
    private bool _canJump = false;

    // Checks
    private bool _isGrounded = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float changeIsGroundedStateDelay = 2;
    private Coroutine _cancellingGrounded;
    private Vector3 _normalVector;
    // Pre-groud sphere
    [SerializeField] private float preGroundSphereRadius = 0.5f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 10;
    [SerializeField] private float dashCooldown = 1;
    [SerializeField] private bool canDash = true;

    // Slide while dashing
    [SerializeField] private bool dashState = false;
    [SerializeField] private float dashStateDuration = 0.2f;

    [Header("Inputs")]
    private Vector2 _keyboard;

    [Header("Orientation")]
     private Vector3 _planeOrientationForward;
     private Vector3 _planeOrientationRight;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInputs();
        GetOrientation();
        GroundMoveManagement();
        JumpManagement();

        Debug.Log(rb.velocity.magnitude);
    }

    void FixedUpdate()
    {
        Gravity();
        AirMove();
    }

    void GetInputs()
    {
        // Movement
        _keyboard.x = Input.GetAxisRaw("Horizontal");
        _keyboard.y = Input.GetAxisRaw("Vertical");
        _keyboard.Normalize();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && _canJump)
            Jump();

        // Dash
        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            Dash();
    }

    void GetOrientation()
    {
        // Get orientation without y
        _planeOrientationForward = RemoveYFromVector(orientation.transform.forward);
        _planeOrientationRight = RemoveYFromVector(orientation.transform.right);
    }

    #region GroundMovement

    void GroundMoveManagement()
    {
        _isStatic = _keyboard == Vector2.zero;

        // Check if player is 'static-intetion'
        if (_isGrounded)
        {
            if (!_isStatic)
            {
                staticDesacellerationTime.Reset();
                staticDesacellerationTime.SetDuration(_stopDefaultDuration);
                GroundMove();
            }
            else if(!dashState)
            {
                groundMoveTime.Reset();
                StaticStop();
            }
        }
    }

    void GroundMove()
    {
        groundMoveTime.Update(Time.deltaTime);
        Vector3 maxVelocity = (_planeOrientationForward * _keyboard.y + _planeOrientationRight * _keyboard.x) * groundMoveSpeed;
        Vector3 moveVelocity = Vector3.Lerp(Vector3.zero, maxVelocity, groundMoveTime.GetValue());
        rb.velocity = moveVelocity + new Vector3(0, rb.velocity.y, 0);
    }

    private Vector3 _staticStartVelocity;
    private float _stopExtraDuration;
    private float _stopDefaultDuration;
    void StaticStop()
    {
        // Stop player logic
        if (staticDesacellerationTime.GetValue() == 0)
        {
            _staticStartVelocity = rb.velocity;

            _stopDefaultDuration = staticDesacellerationTime.GetDuration();
            _stopExtraDuration = GetStopExtraTime(rb.velocity.magnitude);
            staticDesacellerationTime.SetDuration(_stopDefaultDuration + _stopExtraDuration);
        }

        staticDesacellerationTime.Update(Time.deltaTime);
        rb.velocity = Vector3.Lerp(_staticStartVelocity, new Vector3(0, rb.velocity.y, 0), staticDesacellerationTime.GetValue());
    }

    float GetStopExtraTime(float x) => (4 / (1 + Mathf.Pow((float) Math.E, (float) -0.08 * (x - 60))));

    #endregion

    #region Jump

    void JumpManagement()
    {
        _isJumping = IsJumping();
        _canJump = _isGrounded || CollidersCointainsGround(Physics.OverlapSphere(foot.position, preGroundSphereRadius));
    }

    bool CollidersCointainsGround(Collider[] colliders)
    {
        foreach (Collider c in colliders)
        {
            if (IsGroundLayer(c.gameObject.layer))
                return true;
        }
        return false;
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, Mathf.Infinity), rb.velocity.z);
        rb.AddForce(body.transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(_normalVector * 0.5f * jumpForce, ForceMode.Impulse);
    }

    bool IsJumping() => (rb.velocity.y > 0 && Input.GetKey(KeyCode.Space) && !_isGrounded);

    void DrawPreGroundSphere()
    {
        if(foot == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(foot.position, preGroundSphereRadius);
    }

    void Gravity()
    {
        if(_isJumping)
            rb.AddForce(Physics.gravity * gravityMultiplier * gravityPercentWhileJumping);
        else 
            rb.AddForce(Physics.gravity * gravityMultiplier);
    }

    #endregion

    #region AirMovement
    private void AirMove()
    {
        if (_isGrounded) return;
        
        // Foward
        rb.AddForce(_planeOrientationForward * _keyboard.y * airMoveSpeed);
        // Sideways
        rb.AddForce(_planeOrientationRight * _keyboard.x * airMoveSpeed);
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

    #region Dash

    public void Dash()
    {
        Vector3 foward = orientation.transform.forward;
        rb.velocity = new Vector3(foward.x, foward.y * 1.5f, foward.z) * rb.velocity.magnitude;
        rb.AddForce(foward * dashForce, ForceMode.Impulse);
        canDash = false;
        dashState = true;
        Invoke(nameof(ExitDashState), dashStateDuration);
        Invoke(nameof(EnableDash), dashCooldown);
    }

    void EnableDash() => canDash = true;

    void ExitDashState() => dashState = false;

    #endregion

    private void OnCollisionStay(Collision other)
    {
        // Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (!IsGroundLayer(layer)) return;

        // get if ground is working as a floor
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                _isGrounded = true;
                _normalVector = normal;
                if (_cancellingGrounded != null)
                    StopCoroutine(_cancellingGrounded);
            }
        }

        // Invoke ground/wall cancel, since we can't check normals with CollisionExit
        _cancellingGrounded = StartCoroutine(ChangeIsGroundedState(changeIsGroundedStateDelay, false));
    }

    bool IsGroundLayer(int layer) => groundLayer == (groundLayer | (1 << layer));

    bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < 75;
    }

    IEnumerator ChangeIsGroundedState(float delay, bool newState)
    {
        yield return new WaitForSeconds(delay);
        _isGrounded = newState;
    }

    void OnDrawGizmosSelected()
    {
        DrawPreGroundSphere();
    }
}