using System.Linq.Expressions;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
        private bool triggerFlag = false;
        public override void EnterState()
        {
            (bool hitGround, RaycastHit rayHit) = _context.RaycastToGround();
            if (_context.JumpReady)
            {
                if (_context.PhysicsBody.velocity.y < 0f)
                {
                    Vector3 newVel = _context.PhysicsBody.velocity;
                    newVel.y = 0;
                    _context.PhysicsBody.velocity = newVel;
                }
                triggerFlag = true;
            }

        }
        public override void UpdateState()
        {

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
            if (triggerFlag)
            {
                triggerFlag = false;
                if (_context.JumpReady)
                {
                    _context.JumpReady = false;
                    _context.ShouldMaintainHeight = false;
                    _context.IsJumping = true;
                    Vector3 jumpVector = new Vector3();
                    jumpVector += Vector3.up * _context.JumpForceFactor;

                    _context.PhysicsBody.AddForce(jumpVector, ForceMode.VelocityChange);

                    _context.TimeSinceJumpPressed = _context.JumpBuffer;
                    _context.TimeSinceJump = 0f;
                    _context.TimeSinceUngrounded = _context.CoyoteTime;

                }

                SwitchState(_factory.Aerial());
            }
        }
    }
}
