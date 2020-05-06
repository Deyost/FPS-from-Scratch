using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 100f;
    [SerializeField] float rotationYSpeed = 100f;
    [SerializeField] float rotationXSpeed = 100f;
    [SerializeField] bool inverseCamera = false;
    [SerializeField] FootstepSoundGenerator soundGenerator;
    [SerializeField] bool isWalking = false;
    [SerializeField] float footstepTimer;
    [SerializeField] bool isJumping = false;
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float jumpTimer = 5.0f;

    private Rigidbody rb;
    private float rotateY, rotateX;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        soundGenerator = GetComponent<FootstepSoundGenerator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        PlayerRotationSide();
        PlayerRotationUpDown();
        Jump();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerVelocity = new Vector3(h, 0, v);
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerVelocity *= 2 * speed * Time.deltaTime;
            if (playerVelocity.x < 0 || playerVelocity.x > 0 || playerVelocity.y < 0 || playerVelocity.y > 0 || playerVelocity.z < 0 || playerVelocity.z > 0)
            {
                if (!isWalking && !isJumping)
                {
                    PlayFootSoundRun();
                }
            }
        }

        else
        {
            playerVelocity *= speed * Time.deltaTime;
            if (playerVelocity.x < 0 || playerVelocity.x > 0 || playerVelocity.y < 0 || playerVelocity.y > 0 || playerVelocity.z < 0 || playerVelocity.z > 0)
            {
                if (!isWalking && !isJumping)
                {
                    PlayFootSoundWalk();
                }
            }
        }

        playerVelocity = transform.TransformDirection(playerVelocity);


        var velocityChange = playerVelocity - rb.velocity;
        velocityChange.y = 0;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        if (isJumping)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine("JumpTask", jumpTimer);
    }

    IEnumerator JumpTask(float timer)
    {
        rb.velocity = new Vector3(0, jumpForce, 0);
        isJumping = true;
        yield return new WaitForSeconds(timer);
        isJumping = false;
    }

    void PlayFootSoundWalk()
    {
        StartCoroutine("PlayStepSound", footstepTimer);
    }

    void PlayFootSoundRun()
    {
        StartCoroutine("PlayStepSound", 0.4 * footstepTimer);
    }

    IEnumerator PlayStepSound(float timer)
    {
        var randomIndex = Random.Range(0, 5);
        soundGenerator.audioSource.clip = soundGenerator.footstepSounds[randomIndex];
        soundGenerator.audioSource.Play();

        isWalking = true;
        yield return new WaitForSeconds(timer);
        isWalking = false;
    }


    void PlayerRotationSide()
    {
        rotateY = Input.GetAxis("Mouse X") * rotationYSpeed * Time.deltaTime;

        transform.Rotate(0, rotateY, 0);
    }

    void PlayerRotationUpDown()
    {
        if(inverseCamera)
            rotateX += Input.GetAxis("Mouse Y") * rotationXSpeed * Time.deltaTime;

        else
            rotateX += -Input.GetAxis("Mouse Y") * rotationXSpeed * Time.deltaTime;

        float clampedRotationX = Mathf.Clamp(rotateX, -90, 90);
        float clampedRotationZ = Mathf.Clamp(rotateX, 0, 0);

        transform.rotation = Quaternion.Euler(clampedRotationX, transform.eulerAngles.y, clampedRotationZ);
    }
}
