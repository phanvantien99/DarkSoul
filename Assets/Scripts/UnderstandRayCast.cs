using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderstandRayCast : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] LayerMask layerGround;


    private void Update()
    {
        Debug.Log(Physics.SphereCast(transform.position, 0.1f, Vector3.down, out RaycastHit hit, 0.16f, layerGround));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, Vector3.down * 0.16f);
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
