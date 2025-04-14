using UnityEngine;

public class Player_Move : MonoBehaviour
{
    public float Speed;
    public float RotSpeed;
    private float Rotation;
    public float Gravity = 9.81f;
    public float jumpForce = 10.0f;

    private Vector3 MoveDirection;
    private CharacterController controller;
    private Animator anim;
    private float verticalVelocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -Gravity * Time.deltaTime; // Garante que o personagem fique no chão

            // Movimento para frente e trás
            if (Input.GetKey(KeyCode.W))
            {
                MoveDirection = Vector3.forward * Speed;
                anim.SetInteger("transition", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                MoveDirection = Vector3.back * Speed;
                anim.SetInteger("transition", 1);
            }
            else
            {
                MoveDirection = Vector3.zero;
                anim.SetInteger("transition", 0);
            }

            // Pulo
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = jumpForce;
                anim.SetInteger("transition", 2);
            }
        }
        else
        {
            verticalVelocity -= Gravity * Time.deltaTime; // Aplica gravidade quando está no ar
        }

        // Rotação
        Rotation += Input.GetAxis("Horizontal") * RotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, Rotation, 0);

        // Aplica direção do movimento e pulo
        Vector3 move = transform.TransformDirection(MoveDirection);
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }
}
