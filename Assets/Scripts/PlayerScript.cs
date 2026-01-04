using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    //Player
    public float speed = 5f;
    public float jumpForce = 6f;

    private Rigidbody2D rb;
    private Animator characterAnimator;
    public float bounceForce = 8f;

    public bool bFaceRight;

    //Ground check
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;


    // Input
    private Vector2 moveInput;
    private bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        characterAnimator = GetComponent<Animator>();
        Physics2D.queriesStartInColliders = false;
    }

    //Input calls

    //Move action
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    //Jump action
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }

    //Fisicas

    void FixedUpdate()
    {
        float horizontalMovement = moveInput.x;
        characterAnimator.SetFloat("MovementSpeed", Mathf.Abs(horizontalMovement));


        bool isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);

        // SALTO
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        jumpPressed = false;

        if (horizontalMovement < 0 && bFaceRight ||
            horizontalMovement > 0 && !bFaceRight)
        {
            Turn();
        }
    }

    void Turn() //testing con el spriterenderer
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;

        bFaceRight = !bFaceRight;
    }


}
