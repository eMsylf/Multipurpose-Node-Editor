using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public UnityEvent onCollisionEnter = new UnityEvent();
    public UnityEvent<GameObject> onCollisionEnterSelf = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> onCollisionEnterOther = new UnityEvent<GameObject>();
    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter.Invoke();
        onCollisionEnterSelf.Invoke(gameObject);
        onCollisionEnterOther.Invoke(collision.gameObject);
    }
}
