using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RigidbodyCharacterEngine : MonoBehaviour
{
    private Rigidbody m_rb;
    private Vector3 m_gravitationalForce;
    private Vector3 m_rayDir = Vector3.down;
    private Vector2 m_moveContext;

    private bool m_shouldMaintainHeight = true;

    [Header("Levitation Control:")]
    [SerializeField] private float m_levitateHeight = 1.75f; 
    [SerializeField] private float m_rayToGroundLength = 3f; 
    [SerializeField] public float m_rideSpringStrength = 50f; 
    [SerializeField] private float m_rideSpringDamper = 5f; 


    private Vector3 m_moveInput;
    private float m_speedFactor = 1f;
    private float m_maxAccelForceFactor = 1f;
    private Vector3 m_GoalVel = Vector3.zero;

    [Header("Movement:")]
    [SerializeField] private AnimationCurve m_accelerationFactorFromDot;
    [SerializeField] private AnimationCurve m_maxAccelerationForceFactorFromDot;
    [SerializeField] private Vector3 m_moveForceScale = new Vector3(1f, 0f, 1f);
    [SerializeField] private Locomotion groundLocomotion;
    [SerializeField] private Locomotion airLocomotion;
    [SerializeField] private Locomotion sprintLocomotion;


    private Vector3 m_jumpInput;
    private float m_timeSinceJumpPressed = 0f;
    private float m_timeSinceUngrounded = 0f;
    private float m_timeSinceJump = 0f;
    private bool m_jumpReady = true;
    private bool m_isJumping = false;

    [Header("Jump:")]
    [SerializeField] private float m_jumpForceFactor = 10f;
    [SerializeField] private float m_riseGravityFactor = 5f;
    [SerializeField] private float m_fallGravityFactor = 10f; 
    [SerializeField] private float m_lowJumpFactor = 2.5f;
    [SerializeField] private float m_jumpBuffer = 0.15f; 
    [SerializeField] private float m_coyoteTime = 0.25f;
    [SerializeField] private LayerMask m_whatIsGround;

    [System.Serializable]
    public class Locomotion
    {
        public float maxSpeed = 8f;
        public float acceleration = 200f;
        public float maxAccelForce = 150f;
        public Vector3 moveForceScale = new Vector3(1f, 0f, 1f);
    }

    private bool _prevGrounded = false;

    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_gravitationalForce = Physics.gravity * m_rb.mass;
    }

    public void GetMovementInput(Vector3 _movement)
    {
        m_moveInput = _movement;
    }

    public void GetJumpInput(float _timeSinceJumpPressed)
    {
        m_timeSinceJumpPressed = _timeSinceJumpPressed;
        m_jumpInput = new Vector3(0, 1, 0);
    }

    private bool CheckIfGrounded(bool rayHitGround, RaycastHit rayHit)
    {
        bool grounded;
        if (rayHitGround == true)
        {
            grounded = rayHit.distance <= m_levitateHeight * 1.3f; // 1.3f allows for greater leniancy (as the value will oscillate about the rideHeight).
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }


    private void FixedUpdate()
    {
        (bool rayHitGround, RaycastHit rayHit) = RaycastToGround();
        Locomotion locomotion = airLocomotion;

        bool grounded = CheckIfGrounded(rayHitGround, rayHit);

        if (grounded == true)
        {
            locomotion = groundLocomotion;
            m_timeSinceUngrounded = 0f;
            if (m_timeSinceJump > 0.2f)
            {
                m_isJumping = false;
            }
        }

        else
        {
            m_timeSinceUngrounded += Time.fixedDeltaTime;
        }

        CharacterMove(m_moveInput, rayHit, locomotion);
        
        HandleJumpAndFall(m_jumpInput, grounded, rayHit);

        if (rayHitGround && m_shouldMaintainHeight)
        {
            MaintainHeight(rayHit);
        }
        _prevGrounded = grounded;
    }


    private (bool, RaycastHit) RaycastToGround()
    {
        RaycastHit rayHit;
        Ray rayToGround = new Ray(transform.position, m_rayDir);
        bool rayHitGround = Physics.Raycast(rayToGround, out rayHit, m_rayToGroundLength, m_whatIsGround);
        return (rayHitGround, rayHit);
    }

    private void MaintainHeight(RaycastHit rayHit)
    {
        Vector3 vel = m_rb.velocity;
        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }
        float rayDirVel = Vector3.Dot(m_rayDir, vel);
        float otherDirVel = Vector3.Dot(m_rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;
        float currHeight = rayHit.distance - m_levitateHeight;
        float springForce = (currHeight * m_rideSpringStrength) - (relVel * m_rideSpringDamper);
        Vector3 maintainHeightForce = -m_gravitationalForce + springForce * Vector3.down;
        m_rb.AddForce(maintainHeightForce);

        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(-maintainHeightForce, rayHit.point);
        }
    }




    /// <summary>
    /// Apply forces to move the character up to a maximum acceleration, with consideration to acceleration graphs.
    /// </summary>
    /// <param name="moveInput">The player movement input.</param>
    /// <param name="rayHit">The rayHit towards the platform.</param>
    private void CharacterMove(Vector3 moveInput, RaycastHit rayHit, Locomotion locomotion)
    {
        Vector3 m_UnitGoal = moveInput;
        Vector3 unitVel = m_GoalVel.normalized;
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
        float accel = locomotion.acceleration * m_accelerationFactorFromDot.Evaluate(velDot);
        Vector3 goalVel = m_UnitGoal * locomotion.maxSpeed * m_speedFactor;
        m_GoalVel = Vector3.MoveTowards(m_GoalVel,
                                        goalVel,
                                        accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (m_GoalVel - m_rb.velocity) / Time.fixedDeltaTime;
        float maxAccel = locomotion.maxAccelForce * m_maxAccelerationForceFactorFromDot.Evaluate(velDot) * m_maxAccelForceFactor;
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        m_rb.AddForceAtPosition(Vector3.Scale(neededAccel * m_rb.mass, m_moveForceScale), transform.position); // Using AddForceAtPosition in order to both move the player and cause the play to lean in the direction of input.
    }

    private void CollisionInPlayersPath()
    {

    }


    private void HandleJumpAndFall(Vector3 jumpInput, bool grounded, RaycastHit rayHit)
    {
        m_timeSinceJumpPressed += Time.fixedDeltaTime;
        m_timeSinceJump += Time.fixedDeltaTime;

        HandleJump(jumpInput, grounded, rayHit);
        
        if (m_rb.velocity.y < 0)
        {
            HandleFall(grounded);
        }
        else if (m_rb.velocity.y > 0)
        {
            //Do something
        }
    }

    private void HandleJump(Vector3 jumpInput, bool grounded, RaycastHit rayHit)
    {
        if (m_timeSinceJumpPressed < m_jumpBuffer)
        {
            if (m_timeSinceUngrounded < m_coyoteTime)
            {
                if (m_jumpReady)
                {
                    m_jumpReady = false;
                    m_shouldMaintainHeight = false;
                    m_isJumping = true;
                    m_rb.velocity = new Vector3(m_rb.velocity.x, 0f, m_rb.velocity.z); // Cheat fix... (see comment below when adding force to rigidbody).
                    if (rayHit.distance != 0) // i.e. if the ray has hit
                    {
                        m_rb.position = new Vector3(m_rb.position.x, m_rb.position.y - (rayHit.distance - m_levitateHeight), m_rb.position.z);
                    }
                    m_rb.AddForce(Vector3.up * m_jumpForceFactor, ForceMode.Impulse); // This does not work very consistently... Jump height is affected by initial y velocity and y position relative to RideHeight... Want to adopt a fancier approach (more like PlayerMovement). A cheat fix to ensure consistency has been issued above...
                    m_timeSinceJumpPressed = m_jumpBuffer; // So as to not activate further jumps, in the case that the player lands before the jump timer surpasses the buffer.
                    m_timeSinceJump = 0f;
                }
            }
        }
    }

    private void HandleFall(bool grounded)
    {
            m_shouldMaintainHeight = true;
            m_jumpReady = true;
            if (!grounded)
            {
                // Increase downforce for a sudden plummet.
                m_rb.AddForce(m_gravitationalForce * (m_fallGravityFactor - 1f)); // Hmm... this feels a bit weird. I want a reactive jump, but I don't want it to dive all the time...
            }
    }
}

