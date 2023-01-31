using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ForceReciever : MonoBehaviour
{
    [SerializeField] protected Rigidbody m_rigidbody;

    public virtual void RegisterRigidbody ()
    {
        ForceRecieverRegister.Current.AddToList(this);
    }

    public virtual void UnRegisterRigidbody ()
    {
        ForceRecieverRegister.Current.RemoveFromList(this);
    }

    public virtual void Start()
    {
        RegisterRigidbody();
    }
       
    public virtual void ApplyForce (Vector3 force, ForceMode forceMode)
    {
        m_rigidbody.AddForce(force, forceMode);
    }

    public virtual void ApplyForceAtPoint(Vector3 force, Vector3 point, ForceMode forceMode)
    {
        m_rigidbody.AddForceAtPosition(force, point, forceMode);
    }

    public void OnDestroy()
    {
        UnRegisterRigidbody();
    }
}
