using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator anim;
    public Collider weaponCollider;

    
    void Start()
    {
        weaponCollider = GetComponent<Collider>();

        weaponCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") )
        {
            anim.SetBool("isAttacking", true);
        }
    }


    public void EnableWeaponCollider()
    {

        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }
}
