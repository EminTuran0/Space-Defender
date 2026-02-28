using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float worldSpeed;

    public int critterCounter;

    public int score; 
    public TextMeshProUGUI scoreText; 
    [SerializeField] private GameObject boss1;

    void Awake(){
        if (Instance != null){
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    void Start()
    {
        critterCounter = 0;
        worldSpeed = 1f; // Default value for worldSpeed
        UpdateScoreText();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P) || Input.GetButtonDown("Fire3")){
            Pause();
        }

        if (critterCounter > 15){
            critterCounter = 0;
            Instantiate(boss1, new Vector2(15f, 0), Quaternion.Euler(0,0,-90));
        }
    }


    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();

        // Silah seviyesini kontrol et
        WeaponUpgradeManager weaponUpgradeManager = FindObjectOfType<WeaponUpgradeManager>();
        if (weaponUpgradeManager != null)
        {
            weaponUpgradeManager.CheckForUpgrade(score);
        }
    }
    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    // --- YENİ EKLENEN KISIM BAŞLANGIÇ ---
    public void LevelComplete()
    {
        WeaponUpgradeManager weaponUpgradeManager = FindObjectOfType<WeaponUpgradeManager>();
        if (weaponUpgradeManager != null)
        {
            weaponUpgradeManager.ResetWeaponLevel();
        }

        // Puanı "FinalScore" adıyla kaydediyoruz
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();

        // Bölüm bitiş sahnesini yüklüyoruz
        SceneManager.LoadScene("Level 1 Complete");
    }
    // --- YENİ EKLENEN KISIM BİTİŞ ---

    public void Pause(){
        if (UIController.Instance.pausePanel.activeSelf == false){
            UIController.Instance.pausePanel.SetActive(true);
            Time.timeScale = 0f;
            AudioManager.Instance.PlaySound(AudioManager.Instance.pause);
        } else {
            UIController.Instance.pausePanel.SetActive(false);
            Time.timeScale = 1f;
            PlayerController.Instance.ExitBoost();
            AudioManager.Instance.PlaySound(AudioManager.Instance.unpause);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void GoToMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        WeaponUpgradeManager weaponUpgradeManager = FindObjectOfType<WeaponUpgradeManager>();
        if (weaponUpgradeManager != null)
        {
            weaponUpgradeManager.ResetWeaponLevel();
        }

        StartCoroutine(ShowGameOverScreen());
    }

    IEnumerator ShowGameOverScreen(){
        yield return new WaitForSeconds(3f);
        PlayerPrefs.SetInt("FinalScore", score);
        SceneManager.LoadScene("GameOver");
    }

    public void SetWorldSpeed(float speed){
        worldSpeed = speed;
    }
}