using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateObject : MonoBehaviour
{
    public GameObject prefab;
    public bool asChild;
    public bool atSelf;
    public bool inheritRotation;

    public void Instantiate()
    {
        Transform parent = asChild ? transform : null;
        Vector3 position = atSelf ? transform.position : prefab.transform.position;
        Quaternion rotation = inheritRotation ? transform.rotation : prefab.transform.rotation;
        Instantiate(prefab, position, rotation, parent);
    }
}
