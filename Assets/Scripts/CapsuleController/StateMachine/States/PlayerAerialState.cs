using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerAerialState : PlayerBaseState
    {
        public PlayerAerialState(PlayerStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
      
        public override void EnterState() {

        }
        public override void UpdateState() {

        }
        public override void ExitState() { 
        
        }
        public override void CheckSwitchStates() { 
        
        }
        public override void InitializeSubState() { 
        
        }

        public override void FixedUpdateState(){
            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();
            if (_context.PhysicsBody.velocity.y < 0)
            {
                HandleFall();
                if (_context.CheckIfGrounded(rayHitGround, rayHit))
                {
                    Debug.Log("Go To Grounded");
                    SwitchState(_factory.Grounded());
                    return;
                }
            }
            else if (_context.PhysicsBody.velocity.y > 0)
            {
                HandleAscent();
            }
            Move(rayHit, _context.AerialLocomotion);
        }

        private void HandleFall()
        {
            _context.ShouldMaintainHeight = true;
            _context.JumpReady = true;
             // Edit at a later time
            _context.PhysicsBody.AddForce(_context.GravitationalForce * (_context.FallGravityFactor - 1f));
        }


        
        private void HandleAscent()
        {
            //Do Something?
        }

        private void Move(RaycastHit rayHit, PlayerStateMachine.Locomotion locomotion)
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
    }
}
