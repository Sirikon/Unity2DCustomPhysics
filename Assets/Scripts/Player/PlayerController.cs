using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float gravity;
    public float speed;

    SpriteRenderer spriteRenderer;
    Animator animator;
    float playerHeight;

    bool isTouchingFloor = false;
    GameObject touchingFloor;
    float distanceToFloor = 0f;
    Vector3 initialPosition;

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
        isTouchingFloor = hit.distance <= maxDistance && hit.distance != 0 ? true : false;

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
        if (!isTouchingFloor)
        {
            Vector3 positionModifier = (Vector3.down * gravity) * Time.smoothDeltaTime;

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
