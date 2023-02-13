using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerGlideState : PlayerBaseState
    {
        public PlayerGlideState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory)
        {

        }

        public override void CheckSwitchStates()
        {

        }

        public override void EnterState()
        {
            
        }

        public override void ExitState()
        {

        }

        public override void FixedUpdateState()
        {
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);
            if (_context.CheckIfGrounded(rayHitGround, rayHit))
            {
                if(grounded&&onIncline) SwitchState(_factory.Grounded());
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

            Vector3 currentHorizontalVelocity = new Vector3(_context.PhysicsBody.velocity.x, 0, _context.PhysicsBody.velocity.z);
            Vector3 currentForwardVelocity = Vector3.Project(currentHorizontalVelocity, currentHorizontalVelocity);
            float targetSpeed = Mathf.Clamp(currentForwardVelocity.magnitude * (1-Time.fixedDeltaTime/10), 0f, currentForwardVelocity.magnitude);
            Vector3 targetVelocity = targetSpeed * _context.transform.forward;
            targetVelocity.y += _context.PhysicsBody.velocity.y;
            _context.GoalVelocity = Vector3.zero;
            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, targetVelocity, Time.fixedDeltaTime*_context.GlideTransferSpeed);
            //_context.PhysicsBody.AddForce(requiredForce*Time.fixedDeltaTime, ForceMode.VelocityChange);
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
