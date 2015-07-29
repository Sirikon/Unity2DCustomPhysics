using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    /* Public vars */
    public float speed;
    public Vector3 direction = Vector3.right;

    /* Bullet's components */
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	void Update () {
        LookToDirection();
        MoveBullet();
	}

    void LookToDirection()
    {
        transform.localScale = new Vector3(direction == Vector3.right ? 1 : -1, 1, 1);
    }

    void MoveBullet()
    {
        transform.Translate(direction * speed * Time.smoothDeltaTime);
    }
}
