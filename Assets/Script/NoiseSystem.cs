using UnityEngine;

public class NoiseSystem : MonoBehaviour
{
    public static NoiseSystem instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void MakeNoise(Vector3 position, float radius)
    {
        Debug.Log("Noise at " + position);

        Collider[] hits = Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hits)
        {
            EnemyPatrol enemy = hit.GetComponentInParent<EnemyPatrol>();
            if (enemy != null)
            {
                enemy.OnHearNoise(position);
            }
        }
    }
}
