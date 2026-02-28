using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Spawn Area (World Space)")]
    [SerializeField] private Transform minPos;
    [SerializeField] private Transform maxPos;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] powerUpPrefabs;

    [Header("Timing")]
    [SerializeField] private float minInterval = 6f;
    [SerializeField] private float maxInterval = 12f;

    [Header("Spawn Options")]
    [Tooltip("Spawn Z will be forced to this spawner's Z (recommended for 2D).")]
    [SerializeField] private bool lockZToSpawner = true;

    [Tooltip("If spawned prefab has Rigidbody2D, set gravityScale=0 to prevent dropping too fast.")]
    [SerializeField] private bool disableRigidbody2DGravityOnSpawn = true;

    [Tooltip("Spawn as child of this spawner (usually OFF for gameplay objects).")]
    [SerializeField] private bool spawnAsChild = false;

    private float timer;

    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;
        if (minPos == null || maxPos == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        // güvenli aralık
        float a = Mathf.Max(0.05f, minInterval);
        float b = Mathf.Max(a, maxInterval);
        timer = Random.Range(a, b);
    }

    private void Spawn()
    {
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        if (prefab == null) return;

        // Min/Max ters olsa bile düzelt
        float minX = Mathf.Min(minPos.position.x, maxPos.position.x);
        float maxX = Mathf.Max(minPos.position.x, maxPos.position.x);

        float minY = Mathf.Min(minPos.position.y, maxPos.position.y);
        float maxY = Mathf.Max(minPos.position.y, maxPos.position.y);

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        float z = lockZToSpawner ? transform.position.z : prefab.transform.position.z;

        Vector3 spawnPos = new Vector3(x, y, z);

        GameObject go;
        if (spawnAsChild)
            go = Instantiate(prefab, spawnPos, Quaternion.identity, transform);
        else
            go = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (disableRigidbody2DGravityOnSpawn)
        {
            var rb2d = go.GetComponent<Rigidbody2D>();
            if (rb2d != null) rb2d.gravityScale = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (minPos == null || maxPos == null) return;

        float minX = Mathf.Min(minPos.position.x, maxPos.position.x);
        float maxX = Mathf.Max(minPos.position.x, maxPos.position.x);

        float minY = Mathf.Min(minPos.position.y, maxPos.position.y);
        float maxY = Mathf.Max(minPos.position.y, maxPos.position.y);

        Vector3 a = new Vector3(minX, minY, transform.position.z);
        Vector3 b = new Vector3(maxX, minY, transform.position.z);
        Vector3 c = new Vector3(maxX, maxY, transform.position.z);
        Vector3 d = new Vector3(minX, maxY, transform.position.z);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, c);
        Gizmos.DrawLine(c, d);
        Gizmos.DrawLine(d, a);
    }
}
