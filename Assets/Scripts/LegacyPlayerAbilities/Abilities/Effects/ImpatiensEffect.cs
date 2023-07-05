using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ImpatiensEffect : MonoBehaviour
{
    [SerializeField] private float m_lifeTime = 2f;
    [SerializeField] private float m_force = 450f;
    [SerializeField] private float m_range = 10;

    private bool wasTriggered = false;
    private float m_timeSinceSpawn = 0;

    // Update is called once per frame
    void Update()
    {
        m_timeSinceSpawn += Time.deltaTime;
        if (m_timeSinceSpawn > m_lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
    }


    private void OnTriggerEnter(Collider other)
    {
        ForceReciever colReciever = other.GetComponent<ForceReciever>();

        if (colReciever != null)
        {
            Trigger(colReciever);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        ForceReciever colReciever = other.GetComponent<ForceReciever>();

        if (colReciever != null)
        {
            Trigger(colReciever);
        }
    }

    private void Trigger(ForceReciever trigger)
    {
        if(trigger != null&&!wasTriggered)
        {
            ApplyForce(trigger);
            print("triggered");
        }
        wasTriggered = true;
    }


    void ApplyForce(ForceReciever current)
    {
        Vector3 currentVelocity = current.Rigidbody.velocity;
        Vector3 offset = transform.position - current.Rigidbody.position;
        if (current.gameObject.layer == 3)
            currentVelocity.y = 0;
        current.Rigidbody.velocity = currentVelocity;
        current.ApplyForce(transform.up * m_force/3, ForceMode.Force);
        current.ApplyForce(transform.forward * m_force, ForceMode.Force);
    }

}