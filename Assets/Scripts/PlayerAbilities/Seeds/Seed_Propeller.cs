using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Seed_Propeller : MonoBehaviour
{
    [SerializeField] private LayerMask _hittableLayers;
    private Vector3 _lastPosition;
    private Rigidbody _rigidbody;
    private float _initialSpeed;

    [SerializeField] private float _lifeTime = 5;
    private float _timeCounter = 0;


    [SerializeField] private AnimationCurve _speedCurve;

    
    // Start is called before the first frame update

    public void Initialize(float speed, Vector3 relativeSpeed)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lastPosition = transform.position;
        _rigidbody.AddForce(transform.forward * speed + relativeSpeed, ForceMode.VelocityChange);
        _initialSpeed = speed;
        _timeCounter = 0;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _timeCounter += Time.deltaTime;
        float timeRatio = _timeCounter / _lifeTime;
        _rigidbody.velocity = transform.forward * _initialSpeed * _speedCurve.Evaluate(timeRatio);
        _lastPosition = transform.position;
    }
}
