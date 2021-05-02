using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemyCorners;
    [SerializeField] GameObject key;
    [SerializeField] GameObject keyCorners;
    [SerializeField] int enemiesAtStart = 3;

    // Private variables
    List<Transform> enemyCornerSpots;
    List<Transform> keyCornerSpots;
    int enemyCount;
    float spawnRate = 20;

    // Start is called before the first frame update
    void Start()
    {
        // Get possible enemy corners
        enemyCornerSpots = new List<Transform>();
        enemyCorners.gameObject.GetComponentsInChildren(enemyCornerSpots);

        // Remove main game object to only keep the children inside
        enemyCornerSpots.RemoveAt(0);
      
        for(int i = 0; i < enemiesAtStart; i++)
        {
            SpawnEnemies();
        }

        StartCoroutine(enemyCoroutine());

        // Get possible key corners
        keyCornerSpots = new List<Transform>();
        keyCorners.gameObject.GetComponentsInChildren(keyCornerSpots);

        // Remove main game object to only keep the children inside
        keyCornerSpots.RemoveAt(0);

        SpawnKey();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CheckIsPositionEmpty (Transform transform)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.transform.position == transform.position)
            {
                // Is not empty
                return false;
            }
        }

        // Is empty
        return true;
    }

    void SpawnKey()
    {
        int index = Random.Range(0, keyCornerSpots.Count);

        Instantiate(key, keyCornerSpots[index]);
    }

    void SpawnEnemies ()
    {
        int index = -1;
        do
        {
            int randomIndex = Random.Range(0, enemyCornerSpots.Count);

            if (CheckIsPositionEmpty(enemyCornerSpots[randomIndex]))
            {
                index = randomIndex;
            }

        } while (index == -1);
        
        
        Instantiate(enemy, enemyCornerSpots[index]);

        enemyCount++;
        
    }

    IEnumerator enemyCoroutine ()
    {
        yield return new WaitForSeconds(spawnRate);

        SpawnEnemies();

        if (spawnRate > 5)
        {
            spawnRate -= 2;
        }

        StartCoroutine(enemyCoroutine());
    }
}
