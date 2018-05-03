using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingAttack : MonoBehaviour, IKillable {

    public float attackTime = 3f;
    private float nextAttackTime = 5;
    public float hitForce = 1;
    public float hitStunTime = 0.1f;
    private bool attacking = false;
    private bool alreadyHit = false;

    public float offsetX = 5;
    public float offsetY = 2;
    private int directionMultiplier;
    private float dotProductResult;

    public Transform player;
    private Vector2 originalPosition;
    private Vector2 direction;
    private Vector2 targetPos;

    public GameObject deathEffect;

    void FixedUpdate()
    {
        if (!attacking)
            transform.position = Vector2.Lerp(transform.position, targetPos, 2f * Time.deltaTime);
    }

    void Update()
    {
        targetPos = player.position + new Vector3(-offsetX * directionMultiplier, offsetY, 0);
        direction = (player.position - transform.position).normalized;
        dotProductResult = Vector2.Dot(direction, Vector2.right);

        AttackTimer();
        CalculateDirection();
    }

    void CalculateDirection()
    {
        if (!attacking)
        {
            if (dotProductResult > 0)
            {
                directionMultiplier = 1;
            }
            else
            {
                directionMultiplier = -1;
            }
        }
    }

    public void Kill()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (attacking)
        {
            if (collision.gameObject.tag == "Player" && !alreadyHit)
            {
                Rigidbody2D hitRB = collision.GetComponent<Rigidbody2D>();

                if (hitRB != null)
                {
                    alreadyHit = true;
                    hitRB.AddForce(Vector2.right * directionMultiplier * hitForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    void AttackTimer()
    {
        if(Time.time > nextAttackTime)
        {
            nextAttackTime = Time.time + Random.Range(5,8);
            StartCoroutine(DiveAttack());
        }
    }

    IEnumerator DiveAttack()
    {
        alreadyHit = false;
        attacking = true;
        bool hitStunHappened = false;

        float startTime = Time.time;
        float endTime = startTime + attackTime;

        Vector3 origoPosition = player.transform.position;
        // Jannen koodia
        while (Time.time < endTime)
        {

            //Hitstun (Tuomaksen)
            if (alreadyHit && !hitStunHappened)
            {
                Time.timeScale = 1/20f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                yield return new WaitForSeconds(hitStunTime);
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02f;
                hitStunHappened = true;
            }

            float currentTime = Time.time - startTime;
            float percent = currentTime / attackTime;

            float currentOffsetX = offsetX * 2 * percent - offsetX;

            float currentOffsetY = 2f / 25f * Mathf.Pow(currentOffsetX, 2);

            if (directionMultiplier == -1)
            {
                currentOffsetX = -currentOffsetX;
            }

            Vector3 offset = new Vector3(currentOffsetX, currentOffsetY, 0);

            transform.position = origoPosition + offset;
            yield return null;
        }
        //Jannen koodi loppuu
        attacking = false;
    }
}
