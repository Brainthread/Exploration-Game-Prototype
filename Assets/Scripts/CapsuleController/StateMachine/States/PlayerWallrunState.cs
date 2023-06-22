using System.Linq.Expressions;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerWallrunState : PlayerBaseState
    {
        public PlayerWallrunState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
        private Vector3 lastInput = Vector3.zero;
        public override void EnterState()
        {
            _context.PhysicsBody.useGravity = false;
            Vector3 velocity = _context.PhysicsBody.velocity;
            velocity.y = 0;
            _context.PhysicsBody.velocity = velocity;
            Debug.Log("Enter Wallrun");
        }
        public override void UpdateState()
        {


        }
        public override void ExitState()
        {
            Debug.Log("Exit Wallrun");
            _context.GetComponent<CharacterMouseLook>().DoTilt(0, 120);
        }
        public override void CheckSwitchStates()
        {

        }
        public override void InitializeSubState()
        {

        }

        public override void FixedUpdateState()
        {

            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();

            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);

            if(_context.LocalMoveDirection.x != 0)
                lastInput = _context.LocalMoveDirection;
            
            if(grounded&&onIncline)
            {
                SwitchState(_factory.Grounded());
                return;
            }

            (bool wallWasHit, RaycastHit wallHit) = FindWallrunSurface(lastInput, _context.transform.position, _context.transform.right, _context.WallrunAttachmentDistance, _context.WallrunnableLayers);

            if (!wallWasHit)
            {
                SwitchState(_factory.Aerial());
                return;
            }

            PlayerMovementStateMachine.Locomotion locomotion = _context.IsRunning ? _context.RunLocomotion : _context.WalkLocomotion;
            Move(locomotion, wallHit.normal);
            AttachToWall(wallHit, _context.LocalMoveDirection);
            _context.GetComponent<CharacterMouseLook>().DoTilt(10*Mathf.Sign(_context.LocalMoveDirection.x), 120);
            Slide();
            if (_context.TimeSinceJumpPressed < _context.JumpBuffer)
                Jump(wallHit.normal);
        }

        private void Move(PlayerMovementStateMachine.Locomotion locomotion, Vector3 normal)
        {

            Vector3 m_UnitGoal = _context.WorldMoveDirection.normalized;
            Vector3 movementVector = Vector3.Cross(Vector3.up, normal);
            m_UnitGoal = Vector3.Project(m_UnitGoal, movementVector).normalized;
            Vector3 unitVel = _context.GoalVelocity.normalized;
            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            float accel = locomotion.acceleration * _context.AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * Mathf.Clamp(Vector3.ProjectOnPlane(_context.PhysicsBody.velocity, Vector3.up).magnitude, locomotion.maxSpeed, 10000) * _context.SpeedFactor;

            _context.GoalVelocity = Vector3.MoveTowards(_context.GoalVelocity,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);

            Vector3 tempGoal = _context.GoalVelocity + -normal*1;
            tempGoal.y = _context.PhysicsBody.velocity.y;
            _context.GoalVelocity = tempGoal;

            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, _context.GoalVelocity, accel * Time.fixedDeltaTime);
        }

        private void AttachToWall(RaycastHit hit, Vector3 moveDirection)
        {
            Vector3 forceDirection = hit.normal * -1;
            forceDirection.y = 0;
            Vector3 force = forceDirection * _context.WallrunAttachmentForce;
            _context.PhysicsBody.AddForce(force);
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

        private void Jump(Vector3 normal)
        {
            normal.y = 0; 
            _context.JumpReady = false;
            _context.ShouldMaintainHeight = false;
            _context.IsJumping = true;
            Vector3 jumpVector = new Vector3() + normal.normalized * _context.WalljumpSideForce;
            jumpVector += Vector3.up * _context.WalljumpUpForce;

            _context.PhysicsBody.AddForce(jumpVector, ForceMode.VelocityChange);

            _context.TimeSinceJumpPressed = _context.JumpBuffer;
            _context.TimeSinceJump = 0f;
            _context.TimeSinceUngrounded = _context.CoyoteTime;
            _context.GoalVelocity = jumpVector;
        }


        public static bool ShouldBeAttached(Vector3 moveInput, Vector3 position, PlayerMovementStateMachine context, float attachmentDistance, LayerMask layers)
        {
            (bool hitWall, RaycastHit raycastHit) = FindWallrunSurface(moveInput, position, context.transform.right, attachmentDistance, layers);
            
            Vector3 contextVelocity = context.PhysicsBody.velocity;
            float dot = Vector3.Dot(contextVelocity, -raycastHit.normal);

            if (!hitWall||moveInput.z<=0||dot<=0)
                return false;
            return true;
        }

        public static (bool, RaycastHit) FindWallrunSurface(Vector3 moveInput, Vector3 position, Vector3 contextRight, float attachmentDistance, LayerMask layers)
        {
            RaycastHit hit = new RaycastHit();
            bool didHit = false;
            Vector3 raycastDirection = (moveInput.x * contextRight).normalized;
            Vector3 raycastPosition = position;
            if (raycastDirection != Vector3.zero)
            {
                if (Physics.Raycast(raycastPosition, raycastDirection, out hit, attachmentDistance, layers))
                {
                    didHit = true;
                }
            }
            return (didHit, hit);
        }
    }

    /*
     
     */
}
