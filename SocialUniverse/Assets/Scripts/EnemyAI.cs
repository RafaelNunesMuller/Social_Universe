using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform playerTransform;
    // public float chaseRange = 10f; // NÃO SERÁ MAIS USADO para detectar, pois o trigger faz isso

    public string animFloatSpeed = "Speed";

    private NavMeshAgent agent;
    private Animator anim;
    private bool isPlayerInChaseRange = false; // <<< ESSA FLAG É FUNDAMENTAL

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent não encontrado no inimigo: " + gameObject.name + ". Certifique-se de que ele está anexado.");
            enabled = false;
            return;
        }
        if (anim == null)
        {
            Debug.LogError("Animator não encontrado no inimigo: " + gameObject.name + ". Certifique-se de que ele está anexado.");
            enabled = false;
            return;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Player não encontrado. Certifique-se de que o jogador tem a tag 'Player' ou atribua a referência manualmente no Inspector do inimigo.");
                enabled = false;
                return;
            }
        }

        // No início, o inimigo deve estar IDLE por padrão
        isPlayerInChaseRange = false; // Garante que começa fora da zona
        Idle(); // Força o estado idle no início
    }

    void Update()
    {
        if (playerTransform == null || !agent.enabled) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("NavMeshAgent do inimigo não está na NavMesh! Verifique o Bake e a posição inicial.");
            return;
        }

        // A lógica principal agora depende da flag isPlayerInChaseRange
        if (isPlayerInChaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);
        anim.SetFloat(animFloatSpeed, 1f);
        // Debug.Log("Estado: Perseguindo. isPlayerInChaseRange: " + isPlayerInChaseRange); // Opcional, se precisar de mais detalhes
    }

    void Idle()
    {
        agent.isStopped = true;
        anim.SetFloat(animFloatSpeed, 0f);
        // Debug.Log("Estado: Idle. isPlayerInChaseRange: " + isPlayerInChaseRange); // Opcional, se precisar de mais detalhes
    }

    // Chamado quando outro Collider entra neste trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInChaseRange = true;
            Debug.Log("Player entrou na DetectionZone! Perseguindo...");
        }
    }

    // Chamado quando outro Collider sai deste trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInChaseRange = false;
            Debug.Log("Player saiu da DetectionZone! Voltando para Idle...");
        }
    }
}