using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAbility : MonoBehaviour
{
    private Camera main;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float maxDistance = 30;
    [SerializeField] private float maxForce = 1000;
    private GameObject hitObject;
    private Vector3 offset;
    private float maxDist;
    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if(Physics.Raycast(main.transform.position, main.transform.forward, out hit, maxDistance, mask))
            {
                maxDist = hit.distance;
                hitObject = hit.transform.gameObject;
                offset = hit.point - hitObject.transform.position;
            }
        }
        if(Input.GetKeyUp(KeyCode.E))
        {
            hitObject = null;
        }
        if (hitObject)
        {
            float distance = Vector3.Distance(transform.position, hitObject.transform.position);
            if(distance > (maxDist+3f)) { 
                Vector3 off = (hitObject.transform.position + offset) - transform.position;
                GetComponent<Rigidbody>().AddForce(off.normalized * Time.deltaTime*maxForce, ForceMode.VelocityChange);
            }
        }
    }
}
