using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour, IKillable, IMovable {

    public Rigidbody2D rbPlayer;
    public Rigidbody2D rbWheel;

    public float jumpForce = 1000f;
    public bool onGround = false;
    public float landingAudioThreshold = 0;

    private bool dead = false;
    private bool jump = false;

    public float maxSpeed = 400f;
    public float rotationSpeed = 200f;

    private float currentSmoothVelocity;
    public float currentMotorSpeed = 0f;
    public float currentMovementSpeed;

    public WheelJoint2D wheel;

    public AudioScript playerAudioScript;
    private Damage playerDamage;
    private GroundCheck groundCheck;

    // Events
    public delegate void PlayerDeath();
    public static event PlayerDeath OnDeath;

    public bool Dead
    {
        get
        {
            return dead;
        }

        set
        {
            dead = value;
        }
    }

    void Start()
    {
        playerAudioScript = GetComponent<AudioScript>();

        playerDamage = gameObject.GetComponentInChildren<Damage>(); // Used to check possible damage on head.
        groundCheck = gameObject.GetComponentInChildren<GroundCheck>(); // Check for the ground from the wheel.
    }

    private void FixedUpdate()
    {
        if (StateManager.gameState == StateManager.GameState.Paused || StateManager.gameState == StateManager.GameState.GameOver)
        {
            wheel.useMotor = false;
        }
    }

    public void Kill()
    {
        Dead = playerDamage.Dead;

        if (OnDeath != null)
            OnDeath();
    }

    public void Jump()
    {
        jump = true;
    }

    public void Move(float p_movement, float p_rotation)
    {
        if (jump)
        {
            if (groundCheck.OnGround)
            {
                //Jumping Audio
                playerAudioScript.PlayAudioRandom(0);

                // The new Jump! 
                rbPlayer.velocity = new Vector2(rbPlayer.velocity.x, 4f);
                rbWheel.velocity = new Vector2(rbWheel.velocity.x, 4f);

                jump = false;
                groundCheck.OnGround = false;
            }
            else
            {
                jump = false;
            }
        }

        // Movement
        //SmoothDamp is used to make the movement transition smoothly between directions
        currentMovementSpeed = wheel.jointSpeed;
        currentMotorSpeed = Mathf.SmoothDamp(currentMovementSpeed, p_movement * maxSpeed, ref currentSmoothVelocity, 0.8f);
        currentMotorSpeed = Mathf.Clamp(currentMotorSpeed, -maxSpeed, maxSpeed);
        JointMotor2D motor = new JointMotor2D { motorSpeed = currentMotorSpeed, maxMotorTorque = 10000 };
        wheel.motor = motor;

        // Cycling Audio
        playerAudioScript.AudioModify(wheel.jointSpeed, maxSpeed, 1);

        if (p_movement == 0f)
        {
            wheel.useMotor = false;
        }
        else
        {
            wheel.useMotor = true;
        }

        rbPlayer.AddTorque(-p_rotation * rotationSpeed * Time.fixedDeltaTime * 10f);

    }
}
