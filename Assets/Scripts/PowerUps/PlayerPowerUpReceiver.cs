using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerPowerUpReceiver : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("Shield VFX prefab (MeshRenderer veya Sprite olabilir). Player'ın CHILD'ı olarak spawn edilir.")]
    [SerializeField] private GameObject shieldVisualPrefab;

    [Tooltip("Shield görselini gemiyi saracak şekilde büyütmek için çarpan.")]
    [SerializeField] private float shieldScaleMultiplier = 2f;

    [Tooltip("Shield görselinin player'a göre local offset'i.")]
    [SerializeField] private Vector3 shieldLocalOffset = Vector3.zero;

    [Tooltip("URP 2D'de Mesh/Sprite üstte çizilsin diye local Z offset. Örn: -1")]
    [SerializeField] private float shieldZOffset = -1f;

    [Header("Shield UI")]
    [Tooltip("Shield kalan süresini göstermek için TMP Text. Canvas'ta sağ/sol altta konumlandır.")]
    [SerializeField] private TextMeshProUGUI shieldTimerText;

    [Header("Shield Runtime (read-only)")]
    [SerializeField] private int shieldHitsRemaining = 0;
    [SerializeField] private float shieldEndsAt = 0f;

    [Header("Overcharge Runtime (read-only)")]
    [SerializeField] private float overchargeEndsAt = 0f;

    private PlayerController _player;
    private Coroutine _overchargeRoutine;

    private GameObject _shieldVisualInstance;

    public bool ShieldActive => shieldHitsRemaining > 0 && Time.time < shieldEndsAt;
    public float ShieldRemainingSeconds => ShieldActive ? Mathf.Max(0f, shieldEndsAt - Time.time) : 0f;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        if (_player == null)
            Debug.LogWarning("PlayerPowerUpReceiver: PlayerController yok. Power-up etkileri kısıtlı olabilir.");

        // Inspector'da atanmadıysa sahneden isme göre bulmayı dene
        if (shieldTimerText == null)
        {
            var go = GameObject.Find("ShieldTimerText");
            if (go != null) shieldTimerText = go.GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        // Shield UI countdown
        if (shieldTimerText != null)
        {
            if (ShieldActive)
            {
                int seconds = Mathf.CeilToInt(ShieldRemainingSeconds);
                shieldTimerText.gameObject.SetActive(true);
                shieldTimerText.text = $"Shield: {seconds}s";
            }
            else
            {
                shieldTimerText.gameObject.SetActive(false);
            }
        }

        // Shield bitti -> VFX temizle
        if (!ShieldActive && _shieldVisualInstance != null)
            DestroyShieldVisual();
    }

    /// <summary>
    /// PlayerController bunu hasarı uygulamadan önce çağırır.
    /// True dönerse damage shield tarafından bloklandı demektir.
    /// </summary>
    public bool TryBlockDamage(int amount)
    {
        if (amount <= 0) return false;
        if (!ShieldActive) return false;

        // her hasarda 1 hit azalt
        shieldHitsRemaining = Mathf.Max(0, shieldHitsRemaining - 1);

        if (shieldHitsRemaining <= 0)
        {
            shieldEndsAt = 0f;
            DestroyShieldVisual();
        }

        return true;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        if (_player != null)
            _player.AddHealth(amount);
    }

    /// <summary>
    /// Enemy vb. scriptlerin oyuncuya hasar uygulaması için güvenli wrapper.
    /// </summary>
    public void ApplyDamage(int amount)
    {
        if (amount == 0) return;

        if (_player == null) _player = GetComponent<PlayerController>();
        if (_player != null)
            _player.TakeDamage(amount);
    }

    public void ActivateShield(float durationSeconds, int hits)
    {
        durationSeconds = Mathf.Max(0.1f, durationSeconds);
        hits = Mathf.Max(1, hits);

        shieldHitsRemaining = hits;
        shieldEndsAt = Time.time + durationSeconds;

        SpawnOrRefreshShieldVisual();
    }

    public void ActivateOvercharge(float durationSeconds, float fireCooldownMultiplier, int bonusDamage, int bonusBullets)
    {
        durationSeconds = Mathf.Max(0.1f, durationSeconds);
        fireCooldownMultiplier = Mathf.Clamp(fireCooldownMultiplier, 0.1f, 2f);

        overchargeEndsAt = Time.time + durationSeconds;

        if (_overchargeRoutine != null)
            StopCoroutine(_overchargeRoutine);

        _overchargeRoutine = StartCoroutine(OverchargeRoutine(durationSeconds, fireCooldownMultiplier, bonusDamage, bonusBullets));
    }

    private void SpawnOrRefreshShieldVisual()
    {
        if (shieldVisualPrefab == null) return;

        DestroyShieldVisual();

        _shieldVisualInstance = Instantiate(shieldVisualPrefab, transform);
        _shieldVisualInstance.name = $"{shieldVisualPrefab.name} (ShieldRuntime)";

        // URP 2D: Z ile öne al
        Vector3 local = shieldLocalOffset;
        local.z = shieldZOffset;
        _shieldVisualInstance.transform.localPosition = local;

        _shieldVisualInstance.transform.localScale =
            _shieldVisualInstance.transform.localScale * shieldScaleMultiplier;
    }

    private void DestroyShieldVisual()
    {
        if (_shieldVisualInstance != null)
        {
            Destroy(_shieldVisualInstance);
            _shieldVisualInstance = null;
        }
    }

    private IEnumerator OverchargeRoutine(float durationSeconds, float fireCooldownMultiplier, int bonusDamage, int bonusBullets)
    {
        PhaserWeapon weapon = FindObjectOfType<PhaserWeapon>();
        if (weapon == null)
        {
            _overchargeRoutine = null;
            yield break;
        }

        float baseCooldown = weapon.GetFireCooldown();
        int baseDamage = weapon.damage;
        int baseBullets = weapon.bulletCount;

        weapon.SetFireCooldown(baseCooldown * fireCooldownMultiplier);
        weapon.damage = baseDamage + Mathf.Max(0, bonusDamage);
        weapon.bulletCount = baseBullets + Mathf.Max(0, bonusBullets);

        float end = Time.time + durationSeconds;
        while (Time.time < end) yield return null;

        if (weapon != null)
        {
            weapon.SetFireCooldown(baseCooldown);
            weapon.damage = baseDamage;
            weapon.bulletCount = baseBullets;
        }

        _overchargeRoutine = null;
    }
}
