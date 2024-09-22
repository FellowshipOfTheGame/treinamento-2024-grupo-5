using System;
using System.Collections;
using UnityEngine;
using Mantega;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform orientation;
    [SerializeField] protected Transform body;
    [SerializeField] protected Transform foot;
    [SerializeField] private FirstPerson firstPerson;

    [Header("Ground Movemnent")]
    [SerializeField] private float groundMoveSpeed = 10;
    [SerializeField] private InterpolatedFloat groundMoveTime = new(1, 0.2f);
    [SerializeField]  float groundMoveDuration;

    // Counter movement
    private bool _isStatic = true;
    [SerializeField] private InterpolatedFloat _staticDesacellerationTime = new(1, 0.35f);
    [SerializeField] private float stopDefaultDuration;

    [Header("Air Movement")]
    [SerializeField] private float airMoveSpeed = 15;
    [SerializeField] private float airDesaccelerateSpeed;
    [SerializeField] private float airMoveMaxSpeed = 70;

    // Counter movement
    [SerializeField] private InterpolatedFloat airMoveDesacellerationTime = new(1, 0.35f);

    [Header("Jump")]
    // Forces
    [SerializeField] private float gravityMultiplier = 1;
    [Range(0, 1)]
    [SerializeField] private float gravityPercentWhileJumping = 0.75f;
    [SerializeField] private float jumpForce = 15;
    [SerializeField] private bool _isJumping = false;
    [SerializeField] private bool _canJump = false;

    // Checks
    [SerializeField]  private bool _isGrounded = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float changeIsGroundedStateDelay = 0.1f;
    private Coroutine _cancellingGrounded;
    private Vector3 _normalVector;
    // Pre-groud sphere
    [SerializeField] private float preGroundSphereRadius = 0.5f;

    [Header("Walljump")]
    // Checks
    [SerializeField] private bool _isTouchingWall;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float changeWallSlideStateDelay = 0.1f;
    private Coroutine _cancellingWallslide;
    private Vector3 _wallslideNormalVector;
    // Jump
    [SerializeField] private float walljumpForce = 15;
    // Slide
    [SerializeField] private float wallslideMaxFallVelocity = 3.5f;
    
    // Apenas apos matar um inimigo
    [Header("Dash")]
    [SerializeField] private float dashForce = 15;
    [SerializeField] private float dashCooldown = 1;
    [SerializeField] private bool canDash = true;

    // Slide while dashing
    [SerializeField] private bool _isDashing = false;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Slide")]
    [SerializeField] private float slideForce = 15;
    [SerializeField] private float slideCooldown = 1;
    [SerializeField] private bool canSlide = true;


    [Header("Camera Effects")]
    // Rotation
    [SerializeField] private InterpolatedFloat cameraZRotationEffect = new(0, 0.2f);
    [SerializeField] private float cameraTargetZRotation = 3;
    // FOV
    [SerializeField] private InterpolatedFloat fovChangeEffect = new(50f, 0.2f, 70f);
    [SerializeField] private float fovChangeEffectDashTarget = 50;

    [Header("Inputs")]
    private Vector2 _keyboard;

    [Header("Orientation")]
    private Vector3 _planeOrientationForward;
    private Vector3 _planeOrientationRight;

    private void Start()
    {
        if(rb == null)
            Generics.ReallyTryGetComponent(gameObject, out rb);

        _staticDesacellerationTime.SetDuration(stopDefaultDuration);
        groundMoveTime.SetDuration(groundMoveDuration);

        if (firstPerson == null)
            Generics.ReallyTryGetComponent(gameObject, out firstPerson);
        fovChangeEffect.SetStartValue(firstPerson.baseFOV);
    }

    private void Update()
    {
        GetOrientation();
        GetInputs();

        GroundMoveManagement();
        JumpManagement();
        SlideWall();
        LimitAirMove();

        RotateCameraEffect();
        FOVCameraEffect();
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

        _isStatic = _keyboard == Vector2.zero;

        // Walljump or Jump
        if (Input.GetKeyDown(KeyCode.Space))
            if (_canJump)
                Jump();
            else if (_isTouchingWall)
                Walljump(); 

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            Dash();
    }

    void GetOrientation()
    {
        // Get orientation without y
        _planeOrientationForward = RemoveYFromVector(orientation.transform.forward);
        _planeOrientationRight = RemoveYFromVector(orientation.transform.right);
    }
    
    Vector2 GetCordinatesRelativeToOrientation(Vector2 v)
    {
        Vector2 X = new Vector2(_planeOrientationRight.x, _planeOrientationRight.z);
        Vector2 Y = new Vector2(_planeOrientationForward.x, _planeOrientationForward.z);
        return Generics.ChangeVectorCordinates(v, X, Y);
    }

    Vector2 PlaneVelocityRelativeToOrientation() => GetCordinatesRelativeToOrientation(new Vector2(rb.velocity.x, rb.velocity.z));

    Vector3 RemoveYFromVector(Vector3 vector)
    {
        vector.y = 0;
        return vector;
    }

    #region GroundMovement

    void GroundMoveManagement()
    {
        // Check if player is 'static-intetion'
        if (_isGrounded && !_isJumping && !_isDashing)
        {
            if (!_isStatic)
            {
                _staticDesacellerationTime.Reset();
                GroundMove();
            }
            else
            {
                groundMoveTime.Reset();
                StaticStop();
            }
        }
        else
        {
            _staticDesacellerationTime.Reset();
            groundMoveTime.Reset();
        }
    }


    Vector3 _startGroundMoveVelocity;
    void GroundMove()
    {
        if(groundMoveTime.GetValue() == 0)
            StartGroundMove();

        Vector3 maxVelocity = (_planeOrientationForward * _keyboard.y + _planeOrientationRight * _keyboard.x) * groundMoveSpeed;
        groundMoveTime.Update(Time.deltaTime);
        Vector3 moveVelocity = Vector3.Lerp(_startGroundMoveVelocity, maxVelocity, groundMoveTime.GetValue());
        rb.velocity = moveVelocity + new Vector3(0, rb.velocity.y, 0);
    }

    void StartGroundMove()
    {
        _startGroundMoveVelocity = rb.velocity;
        _startGroundMoveVelocity.y = 0;
        groundMoveTime.SetDuration(groundMoveDuration + GetGroundMoveExtraTime(rb.velocity.magnitude));
    }

    float GetGroundMoveExtraTime(float x) => (4 / (1 + Mathf.Pow((float)Math.E, -0.06f * (x - 70))));

    private Vector3 _staticStartVelocity;
    private float _stopExtraDuration;
    void StaticStop()
    {
        // Stop player logic
        if (_staticDesacellerationTime.GetValue() == 0)
            StartStaticStop();

        _staticDesacellerationTime.Update(Time.deltaTime);
        rb.velocity = new Vector3(0, rb.velocity.y, 0) + Vector3.Lerp(_staticStartVelocity, Vector3.zero, _staticDesacellerationTime.GetValue());
    }

    void StartStaticStop()
    {
        _staticStartVelocity = rb.velocity;
        _staticStartVelocity.y = 0;
        _stopExtraDuration = GetStaticStopExtraTime(rb.velocity.magnitude);
        _staticDesacellerationTime.SetDuration(stopDefaultDuration + _stopExtraDuration);
    }

    float GetStaticStopExtraTime(float x) => (4 / (1 + Mathf.Pow((float) Math.E, -0.08f * (x - 60))));

    #endregion

    #region Jump

    void JumpManagement()
    {
        _isJumping = IsJumping();
        _canJump = !_jumpCooldownState && (_isGrounded || CollidersCointainsGround(Physics.OverlapSphere(foot.position, preGroundSphereRadius)));
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

    private float _jumpCooldown = 0.2f;
    private bool _jumpCooldownState = false;
    void Jump()
    {
        _jumpCooldownState = true;
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, Mathf.Infinity), rb.velocity.z);
        rb.AddForce(body.transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(_normalVector * 0.5f * jumpForce, ForceMode.Impulse);
        Invoke(nameof(JumpCooldown), _jumpCooldown);
    }

    void JumpCooldown() => _jumpCooldownState = false;

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

    #region Walljmup
    void Walljump() => rb.AddForce((_wallslideNormalVector + body.up * 0.7f) * walljumpForce, ForceMode.Impulse);

    void SlideWall()
    {
        if (_isTouchingWall)
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallslideMaxFallVelocity, Mathf.Infinity), rb.velocity.z);
    }

    void StartWallslide(Collision other)
    {
        rb.AddForce(body.transform.up * other.impulse.magnitude * 0.4f, ForceMode.Impulse);
    }

    #endregion

    #region AirMovement
    private void AirMove()
    {
        if (_isGrounded || _isStatic) return;

        Vector2 airVelocity = new Vector2(rb.velocity.x, rb.velocity.z);
        Vector3 foward = _planeOrientationForward * _keyboard.y;
        Vector3 sideways = _planeOrientationRight * _keyboard.x;

        airMoveSpeed = GetAirMoveSpeed(Mathf.Clamp(airVelocity.magnitude, 0, 60));
        airDesaccelerateSpeed = GetAirDesaccelerationMoveSpeed(Mathf.Clamp(airVelocity.magnitude, 0, 60));
        
        // Foward
        if (Vector3.Angle(foward, rb.velocity) > 90f)
            rb.AddForce(foward * airDesaccelerateSpeed);
        else
            rb.AddForce(foward * airMoveSpeed);

        Debug.Log($"Foward: {Vector3.Angle(foward, rb.velocity)}");

        // Sideways
        if (Vector3.Angle(sideways, rb.velocity) > 90f)
            rb.AddForce(sideways * airDesaccelerateSpeed * 0.75f);
        else
            rb.AddForce(sideways * airMoveSpeed * 0.75f);

        Debug.Log($"Sideways: {Vector3.Angle(sideways, rb.velocity)}");
    }

    float GetAirMoveSpeed(float x) => (19 / (1 + Mathf.Pow((float)Math.E, 0.1f * (x - 30))));
    float GetAirDesaccelerationMoveSpeed(float x) => (7 + 20 / (1 + Mathf.Pow((float)Math.E, -0.05f * (x - 15))));

    void LimitAirMove()
    {
        if (_isGrounded)
            return;

        Vector3 planeVelocity = RemoveYFromVector(rb.velocity);
        rb.velocity = planeVelocity.normalized * Mathf.Clamp(planeVelocity.magnitude, 0, airMoveMaxSpeed) + new Vector3(0, rb.velocity.y, 0);
    }

    #endregion

    #region Dash

    public void Dash()
    {
        Vector3 dashDirection;

        // Apply force
        if (_keyboard != Vector2.zero)
            dashDirection = _planeOrientationForward * _keyboard.y + _planeOrientationRight * _keyboard.x;
        else
            dashDirection = _planeOrientationForward;

        rb.velocity = dashDirection * dashForce + new Vector3(0, rb.velocity.y, 0);
        // Manage states
        canDash = false;
        _isDashing = true;
        Invoke(nameof(ExitDashState), dashDuration);
        Invoke(nameof(EnableDash), dashCooldown);
    }

    void EnableDash() => canDash = true;

    void ExitDashState() => _isDashing = false;

    #endregion

    #region Effects

    void RotateCameraEffect()
    {
        float target = Generics.Sign(_keyboard.x) * -cameraTargetZRotation;
        // Check if Change target
        if(target != cameraZRotationEffect.GetTarget())
        {
            cameraZRotationEffect.SetStartValue(cameraZRotationEffect.GetValue());
            cameraZRotationEffect.SetTarget(target);
            cameraZRotationEffect.Reset();
        }

        cameraZRotationEffect.Update(Time.deltaTime);
        firstPerson.SetZRotation(cameraZRotationEffect.GetValue());
    }

    void FOVCameraEffect()
    {
        float newTarget;
        if (_isDashing)
        {
            newTarget = fovChangeEffectDashTarget;
            fovChangeEffect.SetDuration(dashDuration);
        }
        else
        {
            newTarget = firstPerson.baseFOV;
            fovChangeEffect.SetDuration(dashDuration * 2f);
        }

        fovChangeEffect.Update(Time.deltaTime);
        if (fovChangeEffect.GetTarget() != newTarget)
        {
            fovChangeEffect.SetStartValue(fovChangeEffect.GetValue());
            fovChangeEffect.SetDuration(fovChangeEffect.GetDuration() - Time.deltaTime);
            fovChangeEffect.SetTarget(newTarget);
            fovChangeEffect.Reset();
        }

        firstPerson.SetFOV(fovChangeEffect.GetValue());
    }

    #endregion

    #region Collision
    private void OnCollisionStay(Collision other)
    {
        // Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;

        if (IsGroundLayer(layer)) Ground(other);
        if (IsWallLayer(layer)) Wall(other);
        
    }

    void Ground(Collision other)
    {
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
                break;
            }
        }

        // Invoke ground/wall cancel, since we can't check normals with CollisionExit
        _cancellingGrounded = StartCoroutine(ChangeIsGroundedState(changeIsGroundedStateDelay, false));
    }

    void Wall(Collision other)
    {
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsWall(normal))
            {
                _isTouchingWall = true;
                _wallslideNormalVector = normal;
                if (_cancellingWallslide != null)
                    StopCoroutine(_cancellingWallslide);
                break;
            }
        }

        _cancellingWallslide = StartCoroutine(ChangeIsSlidingWallState(changeWallSlideStateDelay, false));
    }

    private void OnCollisionEnter(Collision other)
    {
        int layer = other.gameObject.layer;
        if (IsGroundLayer(layer))
            HitGround(other);
        if (IsWallLayer(layer))
            HitWall(other);
    }

    void HitGround(Collision other)
    {
        // get if ground is working as a floor
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                StartGroundMove();
                break;
            }
        }
    }

    void HitWall(Collision other)
    {
        // get if ground is working as a floor
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsWall(normal))
            {
                StartWallslide(other);
                break;
            }
        }
    }

    bool IsGroundLayer(int layer) => groundLayer == (groundLayer | (1 << layer));

    bool IsWallLayer(int layer) => wallLayer == (wallLayer | (1 << layer));

    bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < 75;
    }

    bool IsWall(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle > 80;
    }

    IEnumerator ChangeIsGroundedState(float delay, bool newState)
    {
        yield return new WaitForSeconds(delay);
        _isGrounded = newState;
    }

    IEnumerator ChangeIsSlidingWallState(float delay, bool newState)
    {
        yield return new WaitForSeconds(delay);
        _isTouchingWall = newState;
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        DrawPreGroundSphere();
    }
}

