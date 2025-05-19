using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Busca o jogador pela tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogError("Jogador com tag 'Player' não encontrado!");
    }

    void Update()
    {
        if (player != null)
        {
            // Atualiza o destino do inimigo a cada frame
            agent.SetDestination(player.position);
        }
    }
}