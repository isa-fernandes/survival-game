using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemyCorners;
    [SerializeField] GameObject key;
    [SerializeField] GameObject keyCorners;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] int enemiesAtStart = 3;
    [SerializeField] float timer = 90f;

    // Private variables
    List<Transform> enemyCornerSpots;
    List<Transform> keyCornerSpots;
    PlayerController playerController;
    int enemyCount;
    float spawnRate = 10;

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

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isAlive)
        {
            SetTime(Time.deltaTime);
        }
        
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
            spawnRate -= 5;
        }

        StartCoroutine(enemyCoroutine());
    }

    IEnumerator GameOver ()
    {
        playerController.Loose();
        yield return new WaitForSeconds(4);
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void SetTime (float subtract)
    {
        if (timer - subtract > 0)
        {
            timer -= subtract;
            
        } else
        {
            timer = 0;
        }

        timerText.SetText(Mathf.RoundToInt(timer).ToString());

        if (timer < 30)
        {
            timerText.color = Color.red;
        }
        if (Mathf.RoundToInt(timer) <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    public float GetTime ()
    {
        return timer;
    }
}
