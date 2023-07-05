using System.Linq.Expressions;
using UnityEngine;

namespace CapsuleController
{
    public class PlayerWallSlideState : PlayerBaseState
    {
        public PlayerWallSlideState(PlayerMovementStateMachine context, PlayerStateFactory factory) : base(context, factory) { }
        private Vector3 lastInput = Vector3.zero;
        bool active;
        public override void EnterState()
        {
            active = true;
            _context.PhysicsBody.useGravity = false;
            Vector3 velocity = _context.PhysicsBody.velocity;
            velocity.y = 0;
            _context.PhysicsBody.velocity = velocity;
            _context.GetComponent<CharacterMouseLook>().DoTilt(0, 120);
        }
        public override void UpdateState()
        {
            _context.WallrunSlipCoefficient = Mathf.MoveTowards(_context.WallrunSlipCoefficient, _context.WallrunMaxSlipCoefficent, Time.deltaTime * _context.WallrunSlipCoefficientDelta);
        }
        public override void ExitState()
        {
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


            if (grounded && onIncline)
            {
                SwitchState(_factory.Grounded());
                return;
            }

            (bool wallWasHit, RaycastHit wallHit) = FindWallslideSurface(_context.WorldMoveDirection, _context.transform.position, _context.transform.right, _context.WallrunAttachmentDistance, _context.WallrunnableLayers);

            if (!wallWasHit)
            {
                SwitchState(_factory.Aerial());
                return;
            }
            if (_context.LocalMoveDirection.z > 0 && PlayerWallrunState.ShouldBeAttached(_context.LocalMoveDirection, _context.transform.position, _context, _context.WallrunAttachmentDistance, _context.WallrunnableLayers) && _context.TimeSinceWallJump > 0.3f)
            {
                SwitchState(_factory.Wallrunning());
                return;
            }
            AttachToWall(wallHit, _context.LocalMoveDirection);
            Slide();
            DampenMovement();
            if (_context.TimeSinceJumpPressed < _context.JumpBuffer)
            {
                Jump(wallHit.normal);
                _context.WalljumpCounter = Mathf.Max(0, _context.WalljumpCounter - 1);
            }
        }

        private void AttachToWall(RaycastHit hit, Vector3 moveDirection)
        {
            Vector3 forceDirection = hit.normal * -1;
            forceDirection.y = 0;
            Vector3 force = forceDirection * _context.WallrunAttachmentForce;
            _context.PhysicsBody.AddForce(force);
        }

        private void DampenMovement()
        {
            Vector3 speed = _context.PhysicsBody.velocity;
            float tempY = speed.y;
            speed.y = 0;
            speed = Vector3.MoveTowards(speed, Vector3.zero, 25 * Time.fixedDeltaTime);
            speed.y = tempY;
            _context.PhysicsBody.velocity = speed;
        }

        private void Slide()
        {
            _context.PhysicsBody.AddForce((_context.GravitationalForce * (_context.WallrunSlideAccelerationCoefficient * _context.WallrunSlipCoefficient)));
        }

        private void Jump(Vector3 normal)
        {
            Vector3 velocity = _context.PhysicsBody.velocity;
            velocity.y = 0;
            Vector3 fwdSpeed = Vector3.Project(_context.PhysicsBody.velocity, Vector3.Cross(Vector3.up, normal));
            _context.PhysicsBody.velocity = fwdSpeed;

            normal.y = 0;
            _context.JumpReady = false;
            _context.ShouldMaintainHeight = false;
            _context.IsJumping = true;
            Vector3 jumpVector = new Vector3() + normal.normalized * _context.WallslideJumpSideForce;
            float walljumpHeightFactor = Mathf.Min(1, (float)_context.WalljumpCounter/_context.MaxWalljumps);
            jumpVector += Vector3.up * _context.WallrunJumpUpForce * walljumpHeightFactor;
            Debug.Log(new Vector2(jumpVector.x, jumpVector.z) + ": s");
            _context.PhysicsBody.AddForce(jumpVector, ForceMode.VelocityChange);

            _context.GoalVelocity = jumpVector + fwdSpeed;

            _context.TimeSinceJumpPressed = _context.JumpBuffer;
            _context.TimeSinceJump = 0f;
            _context.TimeSinceUngrounded = _context.CoyoteTime;


            SwitchState(_factory.Aerial());
            _context.TimeSinceWallJump = 0;
        }


        public static bool ShouldBeAttached(PlayerMovementStateMachine context, float attachmentDistance, LayerMask layers)
        {
            (bool hitWall, RaycastHit raycastHit) = FindWallslideSurface(context.WorldMoveDirection, context.transform.position, context.transform.right, attachmentDistance, layers);
            if (!hitWall)
                return false;
            return true;
        }

        public static (bool, RaycastHit) FindWallslideSurface(Vector3 worldMoveDirection, Vector3 position, Vector3 contextRight, float attachmentDistance, LayerMask layers)
        {
            RaycastHit hit = new RaycastHit();
            bool didHit = false;
            Vector3 raycastDirection = worldMoveDirection.normalized;
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
