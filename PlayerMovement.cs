using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement must haves
    public bool movementAble;
    private Rigidbody2D rb;
    public Animator animator;
    public bool InputEnabled = true;
    //Running
    private bool facingRight = true;
    private float moveInput;
    public float speed;
    //Jumping
    private float checkRadius = 0.2f;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] Transform groundCheck;
    public float jumpForce;
    public bool isGrounded;
    private float inputVertical;
    //Crouching
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Collider2D crouchingCollider;
    public bool isCrouching = false;
    //Shooting
    public Transform standingFirePoint;
    public Transform crouchingFirePoint;
    public Transform UpFirePoint;
    public GameObject bullet;
    [SerializeField] float FireRate = 0.1f;
    private float TimeStamp = 0.0f;
    [SerializeField] GameObject ShootEffect;
    bool isLookingUp;
    public PlatformEffector2D effector;
    public CameraShake cameraShake;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementAble = true;
    }

    private void FixedUpdate()
    {
        //RUNNING AND CHECKING GROUND
        if ((InputEnabled == true)&&(movementAble == true))
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            moveInput = Input.GetAxis("Horizontal");
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            

            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

            //FLIPPING THE SPRITE
            if (facingRight == false && moveInput > 0)
            {
                Flip();
            }
            else if (facingRight == true && moveInput < 0)
            {
                Flip();
            }
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f,180f,0f);
    }

    private void Update()
    {
        //JUMPING
        if ((movementAble == true)&&(InputEnabled == true))
        {
            if (isGrounded)
            {
                //jumpsLeft = extraJumps; - DOUBLE JUMP UPGRADE PERSPECTIVE
                animator.SetBool("isJumping", false);
            }
            else
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("IsShooting", false);
            }
            if (Input.GetButtonDown("Jump") && isGrounded == true)
            {
                Jump();
            }
        }
            //CROUCHING
            if ((Input.GetAxis("Crouch") == 1)&&(InputEnabled == true))
            {
                movementAble = false;
                isCrouching = true;
                animator.SetBool("IsCrouching", true);
                crouchingCollider.enabled = true;
                standingCollider.enabled = false;
            }
            else
            {
                movementAble = true;
                isCrouching = false;
                animator.SetBool("IsCrouching", false);
                standingCollider.enabled = true;
                crouchingCollider.enabled = false;
            }
                 //SHOOTING STANDING
            if (Input.GetButton("Fire1")&&(isCrouching == false)&&(isLookingUp == false)&&(Time.time > TimeStamp)&&(isGrounded == true)&&(InputEnabled == true))
            {
                TimeStamp = Time.time + FireRate;
                animator.SetBool("IsShooting", true);                   
                ShootStanding();
            }
            else if (Input.GetButton("Fire1")== false)
            {
                animator.SetBool("IsShooting", false);
            }

            //SHOOTING CROUCHING
            if (Input.GetButton("Fire1")&&(isCrouching == true)&&(isLookingUp == false)&&(Time.time > TimeStamp)&&(InputEnabled == true))
            {
                TimeStamp = Time.time + FireRate;
                animator.SetBool("IsShooting", true);                 
                ShootCrouching();
            }

            //SHOOTING UP STANDING
            if (Input.GetButton("Fire1")&&(isLookingUp == true)&&(Time.time > TimeStamp)&&(isGrounded == true)&&(isCrouching == false)&&(InputEnabled == true))
            {
                TimeStamp = Time.time + FireRate;
                animator.SetBool("IsShooting", true);
                ShootUp();
            }
                
            if (Input.GetAxis("LookUp") <0)
            {
                isLookingUp = true;
                animator.SetBool("IsLookingUp", true);                
            }
            else if (Input.GetAxis("LookUp") > -1)
            {
                isLookingUp = false;
                animator.SetBool("IsLookingUp", false);
            }
    }


    public void Jump()
    {
        //AudioScript.PlaySound("Jump");
        animator.SetTrigger("TakeOf");
        rb.velocity = Vector2.up * jumpForce;
        animator.SetBool("isJumping", true);
    }

    void ShootStanding()
    {
        Instantiate(ShootEffect, standingFirePoint.position, standingFirePoint.rotation);
        Instantiate(bullet, standingFirePoint.position, standingFirePoint.rotation);
        StartCoroutine(cameraShake.Shake(0.015f, 0.015f));
    }

    void ShootCrouching()
    {
        Instantiate(ShootEffect, crouchingFirePoint.position, crouchingFirePoint.rotation);
        Instantiate(bullet, crouchingFirePoint.position, crouchingFirePoint.rotation);
    }

    void ShootUp()
    {
        Instantiate(ShootEffect, UpFirePoint.position, UpFirePoint.rotation);
        Instantiate(bullet, UpFirePoint.position, UpFirePoint.rotation);
        StartCoroutine(cameraShake.Shake(0.015f, 0.015f));
    }

}
