using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public float healAmount = 25f; // Quanto de vida a po��o vai curar

    public float floatAmplitude = 0.2f; // Altura m�xima que a po��o vai subir e descer (ex: 0.2 unidades da Unity)
    public float floatFrequency = 1f;   // Velocidade da flutua��o (ex: 1 = uma subida/descida completa por segundo)

    private Vector3 startPosition; // Posi��o inicial da po��o

    // --- VARI�VEL PARA O AUDIO SOURCE (NOVA) ---
    private AudioSource audioSource;
    // ------------------------------------------

    void Start()
    {
        // Guarda a posi��o inicial da po��o
        startPosition = transform.position;

        // --- PEGA O COMPONENTE AUDIOSOURCE (NOVO) ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource n�o encontrado no GameObject da Po��o. Adicione um componente AudioSource.");
        }
        // ------------------------------------------
    }

    void Update()
    {
        // --- C�DIGO PARA FLUTUA��O ---
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        // -----------------------------
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger � o jogador (pela Tag "Player")
        if (other.CompareTag("Player"))
        {
            // Tenta obter o script Player_Move do GameObject do jogador
            Player_Move playerMove = other.GetComponent<Player_Move>();

            // Se o jogador tiver o script Player_Move, chama o m�todo Heal
            if (playerMove != null)
            {
                playerMove.Heal(healAmount);
                Debug.Log("Jogador curado em " + healAmount + " pontos de vida!");

                // --- TOCA O SOM DA PO��O (NOVO) ---
                if (audioSource != null && audioSource.clip != null)
                {
                    // Usa PlayOneShot para tocar o som uma vez, mesmo se j� estiver tocando outro som
                    audioSource.PlayOneShot(audioSource.clip);
                }
                // ---------------------------------

                // Destr�i a po��o AP�S o som tocar, se o som for curto.
                // Se o som for longo, pode ser que voc� queira destruir a po��o um pouco depois.
                // Para sons curtos, destruir imediatamente ap�s tocar � ok.
                Destroy(gameObject);
            }
        }
    }
}