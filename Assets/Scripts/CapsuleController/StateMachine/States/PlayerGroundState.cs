using JetBrains.Annotations;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerGroundState : PlayerBaseState
    {
        public PlayerGroundState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
        public override void EnterState() {
            //Debug.Log("Grounded");
            _context.ShouldMaintainHeight = true;
            _context.JumpReady = true;
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);
            if (ShouldJump() && grounded && onIncline)
                SwitchState(_factory.Jump());
            _context.PhysicsBody.useGravity = false;
        }

        public override void UpdateState() {
            _context.AirJumpCounter = _context.AirJumpNumber;
            if (ShouldJump())
                SwitchState(_factory.Jump());
        }

        public override void ExitState() {
            _context.ShouldMaintainHeight = false;
            _context.PhysicsBody.useGravity = true;
        }

        public override void CheckSwitchStates() { 
        
        }

        public override void InitializeSubState() { 
        
        }

        public override void FixedUpdateState(){
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            PlayerMovementStateMachine.Locomotion locomotion = _context.IsRunning ? _context.RunLocomotion : _context.WalkLocomotion;
            if (!_context.CheckIfGrounded(rayHitGround, rayHit)|| !_context.LegalIncline(rayHitGround, rayHit)) SwitchState(_factory.Aerial());
            Move(rayHit, locomotion);
            if (_context.ShouldMaintainHeight) MaintainHeight(rayHit, Vector3.down);
            _context.TimeSinceUngrounded = 0;
        }

        public void MaintainRotation()
        {
            if(_context.transform.parent != null)
            {
                Quaternion desiredRotation = Quaternion.identity;
                _context.transform.rotation = desiredRotation;
            }
        }

        private void MaintainHeight(RaycastHit rayHit, Vector3 rayDirection)
        {
            Vector3 vel = _context.PhysicsBody.velocity;
            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = rayHit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }
            float rayDirVel = Vector3.Dot(rayDirection, vel);
            float otherDirVel = Vector3.Dot(rayDirection, otherVel);

            float relVel = rayDirVel - otherDirVel;
            float currHeight = rayHit.distance - _context.LevitateHeight;
            float springForce = (currHeight * _context.LevitateStrength) - (relVel * _context.LevitateDamper);
            Vector3 maintainHeightForce = -_context.GravitationalForce + springForce * Vector3.down;
            //_context.PhysicsBody.AddForce(maintainHeightForce);

            Vector3 worldPos = _context.PhysicsBody.position;
            worldPos.y = rayHit.point.y + _context.LevitateHeight;
            worldPos = Vector3.MoveTowards(_context.PhysicsBody.position, worldPos, Time.fixedDeltaTime * 1000);

            _context.PhysicsBody.MovePosition(worldPos);
            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(-maintainHeightForce, rayHit.point);
            }
        }

        private void Move(RaycastHit rayHit, PlayerMovementStateMachine.Locomotion locomotion)
        {
            Vector3 m_UnitGoal = _context.MoveInput.normalized;
            float m_projectedVerticalSpeed = Vector3.ProjectOnPlane(m_UnitGoal, rayHit.normal).y;
            m_UnitGoal.y = m_projectedVerticalSpeed;
            m_UnitGoal = m_UnitGoal.normalized;
            Vector3 unitVel = _context.GoalVelocity.normalized;
            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            float accel = locomotion.acceleration * _context.AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * locomotion.maxSpeed * _context.SpeedFactor;
            _context.GoalVelocity = Vector3.MoveTowards(_context.GoalVelocity,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);
            _context.PhysicsBody.velocity = Vector3.MoveTowards(_context.PhysicsBody.velocity, goalVel, accel * Time.fixedDeltaTime);
        }

        private bool ShouldJump()
        {
            return _context.TimeSinceJumpPressed < _context.JumpBuffer;
        }
    }
}
