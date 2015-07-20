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

    public float gravity;
    public float speed;
    public float jumpTime;
    public float flyAccelerationTime;

    SpriteRenderer spriteRenderer;
    Animator animator;
    float playerHeight;

    bool isTouchingFloor = false;
    bool jumping = false;
    float jumpStartTime = 0;
    bool lastJumpTouchedFloor;

    GameObject touchingFloor;
    float distanceToFloor = 0f;
    Vector3 initialPosition;

    float flyStartTime = 0;
    Ease.Mode gravityEasingMode = Ease.Mode.Linear;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHeight = spriteRenderer.sprite.bounds.size.y * transform.localScale.y;
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        CheckFloorContact();
        ApplyGravity();
        ApplyMovement();
	}

    void CheckFloorContact()
    {
        float maxDistance = playerHeight / 2;
        // Raycast a todo elemento en la capa "Floor" posicionado debajo a cualquier distancia
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Floor"));
        // Si la distancia es menor o igual a la distancia máxima, y la distancia no es 0 (que querría decir una distancia infinita)
        // es que está tocando el suelo
        bool result = hit.distance <= maxDistance && hit.distance != 0 ? true : false;

        if (isTouchingFloor && !result && !jumping)
        {
            flyStartTime = Time.time;
            gravityEasingMode = Ease.Mode.Linear;
        }
            

        isTouchingFloor = result;

        if (isTouchingFloor)
            lastJumpTouchedFloor = true;

        distanceToFloor = hit.distance !=0 ? (hit.distance - maxDistance) : Mathf.Infinity;
    }

    void ApplyMovement()
    {
        Vector3 direction;
        float hAxis = Input.GetAxis("Horizontal");

        if (hAxis < 0)
        {
            animator.SetBool("Walking", true);
            transform.localScale = new Vector3(-1, 1, 1);
            direction = Vector3.left;
        }
        else if (hAxis > 0)
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

        transform.Translate((direction * speed) * Time.smoothDeltaTime);
    }

    
    void ApplyGravity()
    {
        if (Input.GetButtonDown("Jump") && !jumping && lastJumpTouchedFloor && isTouchingFloor)
        {
            jumpStartTime = Time.time;
            flyStartTime = Time.time;
            gravityEasingMode = Ease.Mode.RIn;
            jumping = true;
            lastJumpTouchedFloor = false;
        }

        if (Time.time - jumpStartTime >= jumpTime)
        {
            if (jumping)
            {
                flyStartTime = Time.time;
                gravityEasingMode = Ease.Mode.In;
            }
            
            jumping = false;
        }
            

        if (!isTouchingFloor || jumping)
        {
            Vector3 direction = jumping ? Vector3.up : Vector3.down;
            float lerpValue = Mathf.Min((Time.time - flyStartTime) / flyAccelerationTime, 1);
            
            switch (gravityEasingMode)
            {
                case Ease.Mode.In:
                    lerpValue = Ease.In(lerpValue);
                    break;
                case Ease.Mode.RIn:
                    lerpValue = Ease.RIn(lerpValue);
                    break;
            }

            Vector3 positionModifier = (direction * Mathf.Lerp(0, gravity, lerpValue) * Time.smoothDeltaTime);

            // Si el modificador de altura es mayor a la distancia con el suelo, avanza sólo la distancia restante
            // hasta el suelo, así prevenimos que se incruste en el suelo
            if (-positionModifier.y > distanceToFloor )
                positionModifier.Set(positionModifier.x, -distanceToFloor, positionModifier.z);

            transform.Translate(positionModifier);
        }
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }
}
