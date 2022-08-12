using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderstandRayCast : MonoBehaviour
{
    // Start is called before the first frame update


    private void Start()
    {
        Debug.Log("123");
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 0);

    }

}
