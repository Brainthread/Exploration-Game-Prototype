using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditorInternal;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerMovementStateMachine : MonoBehaviour
    {
        [Header("Common variables")]
        [SerializeField] private bool m_allowControl = true;
        private Rigidbody m_rigidbody;
        private Vector3 m_gravitationalForce;
        private Vector3 m_rayDir = Vector3.down;

        private bool m_shouldMaintainHeight = true;

        [Header("Levitation Control:")]
        [SerializeField] private float m_levitateHeight = 1.75f;
        [SerializeField] private float m_rayToGroundLength = 3f;
        [SerializeField] public float m_rideSpringStrength = 50f;
        [SerializeField] private float m_rideSpringDamper = 5f;


        private Vector3 m_moveInput;
        private bool m_isRunning;
        private float m_speedFactor = 1f;
        private float m_maxAccelForceFactor = 1f;
        private Vector3 m_GoalVel = Vector3.zero;

        private Vector3 m_jumpInput;
        private float m_timeSinceJumpPressed = 0f;
        private float m_timeSinceUngrounded = 0f;
        private float m_timeSinceJump = 0f;
        private bool m_jumpReady = true;
        private bool m_isJumping = false;

        private int m_airJumpCounter = 0;

        [Header("Grounded State:")]
        [SerializeField] private AnimationCurve m_accelerationFactorFromDot;
        [SerializeField] private AnimationCurve m_maxAccelerationForceFactorFromDot;
        [SerializeField] private Vector3 m_moveForceScale = new Vector3(1f, 0f, 1f);

        [SerializeField] private Locomotion walkLocomotion;
        [SerializeField] private Locomotion runLocomotion;
        [SerializeField] private Locomotion aerialLocomotion;


        [Header("Jump:")]
        [SerializeField] private float m_jumpForceFactor = 10f;
        [SerializeField] private float m_riseGravityFactor = 5f;
        [SerializeField] private float m_fallGravityFactor = 10f;
        [SerializeField] private float m_lowJumpFactor = 2.5f;
        [SerializeField] private float m_jumpBuffer = 0.15f;
        [SerializeField] private float m_coyoteTime = 0.25f;
        [SerializeField] private LayerMask m_whatIsGround;
        [SerializeField] private int m_airJumps = 2;

        [System.Serializable]
        public class Locomotion
        {
            public float maxSpeed = 8f;
            public float acceleration = 200f;
            public float maxAccelForce = 150f;
            public Vector3 moveForceScale = new Vector3(1f, 0f, 1f);
        }

        private PlayerBaseState m_currentState;
        private PlayerStateFactory m_states;

        public PlayerBaseState CurrentState { get { return m_currentState; } set { m_currentState = value; } }


        public Rigidbody PhysicsBody { get { return m_rigidbody; } }
        public float LevitateHeight { get { return m_levitateHeight; } }
        public float LevitateStrength { get { return m_rideSpringStrength; } }
        public float LevitateDamper { get { return m_rideSpringDamper; } }
        public Vector3 GravitationalForce { get { return m_gravitationalForce; } }
        public bool ShouldMaintainHeight { get { return m_shouldMaintainHeight; } set { m_shouldMaintainHeight = value; } }


        public Vector3 GoalVelocity { get { return m_GoalVel; } set { m_GoalVel = value; } }
        public AnimationCurve AccelerationFactorFromDot { get { return m_accelerationFactorFromDot; } set { m_accelerationFactorFromDot = value; } }
        public AnimationCurve MaxAccelerationForceFromDot { get { return m_maxAccelerationForceFactorFromDot; } }
        public float SpeedFactor { get { return m_speedFactor; } }
        public Vector3 MoveForceScale { get { return m_moveForceScale; } }
        public float MaxAccelerationForceFactor { get { return m_maxAccelForceFactor; } }
        public Locomotion WalkLocomotion { get { return walkLocomotion; } }
        public Locomotion RunLocomotion { get { return runLocomotion; } }
        public Locomotion AerialLocomotion { get { return aerialLocomotion; } }


        public Vector3 MoveInput { get { return m_moveInput; } }
        public bool IsRunning { get { return m_isRunning; } }

        public float FallGravityFactor { get { return m_fallGravityFactor; } }

        public bool JumpReady { get { return m_jumpReady; } set { m_jumpReady = value; } }
        public float TimeSinceJumpPressed { get { return m_timeSinceJumpPressed; } set { m_timeSinceJumpPressed = value; } }
        public float JumpBuffer { get { return m_jumpBuffer; } }
        public float TimeSinceUngrounded { get { return m_timeSinceUngrounded; } set { m_timeSinceUngrounded = value; } }
        public float CoyoteTime { get { return m_coyoteTime; } }
        public bool IsJumping { get { return m_isJumping; } set { m_isJumping = value; } }
        public float JumpForceFactor { get { return m_jumpForceFactor; } }
        public float TimeSinceJump { get { return m_timeSinceJump; } set { m_timeSinceJump = value; } }

        public int AirJumpNumber { get { return m_airJumps; } }
        public int AirJumpCounter { get { return m_airJumpCounter; } set { m_airJumpCounter = value; } }

        void Awake()
        {
            m_airJumpCounter = 0;
            m_rigidbody = GetComponent<Rigidbody>();
            m_gravitationalForce = Physics.gravity * m_rigidbody.mass;
            m_states = new PlayerStateFactory(this);
            m_currentState = m_states.Grounded();
            m_currentState.EnterState();
            m_timeSinceJumpPressed = Mathf.Max(m_coyoteTime, m_jumpBuffer);
            m_timeSinceJump = Mathf.Max(m_coyoteTime, m_jumpBuffer);
        }

        void Update()
        {
            if (m_timeSinceJumpPressed < m_coyoteTime)
                m_timeSinceJumpPressed += Time.deltaTime;
            if (m_timeSinceJump < m_coyoteTime)
                m_timeSinceJump += Time.deltaTime;

            m_moveInput = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
            m_isRunning = Input.GetButton("Sprint");
            if (Input.GetButtonDown("Jump"))
            {
                m_timeSinceJumpPressed = 0;
                m_jumpInput = new Vector3(0, 1, 0);
            }

            m_currentState.UpdateState();
        }

        private void FixedUpdate()
        {
            m_currentState.FixedUpdateState();
        }


        /*
         * Input Handling functions
         * 
         * 
         */
        public void GetMovementInput(Vector3 _movement)
        {
            m_moveInput = _movement;
        }

        public void GetJumpInput(float _timeSinceJumpPressed)
        {
            m_timeSinceJumpPressed = _timeSinceJumpPressed;
            m_jumpInput = new Vector3(0, 1, 0);
        }

        public void GetSprintInput(bool _isSprinting)
        {
            m_isRunning = _isSprinting;
        }


        /* 
         * Common functions
         * 
         * 
         */
        public bool CheckIfGrounded(bool rayHitGround, RaycastHit rayHit)
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

        public (bool, RaycastHit) RaycastToGround()
        {
            RaycastHit rayHit;
            Ray rayToGround = new Ray(transform.position, m_rayDir);
            bool rayHitGround = Physics.Raycast(rayToGround, out rayHit, m_rayToGroundLength, m_whatIsGround);
            return (rayHitGround, rayHit);
        }

        public void AddForce(Vector3 force)
        {
            if(force.y>0)
                m_currentState = m_states.Aerial();
            m_rigidbody.AddForce(force);
              
        }
    }
}
