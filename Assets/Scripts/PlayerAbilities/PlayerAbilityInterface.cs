using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerAbilityInterface
{
    public void Initialize();
    public void OnAbilityEnable();
    public void OnAbilityDisable();
    public void AbilityActivation();
    public void AbilityDeactivation();
    public void OnAbilityUpdate();
    public bool AbilityIsActive { get; set; }
}
