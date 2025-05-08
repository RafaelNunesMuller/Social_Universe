using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.EventSystems;

public class PlayerMove : MonoBehaviour
{
    public float Speed;
    public float Gravity;
<<<<<<< Updated upstream
=======
    public float JumpForce = 5f; // Adicionamos a força do pulo
    public bool canRotate = true;
    public bool canJump = true; // Adicionamos esta variável para controlar o pulo
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
            Debug.LogError("Cinemachine FreeLook Camera não foi atribuída ao script PlayerMove no objeto: " + gameObject.name);
        }

        // Oculta e prende o cursor do mouse no início do jogo
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();

        // Opcional: Permite liberar o cursor com a tecla Esc (útil para debugging)
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
                    // Obtém a rotação Y da câmera Cinemachine
                    float cameraYaw = cinemachineCamera.transform.eulerAngles.y;
                    Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);

                    // Aplica a rotação da câmera ao player
                    transform.rotation = targetRotation;

                    // Define a direção para frente baseada NA ROTAÇÃO DO PLAYER (agora alinhada com a câmera)
                    _moveDirection = transform.forward * Speed;
                }
                else
                {
                    Debug.LogWarning("Cinemachine Camera não está atribuída, usando direção para frente local.");
                    _moveDirection = transform.forward * Speed;
                }
=======
                float cameraYaw = cinemachineCamera.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
                transform.rotation = targetRotation;
                _moveDirection.x = transform.forward.x * Speed; // Move na direção horizontal
                _moveDirection.z = transform.forward.z * Speed; // Move na direção vertical (no plano horizontal)
                anim.SetInteger("transition", 1);
            }
            else if (cinemachineCamera == null)
            {
                Debug.LogWarning("Cinemachine Camera não está atribuída, usando direção para frente local.");
                _moveDirection.x = transform.forward.x * Speed;
                _moveDirection.z = transform.forward.z * Speed;
>>>>>>> Stashed changes
            }
            
        }

<<<<<<< Updated upstream
=======
        // Lógica de pulo (só pode pular se estiver no chão)
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