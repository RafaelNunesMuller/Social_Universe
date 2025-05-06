using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;
    private int currentPatrolIndex;

    private NavMeshAgent agent;
    private Animator animator;

    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float visionAngle = 60f;
    public float lostTargetDelay = 3f;

    private float lostTimer = 0f;
    private bool playerInSight = false;

    private EnemyState currentState = EnemyState.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInSight = IsPlayerVisible();

        if (playerInSight && distanceToPlayer <= detectionRange)
        {
            lostTimer = 0f; // Resetar contador de perda
            if (distanceToPlayer <= attackRange)
            {
                agent.ResetPath();
                SetState(EnemyState.Attack);
            }
            else
            {
                agent.SetDestination(player.position);
                SetState(EnemyState.Chase);
            }
        }
        else if (currentState == EnemyState.Chase || currentState == EnemyState.Attack)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostTargetDelay)
            {
                GoToNextPatrolPoint();
                SetState(EnemyState.Patrol);
            }
        }
        else
        {
            // Patrulhar
            if (!agent.pathPending && agent.remainingDistance < 0.3f)
            {
                GoToNextPatrolPoint();
            }
            SetState(EnemyState.Patrol);
        }
    }

    bool IsPlayerVisible()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < visionAngle / 2f)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 1.2f, directionToPlayer);
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void SetState(EnemyState state)
    {
        if (currentState != state)
        {
            currentState = state;
            animator.SetInteger("speed", (int)state);
        }
    }
}

public enum EnemyState
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3
}
