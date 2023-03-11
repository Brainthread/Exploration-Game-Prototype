using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapleSeed : PlayerAbility_ProjectileAbility
{
    [SerializeField] private float m_chargeCapacity;
    private float m_charge = 0;
    private bool m_isCharging = false;
    private GameObject m_spellEffect;

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
        if (VoidEffect.Current != null)
        {
            Destroy(VoidEffect.Current.gameObject);
            return;
        }
        if (m_projectileBuf)
        {
            return;
        }
        base.TriggerStop();
        Activate();
        m_charge = 0f;
    }
    public override void Activate()
    {
        base.Activate();

    }
}
