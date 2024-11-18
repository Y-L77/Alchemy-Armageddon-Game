using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Player movement speed
    public float jumpForce = 10f; // Jump force
    public float coyoteTime = 0.2f; // Time after leaving ground where jump is still allowed
    public LayerMask groundLayer; // Layer for ground detection
    public bool isGrounded; // Whether the player is grounded
    private float coyoteTimer; // Timer for coyote time

    public Rigidbody2D rb;
    private bool isTurnedRight = true; // Boolean to check if player is facing right

    // Ground check box variables (editable in the inspector)
    public Vector2 boxSize = new Vector2(1f, 0.2f); // Size of the ground check box (width x height)
    public float boxOffsetY = 0.5f; // Offset to lower or raise the box from the player's position
    public float boxCastDistance = 0.2f; // Distance to check for ground

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJumping();
        HandleRotation();
        CheckGroundStatus();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Apply movement based on user input
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void HandleJumping()
    {
        // Allow jump if grounded or during coyote time
        if (isGrounded || coyoteTimer > 0)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Apply jump force
                coyoteTimer = 0; // Reset coyote timer after jumping
            }
        }

        // Countdown coyote timer when not grounded
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void HandleRotation()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // If moving right but facing left or moving left but facing right, flip the player
        if (moveInput > 0 && !isTurnedRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isTurnedRight)
        {
            Flip();
        }
    }

    private void CheckGroundStatus()
    {
        // Adjust box position to be below the player using boxOffsetY
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y - boxOffsetY);

        // Check if the player is grounded using a boxcast with customizable parameters
        RaycastHit2D hit = Physics2D.BoxCast(boxPosition, boxSize, 0f, Vector2.down, boxCastDistance, groundLayer);

        // If something was hit by the BoxCast, the player is grounded
        isGrounded = hit.collider != null;

        // If grounded, reset coyote time
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
    }

    private void Flip()
    {
        isTurnedRight = !isTurnedRight;

        // Flip the player's local scale on the x-axis
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // Also flip all children objects (if needed for animations, etc.)
        foreach (Transform child in transform)
        {
            child.localScale = new Vector3(-child.localScale.x, child.localScale.y, child.localScale.z);
        }
    }

    private void OnDrawGizmos()
    {
        // Make sure the Rigidbody2D exists before drawing the Gizmo
        if (rb != null)
        {
            // Set Gizmo color to blue for visibility
            Gizmos.color = Color.blue;

            // Adjusted position for the Gizmo, based on the boxOffsetY and the player's position
            Vector3 boxPosition = transform.position - new Vector3(0f, boxOffsetY, 0f);

            // Draw the box that represents the area being checked for ground collision
            Gizmos.DrawWireCube(boxPosition, new Vector3(boxSize.x, boxSize.y, 0f));
        }
    }
}
