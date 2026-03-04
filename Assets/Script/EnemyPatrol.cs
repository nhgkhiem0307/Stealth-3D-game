using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;


public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float waitTime = 1f;

    [Header("Vision")]
    public float viewDistance = 8f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public Transform player;

    enum EnemyState
    {
        Patrol,
        Investigating,
        Alert
    }

    EnemyState currentState = EnemyState.Patrol;

    public AudioSource audioSource;

    public float detectionTime = 2f;
    private float currentDetection = 0f;

    float noiseTimer = 0f;
    float noiseDuration = 2f;
    public float investigateTime = 2f;

    private Vector3 investigatePosition;
    private int currentWaypointIndex;

    NavMeshAgent agent;
    int currentIndex;
    float waitCounter;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        agent.destination = waypoints[0].position;

    }

    void Update()
    {
   
        
            switch (currentState)
            {
                case EnemyState.Patrol:
                    Patrol();
                    break;

                case EnemyState.Investigating:
                    HandleNoiseReaction();
                    break;

                case EnemyState.Alert:
                    // hmm update sau idk man
                    break;
            }

            DetectPlayer();
            HandleFootstepSound();
        
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter >= waitTime)
            {
                currentIndex = (currentIndex + 1) % waypoints.Length;
                agent.destination = waypoints[currentIndex].position;
                waitCounter = 0f;
            }
        }
    }

    void DetectPlayer()
    {
        bool canSee = false;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= viewDistance)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle <= viewAngle / 2f)
            {
                if (Physics.Raycast(transform.position + Vector3.up*2f,
                    directionToPlayer.normalized,
                    out RaycastHit hit,
                    viewDistance, obstacleMask))
                {
                    if (hit.transform == player)
                    {
                        canSee = true;
                    }
                }
            }
        }

        // XỬ LÝ DETECTION
        if (canSee)
        {
            currentDetection += Time.deltaTime;

            if (currentDetection >= detectionTime)
            {
                currentDetection = detectionTime;
                GameManager.instance.PlayerCaught();
            }
        }
        else
        {
            currentDetection -= Time.deltaTime * 2f;
        }

        // Clamp trc
        currentDetection = Mathf.Clamp(currentDetection, 0, detectionTime);

        // Ssau đó tính percent
        float percent = currentDetection / detectionTime;


        if (percent > GameManager.instance.maxDetectionPercent)
        {
            GameManager.instance.maxDetectionPercent = percent;
        }
    }

    public void OnHearNoise(Vector3 noisePos)
    {
        currentState = EnemyState.Investigating;//qua trang thai dieu tra am thanh
        noiseTimer = noiseDuration;

        agent.SetDestination(noisePos);
    }

    void HandleNoiseReaction()
    {

        noiseTimer -= Time.deltaTime;

        if (noiseTimer <= 0f)
        {
            currentState = EnemyState.Patrol;
            agent.SetDestination(waypoints[currentIndex].position);
        }
    }

    void HandleFootstepSound()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    void OnDrawGizmosSelected()//vẽ tầm nhìn
    {
        Gizmos.color = Color.yellow;

        Vector3 eyePos = transform.position + Vector3.up*2f;

        // Tia chính diện
        Gizmos.DrawRay(eyePos, transform.forward * viewDistance);

        // Tia biên trái
        Vector3 leftDir = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(eyePos, leftDir * viewDistance);

        // Tia biên phải
        Vector3 rightDir = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;
        Gizmos.DrawRay(eyePos, rightDir * viewDistance);
    }

}
