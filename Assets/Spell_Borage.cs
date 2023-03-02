using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Borage : PlayerAbility_ProjectileAbility
{

    public override void Exit()
    {
        base.Exit();
    }
    public override void TriggerStart()
    {
        base.TriggerStart();
        Activate();
    }
    public override void TriggerStop()
    {
        base.TriggerStop();

    }
    public override void Activate()
    {
        base.Activate();

    }
}
