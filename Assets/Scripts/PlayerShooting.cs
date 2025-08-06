// PlayerShooting.cs
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;      // drag in your GunPoint
    public GameObject bulletPrefab;  // drag in your Bullet prefab

    [Header("Firing")]
    public float fireRate    = 0.15f; // seconds between shots
    public int   bulletDamage = 1;    // how much each shot hurts

    float lastShot;

    void Update()
    {
        if (Input.GetMouseButton(0) &&
            Time.time >= lastShot + fireRate)
        {
            lastShot = Time.time;
            Shoot();
        }
    }

    void Shoot()
    {
        // 1) Spawn a brand‐new bullet at the muzzle
        GameObject b = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        // 2) Assign its damage (requires your bullet prefab to have an EnemyBullet or similar script)
        var eb = b.GetComponent<EnemyBullet>();
        if (eb != null)
        {
            eb.damage = bulletDamage;
        }

        // 3) Auto‐destroy it after 2 seconds
        Destroy(b, 2f);
    }
}
