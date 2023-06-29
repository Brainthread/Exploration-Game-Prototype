using UnityEngine;

namespace CapsuleController
{
    public class PlayerMovementStateMachine : MonoBehaviour
    {
        [Header("Common variables")]
        [SerializeField] private bool m_allowControl = true;


        private Rigidbody m_rigidbody;
        private Collider m_groundedCollider;
        private Collider m_aerialCollider;
        private Vector3 m_gravitationalForce;
        private Vector3 m_rayDir = Vector3.down;
        

        private bool m_shouldMaintainHeight = true;

        [Header("Levitation Control:")]
        [SerializeField] private float m_groundedHeight = 2;
        [SerializeField] private float m_levitateHeight = 1.75f;
        [SerializeField] private float m_rayToGroundLength = 3f;
        [SerializeField] public float m_rideSpringStrength = 50f;
        [SerializeField] private float m_rideSpringDamper = 5f;


        private Vector3 m_worldMoveDirection;
        private Vector3 m_localMoveDirection;
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

        private bool m_glideInput = false;
        private int m_airJumpCounter = 0;

        private Camera m_mainCamera;

        [Header("Grounded State:")]
        [SerializeField] private AnimationCurve m_accelerationFactorFromDot;
        [SerializeField] private AnimationCurve m_maxAccelerationForceFactorFromDot;
        [SerializeField] private Vector3 m_moveForceScale = new Vector3(1f, 0f, 1f);
        [SerializeField] private float maximumIncline = 45;


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

        [Header("Glide:")]
        [SerializeField] private float m_glideMaxMovementSpeed = 10f;
        [SerializeField] private float m_glideMovementAcceleration = 5f;
        [SerializeField] private float m_glideMovementResetFactor = 1.5f;
        [SerializeField] private float m_glideMaxFallSpeed = 3f;
        [SerializeField] private float m_glideVelocityTransferSpeed = 10;
        [SerializeField] private float m_glideVelocityDecayRate = 0.1f;
        [SerializeField] private float m_glideMinSpeed = 3f;

        [Header("Wallrunning:")]
        [SerializeField] private float m_wallrunAttachmentDistance = 1.5f;
        [SerializeField] private float m_wallrunStickDistance = 1f;
        [SerializeField] private float m_wallrunSlideAcceleration = 2f;
        [SerializeField] private float m_wallrunMinimumIncline = 60;
        [SerializeField] private float m_wallrunRunSpeed = 1.5f;
        [SerializeField] private LayerMask m_wallrunnableLayers;
        [SerializeField] private float m_wallrunAttachmentForce = 4f;
        [SerializeField] private float m_wallrunCameraTilt = 10f;
        [SerializeField] private float m_wallrunMaxSlipCoefficient = 1f;
        [SerializeField] private float m_wallrunSlipCoefficientDelta = 0.3f;
        [SerializeField] private float m_wallrunMinSlipCoefficient = 0f;
        private float m_wallrunSlipCoefficient = 0f;
        [SerializeField] private float m_wallrunJumpSideForce = 10;
        [SerializeField] private float m_wallrunJumpUpForce = 10;

        [Header("Wall sliding:")]
        [SerializeField] private float m_wallslideJumpSideForce = 10;
        [SerializeField] private float m_wallslideJumpUpForce = 10;

        [Header("Walljumping:")]
        [SerializeField] private int m_maxWalljumps = 10;
        private int m_walljumpCounter = 10;
        private float m_timeSinceLastWalljump;
        [SerializeField] private float m_walljumpPowerDecreaseRatio = 0.9f;


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

        public PlayerBaseState CurrentState { get => m_currentState; set => m_currentState = value; }
        public Camera MainCamera => m_mainCamera;


        public Rigidbody PhysicsBody => m_rigidbody;
        public float LevitateHeight => m_levitateHeight;
        public float LevitateStrength => m_rideSpringStrength;
        public float LevitateDamper => m_rideSpringDamper;
        public Vector3 GravitationalForce => m_gravitationalForce;
        public bool ShouldMaintainHeight { get => m_shouldMaintainHeight; set => m_shouldMaintainHeight = value; }


        public Vector3 GoalVelocity { get => m_GoalVel; set => m_GoalVel = value; }
        public AnimationCurve AccelerationFactorFromDot { get => m_accelerationFactorFromDot; set => m_accelerationFactorFromDot = value; }
        public AnimationCurve MaxAccelerationForceFromDot => m_maxAccelerationForceFactorFromDot;
        public float SpeedFactor => m_speedFactor;
        public Vector3 MoveForceScale => m_moveForceScale;
        public float MaxAccelerationForceFactor => m_maxAccelForceFactor;
        public Locomotion WalkLocomotion => walkLocomotion;
        public Locomotion RunLocomotion => runLocomotion;
        public Locomotion AerialLocomotion => aerialLocomotion;




        public bool GlideInput => m_glideInput;
        public float GlideMaxFallSpeed => m_glideMaxFallSpeed;
        public float GlideTransferSpeed => m_glideVelocityTransferSpeed;
        public float GlideVelocityDecayRate => m_glideVelocityDecayRate;
        public float GlideMinSpeed => m_glideMinSpeed;

        public float GlideMaxMovementSpeed => m_glideMaxMovementSpeed;
        public float GlideMovementAcceleration => m_glideMovementAcceleration;
        public float GlideMovementResetFactor => m_glideMovementResetFactor;


