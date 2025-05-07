using UnityEngine;
using Unity.Cinemachine;

public class PlayerMove : MonoBehaviour
{
    public float Speed;
    public float Gravity;

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
            }
        }

        _moveDirection.y -= Gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}