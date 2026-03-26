using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Prefab")]
    public GameObject treePrefab;      

    [Header("spawnPoint")]
    public Transform spawnPoint;      

    [Header("Dissapear(z-axis)")]
    public float destroyZ = -20f;      

    [Header("TreeMoveSpeed")]
    public float moveSpeed = 10f;

    [Header("spawnInterval")]
    public float spawnInterval = 0.5f; 

    private float timer = 0f;

    void Update()
    {
        // control spawn time
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTree();
            timer = 0f;
        }
    }

    void SpawnTree()
    {
        GameObject tree = Instantiate(treePrefab, spawnPoint.position, spawnPoint.rotation);
        tree.AddComponent<TreeMover>().Init(moveSpeed, destroyZ);
    }
}

public class TreeMover : MonoBehaviour
{
    private float moveSpeed;
    private float destroyZ;

    public void Init(float speed, float destroyPosZ)
    {
        moveSpeed = speed;
        destroyZ = destroyPosZ;
    }

    void Update()
    {
        // back move by z axis
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        // over point destroy
        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }
}
