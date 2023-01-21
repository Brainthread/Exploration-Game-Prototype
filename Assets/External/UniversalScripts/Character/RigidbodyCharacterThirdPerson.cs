using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCharacterThirdPerson : MonoBehaviour
{
    [Header("Walking/running")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private bool hasSprint = false;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float directionSwitchForce = 4f;
    [SerializeField] private float directionSwitchThreshold = -0.45f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int airJumpNumber = 1;
    [SerializeField] private bool forceAlwaysAirjump = false;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Determining if on ground")]
    [SerializeField] private float groundRadius = 0.2f;
    [SerializeField] private LayerMask ground;


    [Header("Movement in Air")]
    [SerializeField] private float airControlModifier = 0.1f;
    [SerializeField] private float smoothingMaxDelta = 0.5f;
    [SerializeField] private float maxSlope = 45;
    [SerializeField] private float rayRange = 1.5f;

    private Vector3 m_inputs = Vector3.zero;

    private Rigidbody m_body;

    private float m_speed;

    private float m_coyoteTime;
    private int m_jumps;

    private bool m_formerGroundState = true;
    private Transform m_groundChecker;
    [SerializeField] private bool m_isGrounded = true;



    private Vector3 m_oldInputValues;

    private RaycastHit slopeHit;

    private Vector3 m_implementVelocity;
    private Vector3 m_oldImplementVelocity;
    public enum IsGroundedOverride
    {
        DISABLED,
        GROUNDED,
        NOT_GROUNDED
    };
    private IsGroundedOverride isGroundedOverride = IsGroundedOverride.DISABLED;

    void Start()
    {
        m_body = GetComponent<Rigidbody>();
        m_groundChecker = transform.GetChild(0);
        m_coyoteTime = 0;
        m_speed = walkSpeed;
    }

    void Update()
    {
        m_inputs = Vector3.zero;
        m_inputs.x = Input.GetAxisRaw("Horizontal");
        m_inputs.z = Input.GetAxisRaw("Vertical");

        if (hasSprint)
        {
            if (Input.GetButtonDown("Sprint"))
            {
                m_speed = sprintSpeed;
            }
            if (Input.GetButtonUp("Sprint"))
            {
                m_speed = walkSpeed;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (m_isGrounded || m_coyoteTime > 0 || CanAirJump())
            {
                Vector3 _jumpVel = m_body.velocity;
                _jumpVel.y = 0;
                m_body.velocity = _jumpVel;
                m_body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                m_coyoteTime = 0;
                if (!m_isGrounded && m_coyoteTime <= 0)
                {
                    m_jumps--;
                }
            }
        }
    }

    bool OnSlope()
    {
        Debug.DrawLine(transform.position, transform.position - Vector3.up * rayRange, Color.green);
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, rayRange, ground))
        {
            if (slopeHit.normal != Vector3.up) return true;
        }
        return false;
    }

    bool WithinSlopeLimits()
    {
        float _angle = Vector3.Angle(Vector3.up, slopeHit.normal);
        return _angle < maxSlope;
    }

    public void SetGroundedOverride(IsGroundedOverride over)
    {
        isGroundedOverride = over;
    }

    public float GetMaxMovementMagnitude()
    {
        return sprintSpeed;
    }

    bool CanAirJump()
    {
        if (airJumpNumber > 0 && m_jumps > 0)
        {
            if (forceAlwaysAirjump)
                return true;
        }
        return false;
    }

    void SwitchGroundState()
    {
        m_formerGroundState = m_isGrounded;
        if (!m_isGrounded)
        {
            if (m_jumps == airJumpNumber) m_coyoteTime = coyoteTime;
            m_body.useGravity = true;
        }
        if (m_isGrounded)
        {
            //m_implementVelocity.y = 0f;
            //m_body.velocity = m_implementVelocity;
            m_jumps = airJumpNumber;
            m_body.useGravity = false;
        }
    }

    public bool IsGrounded()
    {
        return m_isGrounded;
    }

    public bool CheckIfGrounded(Vector3 groundCheckPosition, float groundCheckRadius, LayerMask whatIsGround, IsGroundedOverride groundedOverride)
    {
        bool _isGrounded = Physics.CheckSphere(groundCheckPosition, groundCheckRadius, whatIsGround, QueryTriggerInteraction.Ignore);
        if (_isGrounded && OnSlope())
        {
            _isGrounded = WithinSlopeLimits();
            if (m_implementVelocity.y > slopeHit.normal.y * m_speed)
            {
                _isGrounded = false;
            }
        }
        if (groundedOverride != IsGroundedOverride.DISABLED)
        {
            if (groundedOverride == IsGroundedOverride.GROUNDED)
                _isGrounded = true;
            else
                _isGrounded = false;
        }
        return _isGrounded;
    }

    public bool MovementDirectionIsOpposite(Vector3 newVelocity, Vector3 oldVelocity)
    {
        if (oldVelocity == Vector3.zero || newVelocity == Vector3.zero)
            return false;
        float _dotProd = Vector3.Dot(newVelocity, oldVelocity);
        return _dotProd <= directionSwitchThreshold;
    }

    void FixedUpdate()
    {
        m_isGrounded = CheckIfGrounded(m_groundChecker.position, groundRadius, ground, isGroundedOverride);
        if (m_isGrounded != m_formerGroundState)
        {
            SwitchGroundState();
        }
        if (m_coyoteTime > 0)
        {
            m_coyoteTime -= Time.fixedDeltaTime;
        }
        m_implementVelocity = m_body.velocity;

        if (m_inputs.magnitude > 0 || m_isGrounded)
        {
            float dotFactor = 1;
            float _modifier = 1;
            Vector3 _inputVelocities = (transform.forward * m_inputs.z + transform.right * m_inputs.x);
            Vector3 _velocityUpReference = Vector3.up;

            if (OnSlope()) _velocityUpReference = slopeHit.normal;

            dotFactor = MovementDirectionIsOpposite(_inputVelocities, m_oldInputValues) ? directionSwitchForce : 1;
            m_oldInputValues = _inputVelocities;

            _inputVelocities = Vector3.ProjectOnPlane(_inputVelocities, slopeHit.normal);
            _inputVelocities *= m_speed;
            if (_inputVelocities.magnitude < m_speed && _inputVelocities.magnitude > 0)
            {
                float _remainder = _inputVelocities.magnitude / m_speed;
                _inputVelocities /= _remainder;
            }
            if (!OnSlope())
                _inputVelocities.y += m_implementVelocity.y;
            if (!m_isGrounded) _modifier = airControlModifier;
            m_implementVelocity = Vector3.MoveTowards(m_implementVelocity, _inputVelocities, Time.fixedDeltaTime * _modifier * smoothingMaxDelta * dotFactor);
        }
        m_body.velocity = m_implementVelocity;
    }
}