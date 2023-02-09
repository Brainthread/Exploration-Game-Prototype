using System;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerGlideState : PlayerBaseState
    {
        private float m_maxHorizontalVelocity = 0;
        public PlayerGlideState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory)
        {

        }

        public override void CheckSwitchStates()
        {

        }

        public override void EnterState()
        {
            
            m_maxHorizontalVelocity = new Vector3(_context.PhysicsBody.velocity.x, 0, _context.PhysicsBody.velocity.z).magnitude;
        }

        public override void ExitState()
        {

        }

        public override void FixedUpdateState()
        {
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            if (_context.CheckIfGrounded(rayHitGround, rayHit))
            {
                SwitchState(_factory.Grounded());
                return;
            }
            if(!_context.GlideInput)
            {
                SwitchState(_factory.Aerial());
            }
            MaintainVerticalVelocity();
            Glide();
        }

        private void Glide()
        {
            throw new NotImplementedException();
        }

        private void MaintainVerticalVelocity()
        {
            float currentVelocity = _context.PhysicsBody.velocity.y;
            float maxAcceleration = 20;
            float targetVelocity = -_context.GlideMaxFallSpeed;
            float offset = targetVelocity - currentVelocity;

            float requiredForce = offset / Time.fixedDeltaTime;
            float force = Mathf.Clamp(requiredForce, 0 , maxAcceleration);

            _context.PhysicsBody.AddForce(force * Vector3.up);
        }

        public override void InitializeSubState()
        {

        }

        public override void UpdateState()
        {

        }
    }
}
