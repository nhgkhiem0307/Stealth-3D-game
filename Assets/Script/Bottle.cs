using UnityEngine;

public class Bottle : MonoBehaviour
{
    [Header("Bottle Settings")]
    public float throwForce = 10f;
    public float noiseRadius = 100f;
    public float dropNoise = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

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

        NoiseSystem.instance.MakeNoise(transform.position, dropNoise);
    }

    public void Drop()
    {
        rb.isKinematic = false;
        col.enabled = true;
        transform.parent = null;

        NoiseSystem.instance.MakeNoise(transform.position, dropNoise);
    }

    public void Throw(Vector3 direction)
    {
        rb.isKinematic = false;
        col.enabled = true;
        transform.parent = null;

        rb.AddForce(direction * throwForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        NoiseSystem.instance.MakeNoise(transform.position, noiseRadius);

        if (audioSource != null && hitSound != null) //check co null ko thi bo di cung dc
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(hitSound);
        }
    }
}