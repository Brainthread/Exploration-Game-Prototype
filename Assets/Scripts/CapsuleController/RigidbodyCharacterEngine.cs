using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CapsuleController {
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
        private bool m_isSprinting;
        private float m_speedFactor = 1f;
        private float m_maxAccelForceFactor = 1f;
        private Vector3 m_GoalVel = Vector3.zero;

        private Vector3 m_jumpInput;
        private float m_timeSinceJumpPressed = 0f;
        private float m_timeSinceUngrounded = 0f;
        private float m_timeSinceJump = 0f;
        private bool m_jumpReady = true;
        private bool m_isJumping = false;

        [Header("Movement:")]
        [SerializeField] private AnimationCurve m_accelerationFactorFromDot;
        [SerializeField] private AnimationCurve m_maxAccelerationForceFactorFromDot;
        [SerializeField] private Vector3 m_moveForceScale = new Vector3(1f, 0f, 1f);
        [SerializeField] private Locomotion groundLocomotion;
        [SerializeField] private Locomotion airLocomotion;
        [SerializeField] private Locomotion sprintLocomotion;


        [Header("Jump:")]
        [SerializeField] private float m_jumpForceFactor = 10f;
        //[SerializeField] private float m_riseGravityFactor = 5f;
        [SerializeField] private float m_fallGravityFactor = 10f;
        //[SerializeField] private float m_lowJumpFactor = 2.5f;
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
            m_isJumping = false;
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

        public void GetSprintInput(bool _isSprinting)
        {
            m_isSprinting = _isSprinting;
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
                if (m_isSprinting == true) locomotion = sprintLocomotion;
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

            CharacterGroundMove(m_moveInput, rayHit, locomotion);

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
        private void CharacterGroundMove(Vector3 moveInput, RaycastHit rayHit, Locomotion locomotion)
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
            if (m_timeSinceJumpPressed < m_jumpBuffer && !m_isJumping)
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

}

