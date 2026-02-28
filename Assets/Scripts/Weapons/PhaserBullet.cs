using UnityEngine;

public class PhaserBullet : MonoBehaviour
{
    private int damage = 1;
    private Vector3 direction = Vector3.right;

    [Header("Optional VFX")]
    [SerializeField] private GameObject zappedEffect; // Critter vurulunca oynatmak istersen
    // burnEffect burada gereksiz (Player çarpınca Critter scripti yakma efektini yapıyor)

    public void SetDamage(int value) => damage = Mathf.Max(1, value);

    public void SetDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) dir = Vector3.right;
        direction = dir.normalized;
    }

    void Update()
    {
        if (PhaserWeapon.Instance == null) return;

        transform.position += direction * PhaserWeapon.Instance.speed * Time.deltaTime;

        // Out of bounds
        if (transform.position.x > 9 || transform.position.x < -9 || transform.position.y > 5 || transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Asteroid
        if (collision.CompareTag("Obstacle"))
        {
            Asteroid asteroid = collision.GetComponent<Asteroid>();
            if (asteroid) asteroid.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Boss
        if (collision.CompareTag("Boss"))
        {
            Boss1 boss = collision.GetComponent<Boss1>();
            if (boss) boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Critter (Skor + öldür)
        if (collision.CompareTag("Critter"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(1);

            if (zappedEffect != null)
                Instantiate(zappedEffect, collision.transform.position, Quaternion.identity);

            // Critter sayacı vs gerekiyorsa:
            if (GameManager.Instance != null)
                GameManager.Instance.critterCounter++;

            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }
    }
}
