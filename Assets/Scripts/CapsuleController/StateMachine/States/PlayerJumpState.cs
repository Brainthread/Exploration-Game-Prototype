using UnityEngine;

namespace CapsuleController
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }

        public override void EnterState()
        { 
            (bool hitGround, RaycastHit rayHit) = _context.RaycastToGround();
            if (_context.TimeSinceJumpPressed < _context.CoyoteTime)
            {
                if (_context.JumpReady)
                {
                    Debug.Log("Jumped");
                    _context.JumpReady = false;
                    _context.ShouldMaintainHeight = false;
                    _context.IsJumping = true;
                    _context.PhysicsBody.velocity = new Vector3(_context.PhysicsBody.velocity.x, 0f, _context.PhysicsBody.velocity.z);
                    if (rayHit.distance != 0)
                    {
                        _context.PhysicsBody.position = new Vector3(_context.PhysicsBody.position.x, _context.PhysicsBody.position.y - (rayHit.distance - _context.LevitateHeight), _context.PhysicsBody.position.z);
                    }
                    _context.PhysicsBody.AddForce(Vector3.up * _context.JumpForceFactor, ForceMode.Impulse);
                    _context.TimeSinceJumpPressed = _context.JumpBuffer;
                    _context.TimeSinceJump = 0f;
                }
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
