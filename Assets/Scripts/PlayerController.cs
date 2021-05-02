using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject particle;
    [SerializeField] TextMeshProUGUI lifesText;
    [SerializeField] TextMeshProUGUI instructionsText;
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;
    [SerializeField] float speed = 1;
    [SerializeField] float rotationSpeed = 150;
    [SerializeField] float jumpSpeed = 10;

    // Private variables
    Animator playerAnimator;
    Rigidbody playerRb;
    Vector3 initialPosition;
    Quaternion initialRotation;
    bool isOnGround = true;
    bool isSliding;
    bool isAlive = true;
    int lifes = 3;
    bool hasKey;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            transform.Translate(Vector3.right * horizontalInput * speed * Time.deltaTime);
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

        }
        

    }

    void UpdateLife(bool isReset = false)
    {
        if (isReset)
        {
            lifes = 3;
        } else if (lifes > 0)
        {
            lifes--;
        }
        lifesText.text = lifes.ToString();
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
        if (other.gameObject.CompareTag("Weapon"))
        {
            StartCoroutine(Hurt());
        }

        if (other.gameObject.CompareTag("WinPlatform"))
        {
            isAlive = false;
            instructionsText.text = "WIN!";
            playerAnimator.SetTrigger("Win");
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject gameObject in gameObjects)
            {
                Destroy(gameObject);
            }
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

    IEnumerator Hurt ()
    {
        UpdateLife();
        
        if (lifes == 0)
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
}
