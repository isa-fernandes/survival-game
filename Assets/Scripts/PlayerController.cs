using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject particle;
    [SerializeField] GameObject particleAttackAll;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject lifeBar;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI instructionsText;
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    [SerializeField] float speed = 1;
    [SerializeField] float rotationSpeed = 100;
    [SerializeField] float jumpSpeed = 10;
    [SerializeField] float maxLifeScaleX = 4;

    public bool isAlive = true;

    // Private variables
    GameManager gameManagerScript;
    Animator playerAnimator;
    Rigidbody playerRb;
    Vector3 initialPosition;
    Quaternion initialRotation;
    bool isOnGround = true;
    bool isSliding;
    bool hasKey;
    bool isCharging = false;
    float currentLife = 100;

    // Start is called before the first frame update
    void Start()
    {
        lifeBar.transform.localScale = new Vector3(maxLifeScaleX, 1.5f, 1f);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
        gameManagerScript = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive && !isCharging)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);

            verticalInput = Input.GetAxis("Vertical");
            transform.Translate(Vector3.forward * verticalInput * speed * Time.deltaTime);
            playerAnimator.SetFloat("Speed", verticalInput);

            if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
            {
                playerAnimator.SetTrigger("Jump");
                playerRb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                isOnGround = false;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(Attack());
            }

            if (Input.GetKeyDown(KeyCode.M) && gameManagerScript.GetTime() < 30)
            {
                StartCoroutine(AttackAll());
            }

        }
        

    }

    void UpdateLife(bool isReset = false)
    {
        if (isReset)
        {
            currentLife = 100;
        } else if (currentLife-20 >= 0)
        {
            currentLife -= 20;
        }
        lifeBar.transform.localScale = new Vector3((currentLife*maxLifeScaleX) / 100, 1.5f, 1f);
    }

    void ResetGame ()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        isAlive = true;

        playerAnimator.SetBool("Dead", false);

        UpdateLife(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Animator>().SetBool("Attack", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Weapon") && !isCharging)
        {
            StartCoroutine(Hurt());
        }

        if (other.gameObject.CompareTag("WinPlatform"))
        {
            StartCoroutine(Win());
        }

        if (other.gameObject.CompareTag("Key"))
        {
            instructionsText.text = "Try to enter the church's door";
            hasKey = true;
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Enemy") && isSliding)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            StartCoroutine(enemyController.Die());
        }

        if (collision.gameObject.CompareTag("LockedCollider") && hasKey)
        {
            Destroy(collision.gameObject);
        }
    }

    IEnumerator Win ()
    {
        isAlive = false;
        playerAnimator.SetTrigger("Win");
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
        yield return new WaitForSeconds(5);
        gameOverPanel.SetActive(true);
        gameOverText.SetText("VICTORY!");
    }

    IEnumerator Hurt ()
    {
        UpdateLife();
        
        if (currentLife == 0)
        {
            isAlive = false;
            playerAnimator.SetBool("Dead", true);

            // Wait 3s then restart game
            yield return new WaitForSeconds(3);
            ResetGame();

        } else
        {
            playerAnimator.SetTrigger("Damage");
        }
        
    }

    IEnumerator Attack ()
    {
        isSliding = true;
        playerAnimator.SetTrigger("Slide");
        particle.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        particle.SetActive(false);
        isSliding = false;
    }

    IEnumerator AttackAll ()
    {
        isCharging = true;
        particleAttackAll.SetActive(true);
        playerAnimator.SetTrigger("AttackAll");
        yield return new WaitForSeconds(3);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            StartCoroutine(enemy.GetComponent<EnemyController>().Die());
        }
        gameManagerScript.SetTime(gameManagerScript.GetTime()/2);
        particleAttackAll.SetActive(false);
        isCharging = false;
    }

    public void Loose ()
    {
        playerAnimator.SetTrigger("Loose");
        isAlive = false;
    }
}
