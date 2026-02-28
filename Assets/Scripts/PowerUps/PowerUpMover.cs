using UnityEngine;

public class PowerUpMover : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float destroyX = -12f;

    private void Update()
    {
        float world = (GameManager.Instance != null) ? GameManager.Instance.worldSpeed : 1f;
        transform.position += Vector3.left * (baseSpeed * world * Time.deltaTime);

        if (transform.position.x <= destroyX)
            Destroy(gameObject);
    }
}
