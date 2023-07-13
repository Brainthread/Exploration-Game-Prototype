using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    [SerializeField] private PlayerAbility[] _numericPlayerAbilities;
    private int _numericAbilityIndex = 0;
    private int _lastNumericAbilityIndex = -1;
    private PlayerAbility _activeAbility = null;
    private string _numericAbilityInput = "Fire2";
    private GameObject _leftHand;

    public GameObject LeftHand { get => _leftHand; }

    void Start()
    {
        for(int i = 0; i < _numericPlayerAbilities.Length; i++)
        {
            _numericPlayerAbilities[i] = Instantiate(_numericPlayerAbilities[i]);
            _numericPlayerAbilities[i].Initialize(this);
        }
    }

    void Update()
    {
        HandleInput();
        for(int i = 0; i < _numericPlayerAbilities.Length; i++)
        {
            _numericPlayerAbilities[i].AbilityUpdate();
        }
    }

    void HandleInput()
    {
        HandlePCInput();
        if (_numericAbilityIndex >= _numericPlayerAbilities.Length)
            _numericAbilityIndex = 0;
        if (_numericAbilityIndex < 0)
            _numericAbilityIndex = _numericPlayerAbilities.Length - 1;

        if(_lastNumericAbilityIndex != _numericAbilityIndex)
            ChangeAbility();

        if (_activeAbility == null)
            return;

        if(Input.GetButtonDown(_numericAbilityInput))
        {
            _activeAbility.AbilityActivation();
        }
        if(Input.GetButtonUp(_numericAbilityInput))
        {
            _activeAbility.AbilityDeactivation();
        }
    }

    public void ChangeAbility()
    {
        if(_activeAbility != null)
            _activeAbility.AbilityDisable();
        _activeAbility = _numericPlayerAbilities[_numericAbilityIndex];
        _activeAbility.AbilityEnable();
        _lastNumericAbilityIndex = _numericAbilityIndex;
    }
    
    void HandlePCInput()
    {
        if (Input.GetAxisRaw("Scroll") != 0)
        {
            _numericAbilityIndex += (int)Input.GetAxisRaw("Scroll");
            Debug.Log("Move mouserollindex");
        }
        if (Input.GetAxisRaw("SeedScroll") != 0)
        {
            _numericAbilityIndex += (int)Input.GetAxisRaw("SeedScroll");
            Debug.Log("Move mouserollindex");
        }
    }
}
