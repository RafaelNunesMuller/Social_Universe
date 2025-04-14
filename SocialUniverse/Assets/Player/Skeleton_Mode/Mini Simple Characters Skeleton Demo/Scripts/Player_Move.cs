using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player_Move : MonoBehaviour
{
    public float Speed;
    public float RotSpeed;
    private float Rotation;
    public float Gravity;
    public bool isGrounded;
    Rigidbody rb;

    public Vector3 jump;
    public float jumpForce = 2.0f;

    Vector3 MoveDirection;
    CharacterController controller;
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
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
            

            if (Input.GetKey(KeyCode.W))
            {
                MoveDirection = Vector3.forward * Speed;
                MoveDirection = transform.TransformDirection(MoveDirection);
                anim.SetInteger("transition", 1);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                MoveDirection = Vector3.zero;
                anim.SetInteger("transition", 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                MoveDirection = Vector3.back * Speed;
                MoveDirection = transform.TransformDirection(MoveDirection);
                anim.SetInteger("transition", 1);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                MoveDirection = Vector3.zero;
                anim.SetInteger("transition", 0);
            }


            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.AddForce(jump * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }

        }
        Rotation += Input.GetAxis("Horizontal") * RotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, Rotation, 0);

        MoveDirection.y -= Gravity * Time.deltaTime;
        controller.Move(MoveDirection * Time.deltaTime);
    }

}
