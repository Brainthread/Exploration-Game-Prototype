using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Propellerseed", menuName = "Player/Abilities/Seedbags/Propellerseed")]
public class Seedbag_Propeller : PlayerAbility
{
    private bool isActive = false;
    public override bool AbilityIsActive { get => isActive; set => isActive = value; }

    public override void AbilityActivation()
    {
        throw new System.NotImplementedException();
    }

    public override void AbilityDeactivation()
    {
        throw new System.NotImplementedException();
    }

    public override void AbilityUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public override void AbilityDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void AbilityEnable()
    {
        throw new System.NotImplementedException();
    }
}
