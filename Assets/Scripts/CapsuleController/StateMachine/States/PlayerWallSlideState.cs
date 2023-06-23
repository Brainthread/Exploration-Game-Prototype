using System.Linq.Expressions;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerWallSlideState : PlayerBaseState
    {
        public PlayerWallSlideState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
        private Vector3 lastInput = Vector3.zero;
        public override void EnterState()
        {
            _context.PhysicsBody.useGravity = false;
            Vector3 velocity = _context.PhysicsBody.velocity;
            velocity.y = 0;
            _context.PhysicsBody.velocity = velocity;
            Debug.Log("Enter Wallrun");
            _context.GetComponent<CharacterMouseLook>().DoTilt(0, 120);
        }
        public override void UpdateState()
        {
            _context.WallrunSlipCoefficient = Mathf.MoveTowards(_context.WallrunSlipCoefficient, _context.WallrunMaxSlipCoefficent, Time.deltaTime * _context.WallrunSlipCoefficientDelta);
        }
        public override void ExitState()
        {
            Debug.Log("Exit Wallrun");
            _context.GetComponent<CharacterMouseLook>().DoTilt(0, 120);
        }
        public override void CheckSwitchStates()
        {

        }
        public override void InitializeSubState()
        {

        }

        public override void FixedUpdateState()
        {

            (bool rayHitGround, RaycastHit rayHit) = _context.RaycastToGround();

            bool grounded = _context.CheckIfGrounded(rayHitGround, rayHit);
            bool onIncline = _context.LegalIncline(rayHitGround, rayHit);

            if(_context.LocalMoveDirection.x != 0)
                lastInput = _context.LocalMoveDirection;
            
            if(grounded&&onIncline)
            {
                SwitchState(_factory.Grounded());
                return;
            }

            (bool wallWasHit, RaycastHit wallHit) = FindWallslideSurface(lastInput, _context.transform.position, _context.transform.right, _context.WallrunAttachmentDistance, _context.WallrunnableLayers);

            if (!wallWasHit)
            {
                SwitchState(_factory.Aerial());
                return;
            }
            if(_context.LocalMoveDirection.z >= 0)
            {
                SwitchState(_factory.Wallrunning());
                return;
            }
            AttachToWall(wallHit, _context.LocalMoveDirection);
            Slide();
            if (_context.TimeSinceJumpPressed < _context.JumpBuffer && _context.WalljumpCounter>0)
            {
                Jump(wallHit.normal);
                _context.WalljumpCounter--;
            }
                
        }

        private void AttachToWall(RaycastHit hit, Vector3 moveDirection)
        {
            Vector3 forceDirection = hit.normal * -1;
            forceDirection.y = 0;
            Vector3 force = forceDirection * _context.WallrunAttachmentForce;
            _context.PhysicsBody.AddForce(force);
        }

        private void Slide()
        {
            float currentYVelocity = _context.PhysicsBody.velocity.y;
            _context.PhysicsBody.AddForce((_context.GravitationalForce * (_context.WallrunSlideAccelerationCoefficient*_context.WallrunSlipCoefficient)));
        }

        private void Jump(Vector3 normal)
        {
            normal.y = 0; 
            _context.JumpReady = false;
            _context.ShouldMaintainHeight = false;
            _context.IsJumping = true;
            Vector3 jumpVector = new Vector3() + normal.normalized * _context.WalljumpSideForce;
            jumpVector += Vector3.up * _context.WalljumpUpForce;

            _context.PhysicsBody.AddForce(jumpVector, ForceMode.VelocityChange);

            _context.TimeSinceJumpPressed = _context.JumpBuffer;
            _context.TimeSinceJump = 0f;
            _context.TimeSinceUngrounded = _context.CoyoteTime;
            _context.GoalVelocity = jumpVector;
        }


        public static bool ShouldBeAttached(PlayerMovementStateMachine context, float attachmentDistance, LayerMask layers)
        {
            (bool hitWall, RaycastHit raycastHit) = FindWallslideSurface(context.WorldMoveDirection, context.transform.position, context.transform.right, attachmentDistance, layers);
            if (!hitWall)
                return false;
            return true;
        }

        public static (bool, RaycastHit) FindWallslideSurface(Vector3 moveInput, Vector3 position, Vector3 contextRight, float attachmentDistance, LayerMask layers)
        {
            RaycastHit hit = new RaycastHit();
            bool didHit = false;
            Vector3 raycastDirection = (moveInput.x * contextRight).normalized;
            Vector3 raycastPosition = position;
            if (raycastDirection != Vector3.zero)
            {
                if (Physics.Raycast(raycastPosition, raycastDirection, out hit, attachmentDistance, layers))
                {
                    didHit = true;
                }
            }
            return (didHit, hit);
        }
    }

    /*
     
     */
}
