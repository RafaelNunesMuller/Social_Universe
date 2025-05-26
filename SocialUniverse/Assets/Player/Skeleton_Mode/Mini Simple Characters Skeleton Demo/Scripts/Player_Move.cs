using UnityEngine;

public class Player_Move : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Refer�ncia ao CharacterController da Unity
    private CharacterController controller;

    // Velocidade de movimento
    public float speed = 5f;

    // For�a do pulo
    public float jumpForce = 8f;

    // Gravidade aplicada ao personagem
    public float gravity = -9.81f;

    // Velocidade vertical (queda, pulo, etc.)
    private float verticalVelocity;

    // Refer�ncia ao Animator (para anima��es)
    private Animator anim;

    // Refer�ncia � c�mera (para rota��o com o mouse)
    public Transform cameraTransform;

    // Sensibilidade do mouse
    public float mouseSensitivity = 2f;

    // Acumulador para rota��o vertical (c�mera)
    private float xRotation = 0f;

    // Para controlar ataque (gatilho da anima��o)
    private bool isAttacking = false;

    public float MaxHealth = 100f;

    public float Health;


    void Start()
    {
        // Pegando os componentes necess�rios na cena
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Bloqueia e esconde o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Health = MaxHealth;

    }

    void Update()
    {

        Move();
        // ----------- PULO ---------------------
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // "cola" no ch�o
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = jumpForce;

            // Ativa anima��o de pulo (trigger)
            anim.SetTrigger("jump");
        }

        // Aplica gravidade
        verticalVelocity += gravity * Time.deltaTime;
        Vector3 verticalMove = Vector3.up * verticalVelocity;
        controller.Move(verticalMove * Time.deltaTime);

        // ----------- ATAQUE ----------------------
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            anim.SetBool("isAttacking", true); // Ativa anima��o de ataque (trigger)
        }
        else{
            anim.SetBool("isAttacking", false);
        }

        // ----------- ROTACIONA O PLAYER COM O MOUSE (C�MERA) ----------------
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Roda o player horizontalmente
        transform.Rotate(Vector3.up * mouseX);

        // Roda a c�mera verticalmente (limitada para n�o girar demais)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void Move()
    {
        // ----------- MOVIMENTA��O ----------------
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // ----------- ANIMA��O DE WALK/IDLE -------------
        // Se estiver se movendo, ativa a anima��o de corrida (trigger bool)
        if (move != Vector3.zero )
        {
            controller.Move(speed * Time.deltaTime * move ); // Aplica movimenta��o
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (move != Vector3.zero  && Input.GetButton("Fire3"))
        {
            anim.SetBool("isRuning", true);
            controller.Move(speed * Time.deltaTime * move ); // Aplica movimenta��o
            
        }
        else
        {
            anim.SetBool("isRuning", false);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            anim.SetBool("isDefending", true);
        }
        else
        {
            anim.SetBool("isDefending", false);
        }

        if (Health ==0)
        {
            anim.SetBool("DEATH", true);
        }

        else
        {
            anim.SetBool("DEATH", false);
        }

    }


    // Chamado pela anima��o no fim do ataque para desbloquear
    public void EndAttack()
    {
        isAttacking = false;
    }
}