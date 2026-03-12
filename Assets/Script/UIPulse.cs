using UnityEngine;

public class UIPulse : MonoBehaviour
{
    public float speed = 2f;      // tốc độ phóng to thu nhỏ
    public float scaleAmount = 0.05f; // độ phóng to

    private Vector3 startScale;

    void Start()
    {
        startScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = startScale * scale;
    }
}