public abstract class PlayerAbility
{
    public abstract void Initialize();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void AbilityActiveUpdate();
    public abstract void AbilityActiveFixedUpdate();
    public abstract void GetAbilityFireInput();

}
