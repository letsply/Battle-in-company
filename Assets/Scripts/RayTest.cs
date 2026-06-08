using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public int ViewAngle = 45; 
    // Update is called once per frame
    void Update()
    {
        for (int i = -ViewAngle; i < ViewAngle; i++)
        {
            Vector3 vector = transform.TransformDirection(transform.forward);
            Vector3 dir = Quaternion.AngleAxis(i,transform.up) * vector;

            Debug.DrawRay(transform.position, dir, Color.yellow);
        }
    }
}