        public Vector3 LocalMoveDirection => m_localMoveDirection;
        public Vector3 WorldMoveDirection => m_worldMoveDirection;
        public bool IsRunning => m_isRunning;
        public float FallGravityFactor => m_fallGravityFactor;




        public bool JumpReady { get => m_jumpReady; set => m_jumpReady = value; }
        public float TimeSinceJumpPressed { get => m_timeSinceJumpPressed; set => m_timeSinceJumpPressed = value; }
        public float JumpBuffer { get => m_jumpBuffer; }
        public float TimeSinceUngrounded { get => m_timeSinceUngrounded; set => m_timeSinceUngrounded = value; }
        public float CoyoteTime { get => m_coyoteTime; }
        public bool IsJumping { get => m_isJumping; set => m_isJumping = value; }
        public float JumpForceFactor { get => m_jumpForceFactor; }
        public float TimeSinceJump { get => m_timeSinceJump; set => m_timeSinceJump = value; }

        public int AirJumpNumber { get => m_airJumps; }
        public int AirJumpCounter { get => m_airJumpCounter; set => m_airJumpCounter = value; }


        public float WallrunAttachmentDistance => m_wallrunAttachmentDistance;
        public float WallrunStickDistance => m_wallrunStickDistance;
        public float WallrunSlideAccelerationCoefficient => m_wallrunSlideAcceleration;
        public float WallrunRunSpeed => m_wallrunRunSpeed;
        public LayerMask WallrunnableLayers => m_wallrunnableLayers;
        public float WallrunAttachmentForce => m_wallrunAttachmentForce;
        public float WallrunCameraTilt => m_wallrunCameraTilt;
        public float WallrunSlipCoefficient { get => m_wallrunSlipCoefficient; set => m_wallrunSlipCoefficient = value; }
        public float WallrunMaxSlipCoefficent => m_wallrunMaxSlipCoefficient;
        public float WallrunMinSlipCoefficient => m_wallrunMinSlipCoefficient;
        public float WallrunSlipCoefficientDelta => m_wallrunSlipCoefficientDelta;

        public float WallRunJumpSideForce => m_wallrunJumpSideForce;
        public float WallrunJumpUpForce => m_wallrunJumpUpForce;

        public float WallslideJumpSideForce => m_wallslideJumpSideForce;
        public float WallslideJumpUpForce => m_wallslideJumpUpForce;

        public int MaxWalljumps => m_maxWalljumps;
        public int WalljumpCounter { get => m_walljumpCounter; set => m_walljumpCounter = value; }

        public float TimeSinceWallJump { get => m_timeSinceLastWalljump; set => m_timeSinceLastWalljump = value; }

        public float WalljumpPowerDecreaseRatio => m_walljumpPowerDecreaseRatio;


        void Awake()
        {
            WalljumpCounter = MaxWalljumps;
            m_airJumpCounter = 0;
            m_rigidbody = GetComponent<Rigidbody>();
            m_gravitationalForce = Physics.gravity * m_rigidbody.mass;
            m_states = new PlayerStateFactory(this);
            m_currentState = m_states.Aerial();
            m_currentState.EnterState();
            m_timeSinceJumpPressed = Mathf.Max(m_coyoteTime, m_jumpBuffer);
            m_timeSinceJump = Mathf.Max(m_coyoteTime, m_jumpBuffer);
            m_mainCamera = Camera.main;
            m_wallrunSlipCoefficient = WallrunMinSlipCoefficient;
        }

        void Update()
        {
            if (m_timeSinceLastWalljump < 10000)
                m_timeSinceLastWalljump += Time.deltaTime;
            if (m_timeSinceJumpPressed <= m_coyoteTime + 1)
                m_timeSinceJumpPressed += Time.deltaTime;
            if (m_timeSinceJump <= m_coyoteTime + 1)
                m_timeSinceJump += Time.deltaTime;
            if(m_timeSinceUngrounded <= m_coyoteTime + 1)
                m_timeSinceUngrounded += Time.deltaTime;
            m_glideInput = Input.GetButton("Jump");
            m_localMoveDirection = Input.GetAxisRaw("Horizontal") * Vector3.right + Input.GetAxisRaw("Vertical") * Vector3.forward;
            m_worldMoveDirection = Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward;
            m_isRunning = Input.GetButton("Sprint");
            if (Input.GetButtonDown("Jump"))
            {
                m_timeSinceJumpPressed = 0;
                m_jumpInput = new Vector3(0, 1, 0);
            }
            //print(m_currentState);
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
            m_worldMoveDirection = _movement;
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
            bool grounded = false;
            if (rayHitGround == true)
            {
                grounded = rayHit.distance <= m_groundedHeight; 
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

        public void DisableLevitation()
        {
                if(m_currentState == m_states.Grounded())
                    m_currentState = m_states.Aerial();
        }

        public bool LegalIncline(bool hitGround, RaycastHit hit)
        {
            if (!hitGround) return false;
            float angle = GetSlope(hit);
            return angle < maximumIncline;
        }

        public float GetSlope(RaycastHit hit)
        {
            float dot = Vector3.Dot(Vector3.up, hit.normal);
            float angle = Mathf.Rad2Deg * Mathf.Acos(dot);
            return angle;
        }

        public void ResetAerialSpeed()
        {
           
        }
    }
}
