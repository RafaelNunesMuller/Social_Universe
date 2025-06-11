using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public float healAmount = 25f; // Quanto de vida a poção vai curar

    public float floatAmplitude = 0.2f; // Altura máxima que a poção vai subir e descer (ex: 0.2 unidades da Unity)
    public float floatFrequency = 1f;   // Velocidade da flutuação (ex: 1 = uma subida/descida completa por segundo)

    private Vector3 startPosition; // Posição inicial da poção

    // --- VARIÁVEL PARA O AUDIO SOURCE (NOVA) ---
    private AudioSource audioSource;
    // ------------------------------------------

    void Start()
    {
        // Guarda a posição inicial da poção
        startPosition = transform.position;

        // --- PEGA O COMPONENTE AUDIOSOURCE (NOVO) ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource não encontrado no GameObject da Poção. Adicione um componente AudioSource.");
        }
        // ------------------------------------------
    }

    void Update()
    {
        // --- CÓDIGO PARA FLUTUAÇÃO ---
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        // -----------------------------
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é o jogador (pela Tag "Player")
        if (other.CompareTag("Player"))
        {
            // Tenta obter o script Player_Move do GameObject do jogador
            Player_Move playerMove = other.GetComponent<Player_Move>();

            // Se o jogador tiver o script Player_Move, chama o método Heal
            if (playerMove != null)
            {
                playerMove.Heal(healAmount);
                Debug.Log("Jogador curado em " + healAmount + " pontos de vida!");

                // --- TOCA O SOM DA POÇÃO (NOVO) ---
                if (audioSource != null && audioSource.clip != null)
                {
                    // Usa PlayOneShot para tocar o som uma vez, mesmo se já estiver tocando outro som
                    audioSource.PlayOneShot(audioSource.clip);
                }
                // ---------------------------------

                // Destrói a poção APÓS o som tocar, se o som for curto.
                // Se o som for longo, pode ser que você queira destruir a poção um pouco depois.
                // Para sons curtos, destruir imediatamente após tocar é ok.
                Destroy(gameObject);
            }
        }
    }
}