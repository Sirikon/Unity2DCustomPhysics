using UnityEngine;
using System.Collections;

public class BulletManagerController : MonoBehaviour {

    /* Bullet array */
    GameObject[] bullets = new GameObject[100];
    SpriteRenderer[] bulletRenderers = new SpriteRenderer[100];
    BulletController[] bulletControllers = new BulletController[100];
    bool[] bulletActive = new bool[100];
    int bulletCount = 0;
    int activeBullets = 0;

	// Update is called once per frame
	void Update () {
        DespawnInvisibleActiveBullets();
	}

    void DespawnInvisibleActiveBullets()
    {
        for(int i = 0; i < bulletCount; i++)
        {
            if (!bulletRenderers[i].isVisible && bulletActive[i])
            {
                Despawn(i);
            }
        }
    }

    void InstantiateNewBullet()
    {
        GameObject bullet = Instantiate(Resources.Load("Bullet", typeof(GameObject)) as GameObject);
        SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bullets[bulletCount] = bullet;
        bulletRenderers[bulletCount] = bulletRenderer;
        bulletControllers[bulletCount] = bulletController;
        bulletActive[bulletCount] = false;
        bulletCount++;
        bullet.transform.parent = this.gameObject.transform;
        bullet.SetActive(false);
        bullet.name = "Bullet #" + bulletCount.ToString() + " (New)";
    }

    int GetInactiveBulletIndex()
    {
        for(int i = 0; i < bulletCount; i++)
        {
            if (!bulletActive[i])
                return i;
        }
        return -1;
    }

    public void Spawn(Vector3 position, Vector3 direction)
    {
        // Instantiate a new bullet from Resources
        if(bulletCount - activeBullets == 0)
            InstantiateNewBullet();

        int i = GetInactiveBulletIndex();
        GameObject bullet = bullets[i];
        BulletController bulletController = bulletControllers[i];
        bullet.SetActive(true);
        bulletActive[i] = true;
        bullet.name = "Bullet #"+i.ToString();
        // The bullet's initial position is the player's position
        bullet.transform.position = position;
        // Bullet's direction depends on where is the player looking to
        bulletController.direction = direction;
        activeBullets++;
        Debug.Log("Spawned");
    }

    void Despawn(int i)
    {
        GameObject bullet = bullets[i];
        BulletController bulletController = bulletControllers[i];
        bullet.transform.position = Vector3.zero;
        bulletController.direction = Vector3.zero;
        bullet.SetActive(false);
        bulletActive[i] = false;
        bullet.name = "Bullet #" + i.ToString() + " (Despawned)";
        activeBullets--;
    }
}
