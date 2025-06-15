using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float patrolRadius = 10f;
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5f;
    public float waitTime = 2f;
    public float detectionRange = 8f;

    private Vector3 centerPoint;
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    private bool isChasing = false;
    private bool waiting = false;

    public float Health;
    public float MaxHealth = 100;

    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private float lastAttackTime = -999f;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("Jogador com tag 'Player' não encontrado!");

        centerPoint = transform.position;
        MoveToNewPatrolPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            agent.isStopped = true;
            agent.ResetPath();

            // Ataca se estiver no cooldown
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("isAttacking");
                lastAttackTime = Time.time;
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing)
            {
                isChasing = true;
                agent.speed = chaseSpeed;
            }

            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
        }

        else
        {
            // Voltar para patrulha
            if (isChasing)
            {
                isChasing = false;
                agent.speed = patrolSpeed;
                MoveToNewPatrolPoint();
            }

            // Continua patrulhando
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!waiting)
                {
                    waiting = true;
                    animator.SetBool("isWalking", false);
                    Invoke(nameof(MoveToNewPatrolPoint), waitTime);
                }
            }
            else
            {
                animator.SetBool("isWalking", true);

            }
        }
        
        
    }

    

    void MoveToNewPatrolPoint()
    {
        Vector3 newPos = RandomNavSphere(centerPoint, patrolRadius, NavMesh.AllAreas);
        agent.SetDestination(newPos);
        waiting = false;

        animator.SetBool("isWalking", true);
        
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPoint != Vector3.zero ? centerPoint : transform.position, patrolRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
