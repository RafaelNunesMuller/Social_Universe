using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necessário para gerenciar cenas

public class HealthPlayer : MonoBehaviour
{
    [Header("UI de Vida")]
    public Slider HealthSlider;
    public float MaxHealth = 100f;
    public float Health;

    [Header("Animações")]
    public Animator anim;

    [Header("Estado do Jogador")]
    public bool isDead = false;

    // --- REFERÊNCIA AO SCRIPT DE MOVIMENTO DO JOGADOR ---
    private Player_Move playerMoveScript;
    // ----------------------------------------------------

    // --- VARIÁVEL PARA REDUÇÃO DE DANO DA DEFESA ---
    [Header("Defesa")]
    [Range(0f, 1f)] // Restringe o valor entre 0 e 1 no Inspector
    public float defenseDamageReduction = 0.5f; // Redução de dano (0.5 = 50% de redução)
                                                // 0.0f = 100% de redução (imune)
                                                // 1.0f = 0% de redução (nenhuma defesa)
    // ---------------------------------------------------

    void Start()
    {
        Health = MaxHealth;
        if (HealthSlider != null)
        {
            HealthSlider.maxValue = MaxHealth;
            HealthSlider.value = Health;
        }
        else
        {
            Debug.LogWarning("HealthSlider não atribuído no Inspector de HealthPlayer! A barra de vida não será atualizada.");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator não encontrado no GameObject do jogador com HealthPlayer! Animações de vida/morte podem não funcionar.");
        }

        playerMoveScript = GetComponent<Player_Move>();
        if (playerMoveScript == null)
        {
            Debug.LogError("Player_Move script não encontrado no GameObject do jogador. A lógica de defesa e imobilização pode falhar!");
        }
        
        isDead = false; // Garante que o jogador não comece morto
    }

    void Update()
    {
        // Atualiza a barra de vida se houver uma e o valor for diferente
        if (HealthSlider != null && HealthSlider.value != Health)
        {
            HealthSlider.value = Health;
        }

        // --- Lógica de Morte ---
        if (Health <= 0 && !isDead)
        {
            Health = 0; // Garante que a vida não fique negativa
            isDead = true;
            Debug.Log("Player Morreu! Iniciando lógica de morte.");

            if (anim != null)
            {
                anim.SetBool("DEATH", true); // Ativa a animação de morte
            }

            // Desativa os controles e input do jogador
            if (playerMoveScript != null)
            {
                playerMoveScript.canMove = false;
                playerMoveScript.canRotate = false;
                playerMoveScript.canJump = false;
                playerMoveScript.canAttack = false;
                playerMoveScript.canDefend = false;
            }

            // Desabilita o CharacterController para impedir colisões e física após a morte
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // Reinicia a fase após um delay
            Invoke("ReloadCurrentScene", 0.9f); // Chama o método ReloadCurrentScene após 6 segundos
        }
        // Lógica para reviver (se você implementar um sistema de revive)
        else if (Health > 0 && isDead)
        {
            isDead = false;
            if (anim != null)
            {
                anim.SetBool("DEATH", false);
            }
            if (playerMoveScript != null)
            {
                playerMoveScript.canMove = true;
                playerMoveScript.canRotate = true;
                playerMoveScript.canJump = true;
                playerMoveScript.canAttack = true;
                playerMoveScript.canDefend = true;
            }
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = true;
            }
        }
    }

    // --- Lógica de Dano (MODIFICADA para incluir defesa) ---
    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            Debug.Log("Jogador já está morto, não leva mais dano.");
            return;
        }

        float finalDamage = amount;

        // Se o player está defendendo (verificando a flag 'isDefending' no Player_Move)
        if (playerMoveScript != null && playerMoveScript.isDefending)
        {
            finalDamage = amount * defenseDamageReduction; 
            Debug.Log($"Dano {amount} recebido. Defesa ativa. Dano reduzido para {finalDamage}.");
        }
        
        Health -= finalDamage;
        Debug.Log("Vida atual do Player: " + Health);
    }

    // --- Lógica de Cura ---
    public void Heal(float amount)
    {
        if (isDead)
        {
            Debug.Log("Jogador morto, não pode curar.");
            return;
        }

        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        Debug.Log("Vida atual do Player: " + Health);
    }

    // --- Método para recarregar a cena (chamado após a morte) ---
    private void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // --- Métodos de Chave (mantidos aqui se forem usados) ---
    
}