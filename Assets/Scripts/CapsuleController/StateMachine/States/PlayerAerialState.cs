using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerAerialState : PlayerBaseState
    {
        private Vector3 m_entryVelocity;
        private float m_entryHeight;
        public PlayerAerialState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
      
        public override void EnterState() {
            Debug.Log("Player entered Aerial state");
            if (_context.AirJumpCounter > 0)
            {
                _context.JumpReady = true;
            }
            m_entryVelocity = _context.PhysicsBody.velocity;
            m_entryHeight = _context.transform.position.y;

        }
        public override void UpdateState() {
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);
            if (_context.TimeSinceJumpPressed < _context.JumpBuffer && !(grounded))
            {
                if (_context.TimeSinceUngrounded < _context.CoyoteTime)
                {
                    Debug.Log("CoyoteJump! " + _context.CoyoteTime + " " + _context.TimeSinceJumpPressed + (_context.TimeSinceJumpPressed < _context.CoyoteTime));
                    SwitchState(_factory.Jump());
                }
                else if (_context.AirJumpCounter > 0)
                {
                    _context.AirJumpCounter--;
                    SwitchState(_factory.Jump());
                }
                else
                {
                    SwitchState(_factory.Glide());
                }
            }
            if(_context.PhysicsBody.velocity.y<0&&_context.GlideInput)
            {
                SwitchState(_factory.Glide());
            }
 
        }
        public override void ExitState() { 
        
        }
        public override void CheckSwitchStates() { 
        
        }
        public override void InitializeSubState() { 
        
        }

        public override void FixedUpdateState(){
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);
            if (_context.PhysicsBody.velocity.y <= 0)
            {
                _context.PhysicsBody.useGravity = false;
                HandleFall();
            }
            else if (_context.PhysicsBody.velocity.y > 0)
            {
                _context.PhysicsBody.useGravity = true;
                HandleAscent();
            }

            if (_context.TimeSinceJump>0.5f||_context.PhysicsBody.position.y > m_entryHeight + _context.LevitateHeight||_context.PhysicsBody.velocity.y<0)
            {
                if (grounded&&onIncline)
                {
                    SwitchState(_factory.Grounded());
                    return;
                }
                if(grounded&&!onIncline)
                {
                    if(_context.PhysicsBody.velocity.y > 0)
                    {
                        HandleFall();
                    }
                }
            }

            if(PlayerWallrunState.ShouldBeAttached(_context.LocalMoveDirection, _context.transform.position, _context.transform.right, _context.WallrunAttachmentDistance, _context.WallrunnableLayers))
            {
                SwitchState(_factory.Wallrunning());
            }

            Move(rayHit, _context.AerialLocomotion);
        }

        private void HandleFall()
        {
            _context.ShouldMaintainHeight = true;
            _context.JumpReady = true;
             // Edit at a later time
            _context.PhysicsBody.AddForce(_context.GravitationalForce * (_context.FallGravityFactor - 1f));
        }


        
        private void HandleAscent()
        {
            //Do Something?
        }

        private void Move(RaycastHit rayHit, PlayerMovementStateMachine.Locomotion locomotion)
        {
            
            Vector3 m_UnitGoal = _context.WorldMoveDirection.normalized;
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

    }
}
