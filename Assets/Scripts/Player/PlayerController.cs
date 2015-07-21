using UnityEngine;
using System.Collections;

public static class Ease
{
    public enum Mode
    {
        Linear = 0,
        In = 1,
        RIn = 2
    }

    /// <summary>
    /// Linear
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public static float Linear(float k)
    {
        return k;
    }

    /// <summary>
    /// Ease In
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public static float In(float k)
    {
        return Mathf.Pow(k, 2);
    }

    /// <summary>
    /// Reversed Ease In
    /// </summary>
    /// <returns></returns>
    public static float RIn(float k)
    {
        return 1- Mathf.Pow(k, 2);
    }
}

public class PlayerController : MonoBehaviour {

    /* Public/Tweakable vars */
    public float gravity;
    public float speed;
    public float jumpForce;
    public float jumpForceDuration;

    /* Initial Vars */
    SpriteRenderer spriteRenderer;
    float playerHeight;
    float playerWidth;
    Animator animator;
    Vector3 initialPosition;
    Vector3 previousPosition;

    /* Ground Collision Control */
    bool isGrounded = false;
    float distanceToGround = 0f;
    float rayLength;
    Vector3 ray_bottom_left_position;
    Vector3 ray_bottom_right_position;
    RaycastHit2D hit_bottom_left;
    RaycastHit2D hit_bottom_right;

    /* Input */
    float horizontalAxis = 0f;
    bool inputJump = false;

    /* Gravity */
    Ease.Mode gravityEasingMode = Ease.Mode.Linear;
    float flyStartTime = 0;
    bool isFalling = false;

    /* Jump */
    bool isJumping = false;
    float jumpStartTime = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHeight = spriteRenderer.sprite.bounds.size.y * transform.localScale.y;
        playerWidth = spriteRenderer.sprite.bounds.size.x * transform.localScale.x;
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        previousPosition = initialPosition;
    }
	
    void Update()
    {
        // Read input from the user
        CheckInput();
        // Update raycasts, check distance to floor and collisions
        UpdateRaycasts();
        CheckDistanceToGround();
        CheckGrounded();
        // Save actual position
        SavePosition();
        // Apply the Jump physics
        ApplyJump();
        // Update distance to ground after jump
        UpdateRaycasts();
        CheckDistanceToGround();
        // Apply gravity and movement
        ApplyGravity();
        ApplyMovement();
    }

    void UpdateRaycasts()
    {
        rayLength = playerHeight / 2;
        // Ray positions
        ray_bottom_left_position = new Vector3(transform.position.x - (playerWidth / 2), transform.position.y);
        ray_bottom_right_position = new Vector3(transform.position.x + (playerWidth / 2), transform.position.y);

        // Raycast to every element at the "Floor" layer over the player at any distance
        hit_bottom_left = Physics2D.Raycast(ray_bottom_left_position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Floor"));
        hit_bottom_right = Physics2D.Raycast(ray_bottom_right_position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Floor"));

        Debug.DrawLine(ray_bottom_left_position, hit_bottom_left.point, Color.red);
        Debug.DrawLine(ray_bottom_right_position, hit_bottom_right.point, Color.red);
    }

    void CheckDistanceToGround()
    {
        distanceToGround = Mathf.Min(hit_bottom_left.distance, hit_bottom_right.distance) - rayLength;
    }

    void CheckGrounded()
    {
        float previousFrameYDiff = transform.position.y - previousPosition.y;

        bool isGroundedLeft = hit_bottom_left.distance == rayLength || (hit_bottom_left.distance <= rayLength && hit_bottom_left.distance - previousFrameYDiff >= rayLength);
        bool isGroundedRight = hit_bottom_right.distance == rayLength || (hit_bottom_right.distance <= rayLength && hit_bottom_right.distance - previousFrameYDiff >= rayLength);

        isGrounded = isGroundedLeft || isGroundedRight;

        if (isGrounded && isFalling)
            isJumping = false;
    }

    void CheckInput()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        inputJump = Input.GetButtonDown("Jump");
    }

    void ApplyMovement()
    {
        Vector3 direction;

        if (horizontalAxis < 0)
        {
            animator.SetBool("Walking", true);
            transform.localScale = new Vector3(-1, 1, 1);
            direction = Vector3.left;
        }
        else if (horizontalAxis > 0)
        {
            animator.SetBool("Walking", true);
            transform.localScale = new Vector3(1, 1, 1);
            direction = Vector3.right;
        }
        else
        {
            animator.SetBool("Walking", false);
            direction = Vector3.zero;
        }

        transform.Translate(direction * speed * Time.smoothDeltaTime);
    }

    
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            Vector3 positionModifier = (Vector3.down * gravity * Time.smoothDeltaTime);

            // If the position modifier is higher than the distance to the floor, just forward that
            // this way we prevent traspassing the floor
            if ( -positionModifier.y > distanceToGround && isFalling )
                positionModifier.Set(positionModifier.x, -distanceToGround, positionModifier.z);

            transform.Translate(positionModifier);
        }
    }

    void ApplyJump()
    {
        if(inputJump)
        {
            isJumping = true;
            jumpStartTime = Time.time;
        }

        if(Time.time - jumpStartTime >= jumpForceDuration)
        {
            isJumping = false;
        }

        if (isJumping)
        {
            float lerpValue = (Time.time - jumpStartTime) / jumpForceDuration;
            float lerpedJumpForce = Mathf.Lerp(jumpForce, 0, lerpValue);
            isFalling = lerpedJumpForce <= gravity ? true : false;
            Vector3 positionModifier = (Vector3.up * lerpedJumpForce * Time.smoothDeltaTime);
            transform.Translate(positionModifier);
        }
        else
        {
            isFalling = true;
        }
        Debug.Log(isFalling);
    }

    public void SavePosition()
    {
        previousPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
