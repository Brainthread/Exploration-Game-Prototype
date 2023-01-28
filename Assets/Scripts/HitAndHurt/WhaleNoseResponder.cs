using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleNoseResponder : MonoBehaviour, IHurtResponder
{

    [SerializeField] private List<Hurtbox> m_hurtboxes = new List<Hurtbox>();
    private void Start()
    {
        foreach (Hurtbox box in m_hurtboxes)
        {
            box.HurtResponder = this;
        }
    }
    public bool CheckHit(HitData data)
    {
        return true;
    }

    public void Response(HitData data, DamageData damagedata)
    {
        print("OW OW OW");
    }

}
