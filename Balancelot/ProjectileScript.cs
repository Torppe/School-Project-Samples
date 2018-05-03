using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public Transform target;

    public float attackSpeed = 1;
    public float attackRange = 5;
    private bool attacking = false;
    private float distanceToTarget;

    public float h = 10;
    private float gravity;

    private void Start()
    {
        gravity = Physics.gravity.y;
    }

    private void Update()
    {
        distanceToTarget = (target.position - transform.position).magnitude;

        if(!attacking && distanceToTarget <=  attackRange)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        attacking = true;
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Rigidbody2D projectileRB = projectile.GetComponent<Rigidbody2D>();

        projectileRB.velocity = CalculateLaunchVelocity(projectileRB);

        yield return new WaitForSeconds(attackSpeed);
        attacking = false;
    }
    Vector2 CalculateLaunchVelocity(Rigidbody2D rigidbody)
    {
        float displacementY = target.position.y - rigidbody.position.y;
        Vector2 displacementXY = new Vector2(target.position.x - rigidbody.position.x, target.position.y - rigidbody.position.y);

        Vector2 velocityY = Vector2.up * Mathf.Sqrt(-2 * gravity * h);
        Vector2 velocityXY = displacementXY / (Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity));

        return velocityXY + velocityY;
    }
}
