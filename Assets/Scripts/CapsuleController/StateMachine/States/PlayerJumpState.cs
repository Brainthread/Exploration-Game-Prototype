using UnityEngine;

namespace CapsuleController
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        {
            (bool hitGround, RaycastHit rayHit) = _context.RaycastToGround();
            if (_context.JumpReady)
            {
                _context.JumpReady = false;
                _context.ShouldMaintainHeight = false;
                _context.IsJumping = true;

                if(_context.PhysicsBody.velocity.y<0) _context.PhysicsBody.velocity = new Vector3(_context.PhysicsBody.velocity.x, 0f, _context.PhysicsBody.velocity.z);
                _context.PhysicsBody.AddForce(Vector3.up * _context.JumpForceFactor, ForceMode.Impulse);

                _context.TimeSinceJumpPressed = _context.JumpBuffer;
                _context.TimeSinceJump = 0f;
                _context.TimeSinceUngrounded = _context.CoyoteTime;
            }

        }
        public override void UpdateState()
        {
            SwitchState(_factory.Aerial());
        }
        public override void ExitState()
        {

        }
        public override void CheckSwitchStates()
        {

        }
        public override void InitializeSubState()
        {

        }
        public override void FixedUpdateState()
        {

        }
    }
}
