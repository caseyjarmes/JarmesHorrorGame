using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //variables for movement
    private Rigidbody rb;
    public int speed;
    public int strafeSpeed;
    public int retreatSpeed;
    public int rotateSpeed;
    public int runSpeed;

    //variables for flashlight
    public int flashlightTimer;
    public int flashlightRechargeTime;

    //for enemies
    public GameObject[] enemies;

    //variables for ui
    bool dead;
    public int keys;
    public int maxKeys;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI instructionsText;
    public Slider slider;

    private Animator anim;
    
    //gets the gate so it can be rotated when the kes are collected
    public Transform gate;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        dead = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement();
        flashlight();

        //updates the ui
        keyText.text = "Keys: " + keys + "/" + maxKeys;
        slider.value = flashlightTimer;
    }

    //flashlight stuns TD, has a cooldown
    void flashlight()
    {
        //for cooldown
        flashlightTimer++;

        //flashes if flashlight is recharged
        if(Input.GetKey(KeyCode.Space) && (flashlightTimer > flashlightRechargeTime))
        {
            //this sets the point light on the player on
            gameObject.transform.GetChild(19).gameObject.SetActive(true);

            //resets timer
            flashlightTimer = 0;

            //stuns TD
            foreach (GameObject td in enemies)
            {
                if (Vector3.Distance(td.transform.position, transform.position) < 10)
                {
                    EnemyScript TDScript = td.GetComponent<EnemyScript>();
                    td.GetComponent<EnemyScript>().state = State.Stunned;
                }
            }
        }

        //turns flashlight off again
        if (flashlightTimer == 50)
        {
            gameObject.transform.GetChild(19).gameObject.SetActive(false);
        }
    }

    //basic movement code/animation code
    void movement()
    {
        if (Input.GetKey("w") && Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = transform.forward * runSpeed * Time.deltaTime;
            anim.SetBool("IsSprinting", true);
        }
        else
        {
            anim.SetBool("IsSprinting", false);
        }

        if (Input.GetKey("w") && !Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity = transform.forward * speed * Time.deltaTime;
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

        if (Input.GetKey("s"))
        {
            rb.velocity -= transform.forward * retreatSpeed * Time.deltaTime;
            anim.SetBool("IsRetreating", true);
        }
        else
        {
            anim.SetBool("IsRetreating", false);
        }

        if (Input.GetKey("d"))
        {
            rb.velocity += transform.right * strafeSpeed * Time.deltaTime;
            anim.SetBool("StrafeRight", true);
        }
        else
        {
            anim.SetBool("StrafeRight", false);
        }

        if (Input.GetKey("a"))
        {
            rb.velocity -= transform.right * strafeSpeed * Time.deltaTime;
            anim.SetBool("StrafeLeft", true);
        }
        else
        {
            anim.SetBool("StrafeLeft", false);
        }

        float h = rotateSpeed * Input.GetAxis("Mouse X");
        transform.Rotate(0, h, 0);

        //if the player dies, this makes space reload the game
        if (dead && Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    //handles keys and enemys
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("key"))
        {
            //if touch key, destroy it and +1 key count
            Destroy(collision.gameObject);
            keys++;

            //if the last key
            if (keys >= maxKeys)
            {
                //tchanges text on bottom of the screen, tells player to run
                instructionsText.text = "The gate is open!";

                //opens gate
                gate.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
        }

        //runs gameover if an enemy touches the player
        if (collision.gameObject.CompareTag("enemy"))
        {
            gameOver();
        }
    }

    void gameOver()
    {
        //locks animation
        anim.SetBool("dead", true);

        //makes it so pressing space resets game
        dead = true;

        //keeps player from moving
        speed = 0;
        strafeSpeed = 0;
        retreatSpeed = 0;
        rotateSpeed = 0;
        runSpeed = 0;

        //turns on gameover text
        gameOverText.gameObject.SetActive(true);

        //tells player they can reset with space
        instructionsText.text = "Press Space to restart";
    }

    private void OnTriggerEnter(Collider other)
    {
        //code does the same stuff as gameover code, keeps player from moving and makes space reset scene
        dead = true;
        speed = 0;
        strafeSpeed = 0;
        retreatSpeed = 0;
        rotateSpeed = 0;
        runSpeed = 0;

        //turns on win text
        winText.gameObject.SetActive(true);
        instructionsText.text = "Press Space to restart";
    }
}
