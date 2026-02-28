using UnityEngine;

public class PhaserWeapon : MonoBehaviour
{
    public static PhaserWeapon Instance { get; private set; }

    [Header("Bullet Prefab & Spawn")]
    [SerializeField] private PhaserBullet bulletPrefab;
    [SerializeField] private Transform firePoint; // yoksa bu objenin transformu kullanılacak

    [Header("Weapon Stats")]
    public float speed = 10f;   // WeaponUpgradeManager bunu artırıyor
    public int damage = 1;      // WeaponUpgradeManager bunu artırıyor
    public int bulletCount = 1; // Her 200 puanda +1 olacak

    [Header("Fire Control")]
    [SerializeField] private float fireCooldown = 0.2f;
    private float lastFireTime;

    [Header("Spread")]
    [SerializeField] private float spreadAngle = 12f; // çoklu mermi açı yayılımı

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// PlayerController burayı çağırıyor olmalı.
    /// </summary>
    public void Shoot()
    {
        if (Time.time < lastFireTime + fireCooldown) return;
        lastFireTime = Time.time;

        if (bulletPrefab == null)
        {
            Debug.LogError("PhaserWeapon: bulletPrefab is not assigned!");
            return;
        }

        Vector3 origin = (firePoint != null) ? firePoint.position : transform.position;

        int count = Mathf.Max(1, bulletCount);

        // Tek mermi
        if (count == 1)
        {
            SpawnBullet(origin, 0f);
            return;
        }

        // Çoklu mermi: -spread/2 ... +spread/2
        float total = spreadAngle;
        float step = (count == 2) ? total : total / (count - 1);
        float start = -total * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = start + step * i;
            SpawnBullet(origin, angle);
        }
    }

    public float GetFireCooldown() => fireCooldown;

    public void SetFireCooldown(float value)
    {
        fireCooldown = Mathf.Max(0.02f, value);
    }
   



    private void SpawnBullet(Vector3 pos, float angleDeg)
    {
        PhaserBullet b = Instantiate(bulletPrefab, pos, Quaternion.identity);
        b.SetDamage(damage);
        b.SetDirection(Quaternion.Euler(0, 0, angleDeg) * Vector3.right); // sağa doğru, açıyla
    }
}
