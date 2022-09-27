using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f;
    private CharacterController charController;
    private Animator anim;
    

    void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        if(move.magnitude >= 0.1f)
        {
            charController.Move(move * speed * Time.deltaTime);
            if(move != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * 10f);
            }
        }

        
        
    }
}
