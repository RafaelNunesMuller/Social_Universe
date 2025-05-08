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
            }
        }

        _moveDirection.y -= Gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
    }
}