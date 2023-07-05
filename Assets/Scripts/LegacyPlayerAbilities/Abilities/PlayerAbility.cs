using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerAbility
{
    public PlayerAbilityHandler Handler { get; }
    public abstract void Initialize(PlayerAbilityHandler handler);
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Activate();
    public abstract void ActiveUpdate();
    public abstract void PassiveUpdate();

    public virtual void ActiveFixedUpdate() { }
    public virtual void PassiveFixedUpdate() { }

    public abstract void TriggerStart();
    public abstract void TriggerStop();
}
