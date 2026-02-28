using UnityEngine;

public class Critter1 : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    private float moveSpeed;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private float moveTimer;
    private float moveInterval;

    [SerializeField] private GameObject zappedEffect;
    [SerializeField] private GameObject burnEffect;

    [Header("Combat")]
    [SerializeField] private int contactDamage = 1;

    private bool alreadyHitPlayer = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprites != null && sprites.Length > 0)
            spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        GenerateRandomPosition();
        moveSpeed = Random.Range(2f, 5f);
        moveInterval = Random.Range(0.3f, 2f);
        moveTimer = moveInterval;
    }

    private void Update()
    {
        if (moveTimer > 0)
        {
            moveTimer -= Time.deltaTime;
        }
        else
        {
            GenerateRandomPosition();
            moveInterval = Random.Range(0.3f, 2f);
            moveTimer = moveInterval;
        }

        if (GameManager.Instance != null)
            targetPosition -= new Vector3(GameManager.Instance.worldSpeed * Time.deltaTime, 0);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        Vector3 relativePos = targetPosition - transform.position;
        if (relativePos != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(Vector3.forward, relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void GenerateRandomPosition()
    {
        targetPosition = new Vector3(transform.position.x, Random.Range(-4f, 4f), transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (GameManager.Instance != null) GameManager.Instance.AddScore(1);

            if (zappedEffect != null)
                Instantiate(zappedEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayModifiedSound(AudioManager.Instance.squished);

            if (GameManager.Instance != null) GameManager.Instance.critterCounter++;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (alreadyHitPlayer) return;
            alreadyHitPlayer = true;

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

            if (burnEffect != null)
                Instantiate(burnEffect, transform.position, transform.rotation);

            Destroy(gameObject);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayModifiedSound(AudioManager.Instance.burn);

            if (GameManager.Instance != null) GameManager.Instance.critterCounter++;
        }
    }
}
