using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Borage : PlayerAbility
{
    private PlayerAbilityHandler m_handler;
    public PlayerAbilityHandler Handler { get { return m_handler; } set { m_handler = value} }

    public void Activate()
    {
        throw new System.NotImplementedException();
    }

    public void ActiveUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Initialize(PlayerAbilityHandler handler)
    {
        this.Handler = handler;
    }

    public void PassiveUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerStart()
    {
        throw new System.NotImplementedException();
    }

    public void TriggerStop()
    {
        throw new System.NotImplementedException();
    }
}
