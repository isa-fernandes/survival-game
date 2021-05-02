using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float speed = 1;
    [SerializeField] float minDistance = 0.5f;
    [SerializeField] float maxDistance = 3;

    // Private variables
    Animator enemyAnim;
    GameObject player;
    float initialSpeed;
    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        enemyAnim = GetComponentInChildren<Animator>();
        initialSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            transform.LookAt(player.transform);

            float distance = Vector3.Distance(player.gameObject.transform.position, gameObject.transform.position);


            if (distance >= minDistance && distance < maxDistance)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                speed += 0.1f * Time.deltaTime;
            }
            else
            {
                speed = initialSpeed;
            }

            if (distance <= minDistance)
            {
                Attack();
            }

            enemyAnim.SetFloat("Speed", speed);
        }
        
    }

    void Attack()
    {
        enemyAnim.SetBool("Attack", true);
    }

    public IEnumerator Die ()
    {
        isAlive = false;
        explosion.SetActive(true);
        enemyAnim.SetBool("Attack", false);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
