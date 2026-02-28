using UnityEngine;

public class Boss1 : MonoBehaviour
{
    private Animator animator;
    private float speedX;
    private float speedY;
    private bool charging;

    private float switchInterval;
    private float switchTimer;

    [Header("Combat")]
    [SerializeField] private int lives = 100;
    [SerializeField] private int contactDamage = 20;

    private bool isDead = false;

    [SerializeField] private GameObject destroyEffect;

    private void Start()
    {
        animator = GetComponent<Animator>();
        EnterChargeState();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound(AudioManager.Instance.bossSpawn);
    }

    private void Update()
    {
        if (isDead) return;
        if (PlayerController.Instance == null) return;

        if (switchTimer > 0)
        {
            switchTimer -= Time.deltaTime;
        }
        else
        {
            charging = !charging;

            if (charging)
                EnterChargeState();
            else
                EnterRandomMoveState();
        }

        transform.position += new Vector3(speedX, speedY, 0) * Time.deltaTime;

        if (animator != null)
            animator.SetFloat("speedX", Mathf.Abs(speedX));
    }

    private void EnterChargeState()
    {
        speedX = -6f;
        speedY = 0f;
        switchInterval = Random.Range(1.0f, 2.0f);
        switchTimer = switchInterval;

        charging = true;
        if (animator) animator.SetBool("charging", true);
    }

    private void EnterRandomMoveState()
    {
        speedX = Random.Range(-2f, -4f);
        speedY = Random.Range(-1.5f, 1.5f);

        switchInterval = Random.Range(0.8f, 1.5f);
        switchTimer = switchInterval;

        charging = false;
        if (animator) animator.SetBool("charging", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
            if (asteroid) asteroid.TakeDamage(contactDamage);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            // Shield varsa bloklansın diye PlayerPowerUpReceiver üzerinden hasar ver
            PlayerPowerUpReceiver receiver = collision.gameObject.GetComponent<PlayerPowerUpReceiver>();
            if (receiver != null)
            {
                receiver.ApplyDamage(contactDamage);
            }
            else
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null) player.TakeDamage(contactDamage);
            }
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(5);
            // Bullet kendi scriptinde kapanıyor olabilir, burada Destroy etmiyoruz.
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayModifiedSound(AudioManager.Instance.hitArmor);

        lives -= damageAmount;

        if (lives <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, transform.rotation);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayModifiedSound(AudioManager.Instance.bossDeath);

        Destroy(gameObject);
    }
}
