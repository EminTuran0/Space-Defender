using UnityEngine;

public class LostWhale : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider){
        if (collider.gameObject.CompareTag("Player")){
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LevelComplete();
            }
        }
    }
}