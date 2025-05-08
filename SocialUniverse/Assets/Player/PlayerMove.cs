using UnityEngine;
using Unity.Cinemachine;

public class PlayerMove : MonoBehaviour
{
    public float Speed;
    public float Gravity;
    public float JumpForce = 5f; // Adicionamos a força do pulo
    public bool canRotate = true;
    public bool canJump = true; // Adicionamos esta variável para controlar o pulo

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
        // Lógica de movimento horizontal (aplicada sempre que a tecla W é pressionada)
        if (Input.GetKey(KeyCode.W))
        {
            if (cinemachineCamera != null && canRotate)
            {
                float cameraYaw = cinemachineCamera.transform.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
                transform.rotation = targetRotation;
                _moveDirection.x = transform.forward.x * Speed; // Move na direção horizontal
                _moveDirection.z = transform.forward.z * Speed; // Move na direção vertical (no plano horizontal)
            }
            else if (cinemachineCamera == null)
            {
                Debug.LogWarning("Cinemachine Camera não está atribuída, usando direção para frente local.");
                _moveDirection.x = transform.forward.x * Speed;
                _moveDirection.z = transform.forward.z * Speed;
            }
        }
        else
        {
            _moveDirection.x = 0f;
            _moveDirection.z = 0f;
        }

        // Lógica de pulo (só pode pular se estiver no chão)
        if (_controller.isGrounded && canJump && Input.GetButtonDown("Jump"))
        {
            _moveDirection.y = JumpForce;
        }

        // Aplica a gravidade
        _moveDirection.y -= Gravity * Time.deltaTime;

        // Move o CharacterController
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}