using UnityEngine;

public class tubaSpawn : MonoBehaviour
{
    public GameObject Tubacano;
    public float taxaSpawn = 2f;
    private float timer = 0f;
    public float heightOffset = 10f;

 
    public GameObject shellPrefab;
    [Range(0f, 1f)]
    public float shellSpawnChance = 0.25f; 
    
    public int spawnEveryN = 0;
    private int spawnCounter = 0;

    public int shellMaxHeal = 1;

    void Start()
    {
        SpawnTuba();
    }

    void Update()
    {
        if (timer < taxaSpawn)
            timer += Time.deltaTime;
        else
        {
            SpawnTuba();
            timer = 0f;
        }
    }

    void SpawnTuba()
    {
        float tubaMin = transform.position.y - heightOffset;
        float tubaMax = transform.position.y + heightOffset;

        float spawnY = Random.Range(tubaMin, tubaMax);
        Vector3 spawnPos = new Vector3(transform.position.x, spawnY, 0);

        GameObject tubaInstance = Instantiate(Tubacano, spawnPos, transform.rotation);

        bool shouldSpawnShell = false;
        spawnCounter++;

        if (spawnEveryN > 0)
        {
            if (spawnCounter % spawnEveryN == 0)
                shouldSpawnShell = true;
        }
        else
        {
            float r = Random.Range(0f, 1f);
            shouldSpawnShell = (shellPrefab != null) && (r <= shellSpawnChance);
        }

        if (shouldSpawnShell && shellPrefab != null)
        {
            if (tubaInstance.transform.Find(shellPrefab.name) != null)
            {
                return;
            }

            var scoreComp = tubaInstance.GetComponentInChildren<TubaPlusScore>();
            Vector3 shellWorldPos = (scoreComp != null) ? scoreComp.transform.position : spawnPos;

            GameObject shell = Instantiate(shellPrefab, shellWorldPos, Quaternion.identity, tubaInstance.transform);
            shell.name = shellPrefab.name;

            if (scoreComp != null)
                shell.transform.localPosition = tubaInstance.transform.InverseTransformPoint(scoreComp.transform.position);

            var collect = shell.GetComponent<ShellCollect>();
            if (collect != null)
            {
                collect.coinValue = shellMaxHeal;
            }
        }
    }
}