using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationVector;

    void Update()
    {
        transform.Rotate(rotationVector);
    }

}
