using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    public float detectionRange = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);

            // Atualiza o parâmetro do Animator (ajuste conforme o Animator do asset)
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }
}
