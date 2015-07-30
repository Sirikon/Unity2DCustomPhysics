using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    /* Public vars */
    public float speed;
    public Vector3 direction = Vector3.right;

    /* Bullet's components */
    SpriteRenderer spriteRenderer;

    /* Public non-serializable vars */
    public float distance;

    private Vector3 startDistance;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ResetStartDistance();
    }
	
	void Update () {
        LookToDirection();
        MoveBullet();
        UpdateDistance();
	}

    void LookToDirection()
    {
        transform.localScale = new Vector3(direction == Vector3.right ? 1 : -1, 1, 1);
    }

    void MoveBullet()
    {
        transform.Translate(direction * speed * Time.smoothDeltaTime);
    }

    void UpdateDistance()
    {
        distance = Mathf.Abs(this.transform.position.x - startDistance.x);
    }

    public void ResetStartDistance()
    {
        distance = 0;
        startDistance = this.transform.position;
    }
}
