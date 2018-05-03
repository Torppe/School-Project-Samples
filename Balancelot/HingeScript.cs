using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeScript : MonoBehaviour {

    private HingeJoint2D hinge;
    private JointMotor2D motorSpeed;
    private Transform body;

    private float angleDifference;
    public float motorAngleLimits = 5f;
    public float k = 0.5f;

	void Start ()
    {
        hinge = GetComponent<HingeJoint2D>();
        body = GameObject.Find("Body").transform;
    }

    private void Update()
    {
        angleDifference = Vector2.SignedAngle(transform.right, (Vector2)body.right);
    }

    void FixedUpdate ()
    {
        motorSpeed = new JointMotor2D { motorSpeed = k * -angleDifference, maxMotorTorque = 5f};
        hinge.motor = motorSpeed;

        if (angleDifference <= motorAngleLimits && angleDifference >= -motorAngleLimits)
        {
            hinge.useMotor = false;
        }
        else
        {
            hinge.useMotor = true;
        }
    }
}
