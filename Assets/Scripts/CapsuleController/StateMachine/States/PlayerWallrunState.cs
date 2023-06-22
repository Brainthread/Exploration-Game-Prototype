using System.Linq.Expressions;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerWallrunState : PlayerBaseState
    {
        public PlayerWallrunState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        {
            _context.PhysicsBody.useGravity = false;
            Debug.Log("Enter Wallrun");
        }
        public override void UpdateState()
        {


        }
        public override void ExitState()
        {
            Debug.Log("Exit Wallrun");
        }
        public override void CheckSwitchStates()
        {

        }
        public override void InitializeSubState()
        {

        }

        public override void FixedUpdateState()
        {
            Slide();
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();

            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);
            Vector3 m_UnitGoal = _context.MoveInput.normalized;
            
            if(grounded&&onIncline)
            {
                SwitchState(_factory.Grounded());
                return;
            }

            if (!ShouldBeAttached(_context.MoveInput.normalized, _context.transform.position, _context.transform.right, _context.WallrunAttachmentDistance, _context.WallrunnableLayers))
            {
                SwitchState(_factory.Aerial());
                return;
            }
            PlayerMovementStateMachine.Locomotion locomotion = _context.IsRunning ? _context.RunLocomotion : _context.WalkLocomotion;
            //Move(rayHit, locomotion);
            Dampen();
        }

        private void Move(RaycastHit rayHit, PlayerMovementStateMachine.Locomotion locomotion)
        {

            Vector3 m_UnitGoal = _context.MoveInput.normalized;
            m_UnitGoal = Vector3.Project(m_UnitGoal, _context.transform.forward);
            Vector3 unitVel = _context.GoalVelocity.normalized;
            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            float accel = locomotion.acceleration * _context.AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * Mathf.Clamp(Vector3.ProjectOnPlane(_context.PhysicsBody.velocity, Vector3.up).magnitude, locomotion.maxSpeed, 10000) * _context.SpeedFactor;

            _context.GoalVelocity = Vector3.MoveTowards(_context.GoalVelocity,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);

            Vector3 tempGoal = _context.GoalVelocity;
            tempGoal.y = _context.PhysicsBody.velocity.y;
            _context.GoalVelocity = tempGoal;

            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, _context.GoalVelocity, accel * Time.fixedDeltaTime);
        }

        private void Dampen()
        {

            _context.GoalVelocity = Vector3.MoveTowards(_context.GoalVelocity,
                                            Vector3.zero,
                                            10 * Time.fixedDeltaTime);

            Vector3 tempGoal = _context.GoalVelocity;
            tempGoal.y = _context.PhysicsBody.velocity.y;
            _context.GoalVelocity = tempGoal;

            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, _context.GoalVelocity, 20 * Time.fixedDeltaTime);
        }

        private void Slide()
        {
            float currentYVelocity = _context.PhysicsBody.velocity.y;
            float factor = 1;
            if(currentYVelocity>0)
            {
                factor = 2;
            }
            _context.PhysicsBody.AddForce((_context.GravitationalForce * (_context.WallrunSlideAccelerationCoefficient*factor)));
        }


        public static bool ShouldBeAttached(Vector3 moveInput, Vector3 position, Vector3 contextRight, float attachmentDistance, LayerMask layers)
        {
            Vector3 raycastDirection = Vector3.Project(moveInput, contextRight).normalized;
            Vector3 raycastPosition = position;
            if(raycastDirection != Vector3.zero)
            {
                RaycastHit hit;
                if(Physics.Raycast(raycastPosition, raycastDirection, out hit, attachmentDistance, layers))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /*
     
     */
}
