using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PowerUpPickup : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private PowerUpType type = PowerUpType.Shield;

    [Header("Optional FX")]
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private GameObject pickupVfx;

    [Header("Overcharge")]
    [SerializeField] private float overchargeDuration = 6f;
    [SerializeField] private float fireCooldownMultiplier = 0.6f; // 0.5 => 2x hızlı
    [SerializeField] private int bonusDamage = 2;
    [SerializeField] private int bonusBullets = 1;

    [Header("Shield")]
    [SerializeField] private float shieldDuration = 8f;
    [SerializeField] private int shieldHits = 1;

    [Header("Health")]
    [SerializeField] private int healAmount = 1;

    private bool _consumed;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed) return;
        if (!other.CompareTag("Player")) return;

        var receiver = other.GetComponent<PlayerPowerUpReceiver>();
        if (receiver == null)
            receiver = other.gameObject.AddComponent<PlayerPowerUpReceiver>();

        switch (type)
        {
            case PowerUpType.Overcharge:
                receiver.ActivateOvercharge(overchargeDuration, fireCooldownMultiplier, bonusDamage, bonusBullets);
                break;
            case PowerUpType.Shield:
                receiver.ActivateShield(shieldDuration, shieldHits);
                break;
            case PowerUpType.Health:
                receiver.Heal(healAmount);
                break;
        }

        _consumed = true;

        if (pickupVfx != null)
            Instantiate(pickupVfx, transform.position, Quaternion.identity);

        if (pickupSound != null)
            pickupSound.Play();

        if (pickupSound != null && pickupSound.clip != null)
            Destroy(gameObject, pickupSound.clip.length);
        else
            Destroy(gameObject);
    }
}
