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
            float targetSpeed = Mathf.Clamp(currentForwardVelocity.magnitude * (1-Time.fixedDeltaTime*_context.GlideVelocityDecayRate), _context.GlideMinSpeed, currentForwardVelocity.magnitude);
            Vector3 targetVelocity = targetSpeed * _context.transform.forward;
            targetVelocity.y += _context.PhysicsBody.velocity.y;
            _context.GoalVelocity = targetVelocity;
            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, targetVelocity, Time.fixedDeltaTime*_context.GlideTransferSpeed);
            //_context.PhysicsBody.AddForce(requiredForce*Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        private void MaintainVerticalVelocity()
        {
            float currentVelocity = _context.PhysicsBody.velocity.y;
            float maxAcceleration = 20;
            float targetVelocity = -_context.GlideMaxFallSpeed;
            float offset = targetVelocity - currentVelocity;
            float dragFactor = 1;
            float velocityDragFactor = 0;


            if(currentVelocity != 0)
            {
                dragFactor = (((Mathf.Abs(currentVelocity) / Mathf.Abs(targetVelocity))-1)/50)+1; //Shoddy maths, replace at some point
            }
            dragFactor = dragFactor*dragFactor*dragFactor*dragFactor;

            dragFactor = Math.Clamp(dragFactor, 0, 100);

            float requiredForce = offset / Time.fixedDeltaTime;
            float force = Mathf.Clamp(requiredForce, -maxAcceleration, maxAcceleration);

            _context.PhysicsBody.AddForce(force * dragFactor * Vector3.up);
            if(offset>0.3f)
                _context.PhysicsBody.AddForce(10 * dragFactor * _context.transform.forward);

        }

        public override void InitializeSubState()
        {

        }

        public override void UpdateState()
        {

        }
    }
}