/*
 * using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Timeline;

public class RigidbodyCharacterCapsuleFirstPerson : MonoBehaviour
{
    [Header("Ground offset")]
    [SerializeField] private float m_stepHeight = 2.2f;
    [SerializeField] private float m_levitateHeight = 2;
    [SerializeField] private float m_levitateStrength;
    [SerializeField] private float m_levitateDamper;
    [SerializeField] private LayerMask m_whatIsGround;

    [Header("Jump")]
    [SerializeField] private float m_jumpBufferTime = 0.25f;
    [SerializeField] private float m_jumpSpeed = 10;

    [Header("Ground Locomotion")]
    [SerializeField] private Locomotion groundLocomotion;

    [Header("Sprint Locomotion")]
    [SerializeField] private bool hasSprint = true;
    [SerializeField] private Locomotion sprintLocomotion;

    [Header("Air Control")]
    [SerializeField] private float m_coyoteTime = 0.2f;
    [SerializeField] private Locomotion airLocomotion;

    [System.Serializable]
    public class Locomotion
    {
        public float m_maxSpeed;
        public float m_acceleration;
        public AnimationCurve m_accelerationFromDirDiff;
        public float m_maxAccelerationForce;
        public AnimationCurve m_maxAccelerationForceFromDirDiff;
        public Vector3 m_forceScale;
        public float m_speedFactor;
    }


    private Vector3 m_inputs;
    private float m_jumpBufferTimer = 0f;
    private bool m_sprint;
    private float m_coyoteTimer;
    private Rigidbody m_rigidbody;
    private bool m_isGrounded;
    private Vector3 m_GoalVelocity;

    private bool m_shouldLevitate;

    private float m_timeSinceUngrounded = 0;
    private float _timeSinceJump = 0;

    private bool m_isJumping = false;

    bool formerGroundState = false;

    private Vector3 latestLevitateForce = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        m_jumpBufferTimer -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            m_jumpBufferTimer = m_jumpBufferTime;
        }

        m_sprint = Input.GetButton("Sprint");
        m_inputs.x = Input.GetAxisRaw("Horizontal");
        m_inputs.z = Input.GetAxisRaw("Vertical");
        m_inputs = m_inputs.normalized;
    }

    void FixedUpdate()
    {

        Vector3 referenceUp = transform.up;

        (bool hitGround, RaycastHit hit) = RaycastToGround(transform.position, transform.up*-1, m_stepHeight, m_whatIsGround);
        //SetPlatform(hit)
        m_isGrounded = hitGround;


        if ((m_isGrounded || m_coyoteTimer>0) && m_jumpBufferTimer > 0)
        {
            Jump(hitGround, hit);
            print("Jump");
            print("pos: " + transform.position);
            print("velocity: " + m_rigidbody.velocity.y);
        }

        if (m_isGrounded) 
        {
            m_coyoteTimer = m_coyoteTime;
            referenceUp = hit.normal;
        }
        else if (m_coyoteTimer > 0)
        {
            m_coyoteTimer -= Time.fixedDeltaTime;
            referenceUp = transform.up;
        }

        if (m_isGrounded)
        {
            if (m_shouldLevitate == true) Levitate(hit.distance);
            if (m_sprint && hasSprint) Move(m_inputs, referenceUp, sprintLocomotion);
            else Move(m_inputs, referenceUp, groundLocomotion);
        }
        else
        {
            Move(m_inputs, referenceUp, airLocomotion);
        }

        if(m_rigidbody.velocity.y < 0)
        {
            m_shouldLevitate = true;
        }

        formerGroundState = m_isGrounded;
    }


    public (bool, RaycastHit) RaycastToGround(Vector3 rayOrigin, Vector3 rayDirection, float rayDistance, LayerMask whatIsGround)
    {
        RaycastHit _hit;
        Ray _rayToGround = new Ray(rayOrigin, rayDirection);
        bool _rayHitGround = Physics.Raycast(_rayToGround, out _hit, rayDistance, whatIsGround);
        return (_rayHitGround, _hit);
    }

    public void Levitate(float groundDistance)
    {
        Vector3 _velocity = m_rigidbody.velocity;
        Vector3 _rayDirection = transform.up * -1;

        Vector3 _otherVel = Vector3.zero;
        Rigidbody hitBody = null;
  
        float _rayDirVelocity = Vector3.Dot(_rayDirection, _velocity);
        float _otherDirVelocity = Vector3.Dot(_rayDirection, _otherVel);

        float _relativeVelocity = Vector3.Dot(_rayDirection, _velocity);

        float _distance = groundDistance - m_levitateHeight;

        float _springForce = (_distance * m_levitateStrength) - (_relativeVelocity * m_levitateDamper);

        m_rigidbody.AddForce(-transform.up * _springForce);

        latestLevitateForce = -transform.up * _springForce;
    }

    public void Jump (bool _hitground, RaycastHit _hit)
    {
        m_jumpBufferTimer = 0;
        Physics.Simulate(Time.fixedDeltaTime);
        if (_hitground)
        {
            transform.position = new Vector3(transform.position.x, _hit.point.y + m_levitateHeight, transform.position.z);
            print("Desired Position: " + _hit.point.y + m_levitateHeight);
        }
        m_rigidbody.velocity = Vector3.ProjectOnPlane(m_rigidbody.velocity, Vector3.up);
        m_rigidbody.velocity += transform.up * m_jumpSpeed;
        m_rigidbody.AddForce(-latestLevitateForce);
        m_coyoteTimer = 0;
        m_isJumping = true;
        m_shouldLevitate = false;
    }

    public void Move(Vector3 _inputs, Vector3 _upReference, Locomotion _locomotion)
    {
        _inputs = transform.forward * _inputs.z + transform.right * _inputs.x;

        Vector3 _inputProjection = Vector3.ProjectOnPlane(_inputs, _upReference);
        Vector3 _speedInUpDirection = Vector3.Project(m_rigidbody.velocity, _upReference);
        Vector3 _unitVelocity = m_GoalVelocity.normalized;

        float _velocityDot = Vector3.Dot(_inputs, _unitVelocity);
        float _accel = _locomotion.m_acceleration * _locomotion.m_accelerationFromDirDiff.Evaluate(_velocityDot);

        Vector3 _goalVelocity = _inputProjection * _locomotion.m_maxSpeed * _locomotion.m_speedFactor + _speedInUpDirection;
        m_GoalVelocity = Vector3.MoveTowards(m_GoalVelocity, _goalVelocity, _accel * Time.fixedDeltaTime);

        Vector3 _neededAcceleration = (m_GoalVelocity - m_rigidbody.velocity) / Time.fixedDeltaTime;

        float _maxAccel = (_locomotion.m_maxAccelerationForce * _locomotion.m_maxAccelerationForceFromDirDiff.Evaluate(_velocityDot));

        _neededAcceleration = Vector3.ClampMagnitude(_neededAcceleration, _maxAccel);

        m_rigidbody.AddForce(_neededAcceleration * m_rigidbody.mass);
    }
}
*/