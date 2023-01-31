using CapsuleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceReciever_Player : ForceReciever
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        if (force.y > 0) {
            GetComponent<PlayerMovementStateMachine>().DisableLevitation();
        }
        base.ApplyForce(force, forceMode);
    }

    public override void ApplyForceAtPoint(Vector3 force, Vector3 point, ForceMode forceMode)
    {
        if (force.y > 0)
        {
            GetComponent<PlayerMovementStateMachine>().DisableLevitation();
        }
        base.ApplyForceAtPoint(force, point, forceMode);
    }
}
