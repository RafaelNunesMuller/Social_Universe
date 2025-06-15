using UnityEngine;

public class Player_Move : MonoBehaviour
{
    // Referência ao CharacterController da Unity
    private CharacterController controller;

    // Velocidade de movimento
    public float speed = 5f;

    // Força do pulo
    public float jumpForce = 8f;

    // Gravidade aplicada ao personagem
    public float gravity = -9.81f;

    // Velocidade vertical (queda, pulo, etc.)
    private float verticalVelocity;

    // Referência ao Animator (para animações)
    private Animator anim;

    public bool isDead = false; 

    // Referência à câmera (para rotação com o mouse)
    public Transform cameraTransform;

    // Sensibilidade do mouse
    public float mouseSensitivity = 2f;

    // Acumulador para rotação vertical (câmera)
    private float xRotation = 0f;

    // Para controlar ataque (gatilho da animação)
    public float attackCooldown = 0.5f;
    private float lastAttackTime = -0.5f;

    // --- REFERÊNCIA AO SCRIPT DE SAÚDE DO JOGADOR ---
    private HealthPlayer healthPlayerScript;
    // --------------------------------------------------

    // Variáveis de controle de diálogo e outras interrupções de input
    public bool canMove = true;
    public bool canRotate = true;
    public bool canJump = true;
    public bool canAttack = true;
    public bool canDefend = true; // Controla se o jogador pode defender (ex: em diálogo)

    // --- VARIÁVEL PARA O ESTADO DE DEFESA ---
    public bool isDefending { get; private set; } = false; // Acessível por outros scripts, mas só pode ser definida aqui
    // -----------------------------------------

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        healthPlayerScript = GetComponent<HealthPlayer>(); // Obtém a referência ao script HealthPlayer

        if (!controller.enabled)
        {
            controller.enabled = true;
            Debug.LogWarning("CharacterController estava desativado e foi ativado no Start.");
        }
        if (healthPlayerScript == null)
        {
            Debug.LogError("HealthPlayer script não encontrado no GameObject do jogador! Verifique se ele está anexado.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        isDefending = false; // Garante que o jogador não comece defendendo
    }

    void Update()
    {
        // --- VERIFICAÇÃO PRINCIPAL: NÃO FAZER NADA SE O JOGADOR ESTIVER MORTO ---
        if (healthPlayerScript != null && healthPlayerScript.isDead)
        {
            // Garante que as animações de movimento/ataque sejam resetadas
            anim.SetBool("isWalking", false);
            anim.SetBool("isRuning", false);
            anim.SetBool("isAttacking", false);
            StopDefending(); // Garante que a defesa seja desativada se morrer
            return; // Sai do Update imediatamente
        }
        // -------------------------------------------------------------------------

        verticalVelocity += gravity * Time.deltaTime;

        // ----------- MANEJO DE ATAQUE E DEFESA (COM PRIORIDADE) ----------------------
        // Se o jogador pressionar o botão de ataque, prioriza o ataque e desativa a defesa
        if (canAttack && Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime + attackCooldown)
        {
            if (isDefending) // Se estiver defendendo, pare de defender para atacar
            {
                StopDefending();
            }
            anim.SetBool("isAttacking", true);
            lastAttackTime = Time.time;
        }
        else if (anim.GetBool("isAttacking") && Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetBool("isAttacking", false);
        }

        // Se o jogador pressionar o botão de defesa, prioriza a defesa e não permite ataque
        // E só permite defesa se não estiver atacando OU se estiver defendendo mas o botão foi solto
        if (canDefend && Input.GetButtonDown("Fire2"))
        {
            // Só ativa a defesa se não estiver na animação de ataque
            if (!anim.GetBool("isAttacking"))
            {
                StartDefending();
            }
            else
            {
                Debug.Log("Não pode defender enquanto está atacando!");
            }
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            StopDefending();
        }

        // ----------- MOVIMENTAÇÃO ----------------
        // Só permite mover se não estiver defendendo e o input de movimento estiver ativado
        if (!isDefending && canMove)
        {
            MovePlayerCharacter();
        }
        else // Se estiver defendendo ou não puder mover, garanta que as animações de movimento estão desativadas
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRuning", false);
        }

        // ----------- PULO ---------------------
        // Não pode pular se estiver defendendo ou se o input estiver desativado
        // Esta parte do código fica aqui no Update, mas deve ser verificada APÓS o manejo de defesa e ataque
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }
            if (canJump && Input.GetKeyDown(KeyCode.Space) && !isDefending)
            {
                anim.SetTrigger("jump");
                verticalVelocity = jumpForce;
            }
        }

        Vector3 verticalMove = Vector3.up * verticalVelocity;
        controller.Move(verticalMove * Time.deltaTime);

        // ----------- ROTACIONA O PLAYER COM O MOUSE (CÂMERA) ----------------
        if (canRotate)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
            else
            {
                Debug.LogWarning("Camera Transform não atribuída ao Player_Move no Inspector!");
            }
        }
    }

    // --- FUNÇÕES DE LÓGICA DE JOGO ---

    void StartDefending()
    {
        if (!isDefending) // Só ativa se não estiver defendendo
        {
            isDefending = true;
            // Como não há animação de defesa, apenas garanta que outras animações estão desativadas
            anim.SetBool("isWalking", false);
            anim.SetBool("isRuning", false);
            anim.SetBool("isAttacking", false);
            Debug.Log("Defesa Ativada! Player não pode mover ou atacar.");
        }
    }

    void StopDefending()
    {
        if (isDefending) // Só desativa se já estiver defendendo
        {
            isDefending = false;
            Debug.Log("Defesa Desativada!");
        }
    }

    public void MovePlayerCharacter()
    {
        // Esta função é chamada apenas se o player PODE mover (verificado em Update)
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");    // W/S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // NORMALIZAÇÃO: Garante que o movimento diagonal não seja mais rápido
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        if (move.magnitude > 0)
        {
            if (Input.GetButton("Fire3")) // Geralmente "Left Shift"
            {
                anim.SetBool("isRuning", true);
                anim.SetBool("isWalking", false); // Garante que isWalking não sobreponha
                controller.Move(speed * 3f * Time.deltaTime * move);
            }
            else
            {
                anim.SetBool("isRuning", false);
                anim.SetBool("isWalking", true);
                controller.Move(speed * Time.deltaTime * move);
            }
        }
        else // Player está parado
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRuning", false);
        }
    }

    public void Heal(float amount)
    {
        if (healthPlayerScript != null && !healthPlayerScript.isDead)
        {
            healthPlayerScript.Heal(amount);
            Debug.Log("Jogador curado em " + amount + " pontos de vida!");
        }
        else
        {
            Debug.LogWarning("Jogador está morto ou HealthPlayer script não encontrado!");
        }
    }

    // Método chamado por um Animation Event no final da animação de ataque (se usado)
    public void EndAttack()
    {
        // Só desativa a animação de ataque se o jogador NÃO estiver morto
        if (healthPlayerScript != null && !healthPlayerScript.isDead)
        {
            anim.SetBool("isAttacking", false);
        }
    }
    
    public int keysCollected = 0;

    public void AddKey()
    {
        if (isDead) return;
        keysCollected++;
        Debug.Log("Chave coletada! Total de chaves: " + keysCollected);
    }

    public bool HasKey()
    {
        return keysCollected > 0;
    }

    public void UseKey()
    {
        if (isDead) return;
        if (keysCollected > 0)
        {
            Debug.Log("Chave usada. Chaves restantes: " + keysCollected);
        }
        else
        {
            Debug.Log("Não há chaves para usar.");
        }
    }
}