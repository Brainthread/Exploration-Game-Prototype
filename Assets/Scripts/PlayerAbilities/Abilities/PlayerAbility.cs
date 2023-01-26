using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerAbility
{
    public abstract void Initialize();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Activate();
    public abstract void ActiveUpdate();
    public abstract void PassiveUpdate();

    public virtual void ActiveFixedUpdate() { }
    public virtual void PassiveFixedUpdate() { }
}
