using UnityEngine;

public class Attck : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Collider weaponCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("isAttacking", true); // Ativa anima��o de ataque (trigger)
        }
        else
        {
            anim.SetBool("isAttacking", false);
        }
    }

    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
}
