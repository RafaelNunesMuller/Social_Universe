using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public float Speed;
    public float Gravity;
<<<<<<< Updated upstream
=======
    public float JumpForce = 5f; // Adicionamos a for�a do pulo
    public bool canRotate = true;
    public bool canJump = true; // Adicionamos esta vari�vel para controlar o pulo
    private Animator anim;
    private Vector3 MoveDirection;
>>>>>>> Stashed changes

    private Vector3 _moveDirection;
    private CharacterController _controller;

    [Tooltip("A Cinemachine FreeLook Camera")]
    public CinemachineCamera cinemachineCamera;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (cinemachineCamera == null)
        {
            Debug.LogError("Cinemachine FreeLook Camera n�o foi atribu�da ao script PlayerMove no objeto: " + gameObject.name);
        }

        // Oculta e prende o cursor do mouse no in�cio do jogo
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();

        // Opcional: Permite liberar o cursor com a tecla Esc (�til para debugging)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Move()
    {
        if (_controller.isGrounded)
        {
            _moveDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
<<<<<<< Updated upstream
                if (cinemachineCamera != null)
                {
                    // Obt�m a rota��o Y da c�mera Cinemachine
                    float cameraYaw = cinemachineCamera.transform.eulerAngles.y;
                    Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);

                    // Aplica a rota��o da c�mera ao player
                    transform.rotation = targetRotation;

                    // Define a dire��o para frente baseada NA ROTA��O DO PLAYER (agora alinhada com a c�mera)
                    _moveDirection = transform.forward * Speed;
                }
                else
                {
                    Debug.LogWarning("Cinemachine Camera n�o est� atribu�da, usando dire��o para frente local.");
                    _moveDirection = transform.forward * Speed;
                }
=======
                float cameraYaw = cinemachineCamera.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
                transform.rotation = targetRotation;
                _moveDirection.x = transform.forward.x * Speed; // Move na dire��o horizontal
                _moveDirection.z = transform.forward.z * Speed; // Move na dire��o vertical (no plano horizontal)
                anim.SetInteger("transition", 1);
            }
            else if (cinemachineCamera == null)
            {
                Debug.LogWarning("Cinemachine Camera n�o est� atribu�da, usando dire��o para frente local.");
                _moveDirection.x = transform.forward.x * Speed;
                _moveDirection.z = transform.forward.z * Speed;
>>>>>>> Stashed changes
            }
            
        }

<<<<<<< Updated upstream
=======
        // L�gica de pulo (s� pode pular se estiver no ch�o)
        if (_controller.isGrounded && canJump && Input.GetButtonDown("Jump"))
        {
            _moveDirection.y = JumpForce;
            anim.SetInteger("transition", 2);
        }

        // Aplica a gravidade
>>>>>>> Stashed changes
        _moveDirection.y -= Gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}