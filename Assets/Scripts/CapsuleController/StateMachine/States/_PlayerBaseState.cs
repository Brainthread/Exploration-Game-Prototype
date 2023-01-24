namespace CapsuleController
{
    public abstract class PlayerBaseState
    {
        protected PlayerMovementStateMachine _context;
        protected PlayerStateFactory _factory;
        public PlayerBaseState(PlayerMovementStateMachine context, PlayerStateFactory factory)
        {
            _context = context;
            _factory = factory;
        }
        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void FixedUpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitializeSubState();
        protected void UpdateStates() { }
        protected void SwitchState (PlayerBaseState newState) {
            ExitState();
            newState.EnterState();
            _context.CurrentState = newState;
        }

        protected void SetSuperState () { }
        protected void SetSubState () { }
    }
}
