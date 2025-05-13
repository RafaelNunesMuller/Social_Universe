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
    private bool isAttacking = false;

    void Start()
    {
        // Pegando os componentes necessários na cena
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Bloqueia e esconde o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        Move();
        // ----------- PULO ---------------------
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // "cola" no chão
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = jumpForce;

            // Ativa animação de pulo (trigger)
            anim.SetTrigger("jump");
        }

        // Aplica gravidade
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 verticalMove = Vector3.up * verticalVelocity;
        controller.Move(verticalMove * Time.deltaTime);

        // ----------- ATAQUE ----------------------
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("attack"); // Ativa animação de ataque (trigger)
        }

        // ----------- ROTACIONA O PLAYER COM O MOUSE (CÂMERA) ----------------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Roda o player horizontalmente
        transform.Rotate(Vector3.up * mouseX);

        // Roda a câmera verticalmente (limitada para não girar demais)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void Move()
    {
        // ----------- MOVIMENTAÇÃO ----------------
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // ----------- ANIMAÇÃO DE WALK/IDLE -------------
        // Se estiver se movendo, ativa a animação de corrida (trigger bool)
        if (move != Vector3.zero && controller.isGrounded)
        {
            controller.Move(speed * Time.deltaTime * move); // Aplica movimentação
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

    }


    // Chamado pela animação no fim do ataque para desbloquear
    public void EndAttack()
    {
        isAttacking = false;
    }
}