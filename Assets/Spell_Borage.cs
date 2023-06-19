using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Borage : PlayerAbility_ProjectileAbility
{

    public void Exit()
    {
        base.Exit();
    }
    public void TriggerStart()
    {
        base.TriggerStart();
        Activate();
    }
    public void TriggerStop()
    {
        base.TriggerStop();

    }
    public void Activate()
    {
        base.Activate();

    }
}
