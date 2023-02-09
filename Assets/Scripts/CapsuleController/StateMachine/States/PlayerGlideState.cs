﻿using System;
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
            Vector3 currentHorizontalVelocity = new Vector3(_context.PhysicsBody.velocity.x, 0, _context.PhysicsBody.velocity.z);
            float targetSpeed = currentHorizontalVelocity.magnitude * (1-Time.fixedDeltaTime);
            Vector3 targetVelocity = targetSpeed * _context.transform.forward;
            Vector3 relativeVelocity = targetVelocity - currentHorizontalVelocity;
            Vector3 requiredForce = relativeVelocity / Time.fixedDeltaTime;

            _context.PhysicsBody.AddForce(requiredForce*Time.fixedDeltaTime * 10f);
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
