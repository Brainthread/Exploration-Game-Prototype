using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BasePlayerAbility : ScriptableObject
{
    public abstract void Initialize();
    public abstract void OnAbilityEnable();
    public abstract void OnAbilityDisable();
    public abstract void AbilityActivation();
    public abstract void AbilityDeactivation();
    public abstract void OnAbilityUpdate();
    public abstract bool AbilityIsActive { get; set; }
}
