using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanceHitCollider : MonoBehaviour {

    public float hitForce = 10f;
    public float torqueForce = 10f;
    private Vector2 direction;
    private Vector2 collidingDirection;
    private Vector2 lastPosition;
    public Transform lance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collidedObject in colliders)
        {
            Rigidbody2D RB = collidedObject.GetComponent<Rigidbody2D>();
            if (RB != null)
            {
                direction = (RB.position - (Vector2)lance.position).normalized;
                RB.AddForce(direction * hitForce, ForceMode2D.Impulse);
                RB.AddTorque(Vector2.SignedAngle(direction, Vector2.right) * -torqueForce, ForceMode2D.Impulse);
            }
        }
        
        if (collision.GetComponent<IKillable>() != null)
        {
            collision.GetComponent<IKillable>().Kill();
        }
    }
}
