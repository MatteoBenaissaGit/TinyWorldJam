using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntTurning : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.up, 0.1f);
    }
}
