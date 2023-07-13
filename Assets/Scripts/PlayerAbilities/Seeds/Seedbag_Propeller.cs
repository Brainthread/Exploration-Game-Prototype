using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Propellerseed", menuName = "Player/Abilities/Seedbags/Propellerseed")]
public class Seedbag_Propeller : PlayerAbility
{
    private bool _isActive = false;
    public override bool AbilityIsActive { get => _isActive; set => _isActive = value; }
    public override PlayerAbilityManager abilityManager { get => _manager; set => _manager = value; }

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _maxHoldTime = 1;
    [SerializeField] private float _minThrowSpeed = 4;
    [SerializeField] private float _maxThrowSpeed = 12;
    private float _holdTime;
    private bool _holding = false;
    private PlayerAbilityManager _manager;

    public override void AbilityActivation()
    {
        _holdTime = 0;
        _holding = true;
    }

    public override void AbilityDeactivation()
    {
        if(_holding) 
        {
            FireProjectile();
        }
        _holding = false;

    }

    public override void AbilityUpdate()
    {
        if(_holding)
        {
            _holdTime = Mathf.MoveTowards(_holdTime, _maxHoldTime, Time.deltaTime);
        }           
    }

    public void FireProjectile()
    {
        float speedFactor = _holdTime / _maxHoldTime;
        float speed = Mathf.Lerp(_minThrowSpeed, _maxThrowSpeed, speedFactor);
        Debug.Log("Send projectile with speed " + speed);
    }

    public override void Initialize(PlayerAbilityManager manager)
    {
        _manager = manager;
    }

    public override void AbilityDisable()
    {
        throw new System.NotImplementedException();
    }

    public override void AbilityEnable()
    {
        _holding = false;
    }
}
