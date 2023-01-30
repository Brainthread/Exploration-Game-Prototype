using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Void : PlayerAbility_ProjectileAbility
{
    [SerializeField] private float m_chargeCapacity;
    private float m_charge = 0;
    private bool m_isCharging = false;

    public override void Exit()
    {
        base.Exit();
        m_charge = 0;
        m_isCharging = false;
    }
    public override void TriggerStart()
    {
        base.TriggerStart();
    }
    public override void TriggerStop()
    {
        base.TriggerStop();
        Activate();
        m_charge = 0f;
    }
}
