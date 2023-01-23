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
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            Debug.Log(rayHitGround);
            if (_context.CheckIfGrounded(rayHitGround, rayHit))
            {
                Debug.Log("should be grounded");
                SwitchState(_factory.Grounded());
                return;
            }
        }
        
        private void HandleAscent()
        {
            //Do Something?
        }
    }
}
