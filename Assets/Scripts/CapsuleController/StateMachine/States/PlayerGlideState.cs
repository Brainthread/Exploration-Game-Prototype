using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerGlideState : PlayerBaseState
    {
        private float m_glideTimeSeconds = 0;
        private const float m_maxForwardGlideForce = 50f;
        public PlayerGlideState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory)
        {

        }

        public override void CheckSwitchStates()
        {

        }

        public override void EnterState()
        {
            Debug.Log("Player entered Glide state");
        }

        public override void ExitState()
        {
            m_glideTimeSeconds = 0;
        }

        public override void FixedUpdateState()
        {
            m_glideTimeSeconds += Time.fixedDeltaTime;
            m_glideTimeSeconds = Mathf.Clamp(m_glideTimeSeconds, 0, 5);
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
            Vector3 m_UnitGoal = _context.WorldMoveDirection.normalized;
            Vector3 unitVel = _context.GoalVelocity.normalized;
            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            float accel = _context.GlideMovementAcceleration * _context.AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * Mathf.Clamp(Vector3.ProjectOnPlane(_context.PhysicsBody.velocity, Vector3.up).magnitude, _context.GlideMaxMovementSpeed, 10000) * _context.SpeedFactor;
            if(m_UnitGoal == Vector3.zero)
            {
                goalVel *= _context.GlideMovementResetFactor;
            }

            _context.GoalVelocity = Vector3.MoveTowards(_context.GoalVelocity,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);

            Vector3 tempGoal = _context.GoalVelocity;
            tempGoal.y = _context.PhysicsBody.velocity.y;
            _context.GoalVelocity = tempGoal;

            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, _context.GoalVelocity, accel * Time.fixedDeltaTime);
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
                dragFactor = ((Mathf.Abs(offset))/100)+1; //Shoddy maths, replace at some point
            }
            dragFactor = dragFactor*dragFactor*dragFactor*dragFactor; //Shoddy maths, replace at some point

            dragFactor = Math.Clamp(dragFactor, 0, 100); //Shoddy maths, replace at some point

            float requiredForce = offset / Time.fixedDeltaTime;
            float force = Mathf.Clamp(requiredForce, -maxAcceleration, maxAcceleration); 

            _context.PhysicsBody.AddForce(force * dragFactor * Vector3.up);
            
        }


        public override void InitializeSubState()
        {

        }

        public override void UpdateState()
        {

        }
    }
}
