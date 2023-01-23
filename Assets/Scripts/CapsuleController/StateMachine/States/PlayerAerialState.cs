using UnityEngine;

namespace CapsuleController
{
    public class PlayerAerialState : PlayerBaseState
    {
        public PlayerAerialState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
      
        public override void EnterState() { 
        
        }
        public override void UpdateState() { 
        
        }
        public override void ExitState() { 
        
        }
        public override void CheckSwitchStates() { 
        
        }
        public override void InitializeSubState() { 
        
        }

        public override void FixedUpdateState(){
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            PlayerStateMachine.Locomotion locomotion = _context.IsRunning ? _context.RunLocomotion : _context.WalkLocomotion;
            if (_context.CheckIfGrounded(rayHitGround, rayHit)) SwitchState(_factory.Grounded());
            if (_context.PhysicsBody.velocity.y < 0)
            {
                HandleFall();
            }
            else if (_context.PhysicsBody.velocity.y > 0)
            {
                HandleAscent();
            }
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
    }
}
