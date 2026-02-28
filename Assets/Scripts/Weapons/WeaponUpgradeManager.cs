// 12/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;

using UnityEngine;
using TMPro;

public class WeaponUpgradeManager : MonoBehaviour
{
    public int currentWeaponLevel = 1;
    public int pointsPerLevel = 200;
    public int maxWeaponLevel = 5;

    public TextMeshProUGUI upgradeText; // Silah seviyesi UI bildirimi için

    private void Start()
    {
        if (upgradeText != null)
        {
            upgradeText.gameObject.SetActive(false);
        }
    }

    public void CheckForUpgrade(int score)
    {
        int newLevel = Mathf.Min(maxWeaponLevel, score / pointsPerLevel + 1);

        if (newLevel > currentWeaponLevel)
        {
            int levelDifference = newLevel - currentWeaponLevel;
            currentWeaponLevel = newLevel;

            UpgradeWeapon(levelDifference);
            ShowUpgradeMessage();
        }
    }

    private void UpgradeWeapon(int levelDifference)
    {
        if (PhaserWeapon.Instance != null)
        {
            PhaserWeapon.Instance.speed += levelDifference * 2; // Mermi hızını artır
            PhaserWeapon.Instance.damage += levelDifference; // Mermi hasarını artır
            PhaserWeapon.Instance.bulletCount += levelDifference; // Aynı anda çıkan mermi sayısını artır
        }
    }

    private void ShowUpgradeMessage()
    {
        if (upgradeText != null)
        {
            upgradeText.text = $"Weapon Upgraded! Level {currentWeaponLevel}";
            upgradeText.gameObject.SetActive(true);
            Invoke(nameof(HideUpgradeMessage), 2f); // 2 saniye sonra mesajı gizle
        }
    }

    private void HideUpgradeMessage()
    {
        if (upgradeText != null)
        {
            upgradeText.gameObject.SetActive(false);
        }
    }

    public void ResetWeaponLevel()
    {
        currentWeaponLevel = 1;
        if (PhaserWeapon.Instance != null)
        {
            PhaserWeapon.Instance.speed = 10; // Varsayılan hız
            PhaserWeapon.Instance.damage = 1; // Varsayılan hasar
        }
    }
}
