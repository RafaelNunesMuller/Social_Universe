using UnityEngine;

public class Player_Move : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Referência à câmera (para rotação com o mouse)
    public Transform cameraTransform;

    // Sensibilidade do mouse
    public float mouseSensitivity = 2f;

    // Acumulador para rotação vertical (câmera)
    private float xRotation = 0f;

    // Para controlar ataque (gatilho da animação)
    public float attackCooldown = 0.5f; // Adicionado cooldown para ataque
    private float lastAttackTime = -0.5f; // Inicializado para permitir o primeiro ataque

    public  float MaxHealth;
    public float Health;

    // Variáveis de controle de diálogo
    public bool canMove = true;    // Controla se o jogador pode andar
    public bool canRotate = true;  // Controla se o jogador pode rotacionar com o mouse
    public bool canJump = true;    // Controla se o jogador pode pular
    public bool canAttack = true;  // Controla se o jogador pode atacar
    public bool canDefend = true;  // Controla se o jogador pode defender

    void Start()
    {
        // Pegando os componentes necessários na cena
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        if (!controller.enabled)
        {
            controller.enabled = true;
            Debug.LogWarning("CharacterController estava desativado e foi ativado no Start.");
        }

        // Bloqueia e esconde o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Health = MaxHealth;
    }

    void Update()
    {
        // Aplica a gravidade sempre, independentemente do estado do chão
        verticalVelocity += gravity * Time.deltaTime;

        // ----------- PULO ---------------------
        if (controller.isGrounded)
        {
            // "cola" no chão apenas se estiver caindo
            if (verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }

            if (canJump && Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
                anim.SetTrigger("jump");
            }

            Vector3 verticalMove = Vector3.up * verticalVelocity;
            controller.Move(verticalMove * Time.deltaTime);

        }
        
        // Aplica o movimento vertical
        
        


        // ----------- MOVIMENTAÇÃO ----------------
        MovePlayerCharacter(); // Chamada para a função de movimentação

        // ----------- ATAQUE ----------------------
        // Permite atacar se canAttack for verdadeiro, o botão for pressionado, e o cooldown terminou
        if (canAttack && Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetBool("isAttacking", true);
            lastAttackTime = Time.time;
        }
        // Se a animação de ataque está em andamento (isAttacking true), mantém a bool ativa no Animator
        // para garantir que a animação seja reproduzida completamente até EndAttack() resetar isAttacking.
        
        else
        {
            anim.SetBool("isAttacking", false);
        }

        // ----------- DEFESA ----------------------
        if (canDefend && Input.GetButtonDown("Fire2"))
        {
            anim.SetBool("isDefending", true);
        }
        else if (Input.GetButtonUp("Fire2")) // Adicionado Input.GetButtonUp para desativar a defesa ao soltar o botão
        {
            anim.SetBool("isDefending", false);
        }

        // ----------- ROTACIONA O PLAYER COM O MOUSE (CÂMERA) ----------------
        if (canRotate)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Roda o player horizontalmente
            transform.Rotate(Vector3.up * mouseX);

            // Roda a câmera verticalmente (limitada para não girar demais)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            if (cameraTransform != null) // Adicionado check para evitar NullReferenceError
            {
                cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
            else
            {
                Debug.LogWarning("Camera Transform não atribuída ao Player_Move no Inspector!");
            }
        }

        // ----------- ANIMAÇÃO DE MORTE -------------
        if (Health <= 0)
        {
            anim.SetBool("DEATH", true);
            
        }

        else
        {
            anim.SetBool("DEATH", false);
        }
    }

    // Função de movimentação separada
    public void MovePlayerCharacter()
    {
        

        if (!canMove)
        {
            // Se não pode mover, zera o input para evitar movimento residual
            anim.SetBool("isWalking", false);
            anim.SetBool("isRuning", false);
            return;
        }

        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");    // W/S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Animação de Walk/Run
        if (move.magnitude > 0) // Verifica se há algum movimento (moveX ou moveZ não são zero)
        {
            // Movimento de corrida (shift)
            if (Input.GetButton("Fire3")) // Geralmente "Left Shift"
            {
                anim.SetBool("isRuning", true);
                anim.SetBool("isWalking", false); // Garante que Walking não esteja ativo junto com Running
                controller.Move(speed * 3f * Time.deltaTime * move);
                
                // Exemplo: dobro da velocidade para corrida
            }
            else
            {
                anim.SetBool("isRuning", false);
                anim.SetBool("isWalking", true);
                controller.Move(speed * Time.deltaTime * move);
            
            }
        }
        else
        {
            // Parado
            anim.SetBool("isWalking", false);
            anim.SetBool("isRuning", false);
        }
    }

    // Chamado pela animação no fim do ataque para desbloquear
    public void EndAttack()
    {
        anim.SetBool("isAttacking", false); // Garante que a bool do Animator seja resetada
    }

    // Exemplo de como o inimigo ou outro script pode causar dano
    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log("Player Health: " + Health);
        if (Health <= 0)
        {
            // Lógica de morte
            Debug.Log("Player Morreu!");
        }
    }
}