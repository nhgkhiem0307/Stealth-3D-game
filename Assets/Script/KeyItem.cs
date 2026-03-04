using UnityEngine;

public class KeyItem : MonoBehaviour
{
    Rigidbody rb;
    Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform holdPoint)
    {
        rb.isKinematic = true;
        col.enabled = false;

        transform.parent = holdPoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        NoiseSystem.instance.MakeNoise(transform.position, 2);
    }

    public void Drop()
    {
        rb.isKinematic = false;
        col.enabled = true;
        transform.parent = null;
        NoiseSystem.instance.MakeNoise(transform.position, 2);
    }
}