using UnityEngine;

public class Key : MonoBehaviour
{

    public float floatAmplitude = 0.2f; // Altura m�xima que a po��o vai subir e descer (ex: 0.2 unidades da Unity)
    public float floatFrequency = 1f;
    private Vector3 startPosition; // Posi��o inicial da po��o
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        // --- C�DIGO PARA FLUTUA��O ---
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        // -----------------------------
    }


    public void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é o jogador (pela Tag "Player")
        if (other.CompareTag("Player"))
        {
            // Tenta obter o script Health_player do GameObject do jogador
            Player_Move playerMove = other.GetComponent<Player_Move>();

            // Se o jogador tiver o script Health_player, chama o m�todo AddKey
            playerMove.AddKey();
            Debug.Log("Chave coletada!");

            // Destrói a chave após ser coletada
            Destroy(gameObject);
        }
    }
}
