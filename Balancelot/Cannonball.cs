using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour {

    public float hitForce = 1;
    public float explosionRadius = 2;
    public GameObject explosionEffectPrefab;

    //Same code used in LanceHitCollider script
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collidedObject in colliders)
        {
            Rigidbody2D collidedRB = collidedObject.GetComponent<Rigidbody2D>();
            if (collidedRB != null)
            {
                Vector2 direction = (collidedRB.position - (Vector2)transform.position).normalized;
                collidedRB.AddForce(direction * hitForce, ForceMode2D.Impulse);
            }
        }
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.Euler(90,0,0));
        Destroy(gameObject);
    }
}
