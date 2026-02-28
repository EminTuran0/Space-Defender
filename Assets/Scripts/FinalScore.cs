using UnityEngine;
using TMPro; // TextMeshPro için gerekli

public class FinalScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Editörden bağlayacağımız yazı

    void Start()
    {
        // Hafızadan puanı çek, eğer puan yoksa 0 getir
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        
        // Ekrana yazdır
        scoreText.text = "Your Score: " + finalScore.ToString();
    }
}