using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    [SerializeField] private BasePlayerAbility[] numericPlayerAbilities;
    private int numbericAbilityIndex = 0;

    void Start()
    {
        
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {

        if (numbericAbilityIndex >= numericPlayerAbilities.Length)
            numbericAbilityIndex = 0;
        if (numbericAbilityIndex < 0)
            numbericAbilityIndex = numericPlayerAbilities.Length - 1;
    }

}
