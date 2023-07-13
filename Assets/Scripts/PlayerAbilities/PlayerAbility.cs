using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PlayerAbility : ScriptableObject
{
    public abstract void Initialize();
    public abstract void AbilityEnable();
    public abstract void AbilityDisable();
    public abstract void AbilityActivation();
    public abstract void AbilityDeactivation();
    public abstract void AbilityUpdate();
    public abstract bool AbilityIsActive { get; set; }
}
