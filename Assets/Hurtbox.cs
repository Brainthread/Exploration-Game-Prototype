using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hurtbox : MonoBehaviour, IHurtDetector
{
    private IHurtResponder m_hurtResponder;
    public IHurtResponder HurtResponder { get { return m_hurtResponder;} set { m_hurtResponder = value; } }
}
