using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PlayerAbility_ProjectileAbility : MonoBehaviour, PlayerAbility
{
    public PlayerAbilityHandler Handler => throw new System.NotImplementedException();

    public virtual void Enter() {
    
    }

    public virtual void Exit() { 
        
    }



    public virtual void ActiveUpdate() {
        //Give me some activation button and call Activate()
    }

    public virtual void PassiveUpdate() { 
    
    }

    public virtual void TriggerStart()
    {
        
    }

    public virtual void TriggerStop()
    {
        
    }

    public void Initialize(PlayerAbilityHandler handler)
    {
        throw new System.NotImplementedException();
    }

    public void Activate()
    {
        throw new System.NotImplementedException();
    }
}
