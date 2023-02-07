using JetBrains.Annotations;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerGroundState : PlayerBaseState
    {
        public PlayerGroundState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
        public override void EnterState() {
            _context.ShouldMaintainHeight = true;
            _context.JumpReady = true;
            if (ShouldJump())
                SwitchState(_factory.Jump());
        }

        public override void UpdateState() {
            _context.AirJumpCounter = _context.AirJumpNumber;
            if (ShouldJump())
                SwitchState(_factory.Jump());
        }

        public override void ExitState() {
            _context.ShouldMaintainHeight = false;
        }

        public override void CheckSwitchStates() { 
        
        }

        public override void InitializeSubState() { 
        
        }

        public override void FixedUpdateState(){
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            PlayerMovementStateMachine.Locomotion locomotion = _context.IsRunning ? _context.RunLocomotion : _context.WalkLocomotion;
            if (!_context.CheckIfGrounded(rayHitGround, rayHit)) SwitchState(_factory.Aerial());
            if (_context.ShouldMaintainHeight) MaintainHeight(rayHit, Vector3.down);
            _context.TimeSinceUngrounded = 0;
            Move(rayHit, locomotion);
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
            _context.PhysicsBody.AddForce(maintainHeightForce);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(-maintainHeightForce, rayHit.point);
            }
        }

        private void Move(RaycastHit rayHit, PlayerMovementStateMachine.Locomotion locomotion)
        {
            Vector3 m_UnitGoal = Vector3.ProjectOnPlane(_context.MoveInput, rayHit.normal).normalized;
            Vector3 unitVel = _context.GoalVelocity.normalized;
            float velDot = Vector3.Dot(m_UnitGoal, unitVel);
            float accel = locomotion.acceleration * _context.AccelerationFactorFromDot.Evaluate(velDot);
            Vector3 goalVel = m_UnitGoal * locomotion.maxSpeed * _context.SpeedFactor;
            _context.GoalVelocity = Vector3.MoveTowards(_context.GoalVelocity,
                                            goalVel,
                                            accel * Time.fixedDeltaTime);

            Vector3 neededAccel = (_context.GoalVelocity - _context.PhysicsBody.velocity) / Time.fixedDeltaTime;
            float maxAccel = locomotion.maxAccelForce * _context.MaxAccelerationForceFromDot.Evaluate(velDot) * _context.MaxAccelerationForceFactor;
            neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
            _context.PhysicsBody.AddForceAtPosition(Vector3.Scale(neededAccel * _context.PhysicsBody.mass, _context.MoveForceScale), _context.transform.position); // Using AddForceAtPosition in order to both move the player and cause the play to lean in the direction of input.
        }

        private bool ShouldJump()
        {
            return _context.TimeSinceJumpPressed < _context.JumpBuffer;
        }
    }
}
