// PERSONAL MODIFICATIONS:
// Teleport player to starter position if they fall off map
// Check total pickups to win in the pickup parent instead of hardcoding in the script
// Make levels
// Make the player restart if they fall off the level or collide with an enemy.


// TO DO:
// Level GUI. Maybe a speedrun timer.
// Respawn GUI
// Add more mechanics to the game as levels go on to make it more challenging; If player's velocity is low enough, they can jump? Might be a cool mechanic
// If below a certain velocity, the players velocity should be completely set to zero

//>--- Dependencies ---<\\
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //>--- Move the Character when given an inputvalue ---<\\
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    //>--- Sets the count UI text and win conditions ---<\\
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= winCount)
        {
            // Win Condition
            statusTextObject.SetActive(true);
            statusTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";
            GameObject.FindGameObjectWithTag("Enemy").SetActive(false);

            // Load next scene
            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings) {
                Debug.Log("Next scene loading");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else {
                // No next scene
                Debug.Log("There was no next scene, so I didn't load anything");
            }
        }
    }

    //>--- Required movement variables ---<\\
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    public float Speed = 0f;

    public GameObject pickUpAncestor;
    private Vector3 playerOriginalPosition;

    //>--- Required GUI variables ---<\\
    public GameObject Canvas;
    public TextMeshProUGUI countText;
    public GameObject statusTextObject;

    //>--- Required PickUp count variables ---<\\
    private int count;
    private int winCount;

    //>--- Required other variables ---<\\
    private int currentSceneIndex;

    void Start()
    {
        //>--- Do the following on script start ---<\\
        rb = GetComponent<Rigidbody>();
        count = 0; // Set pickup count to 0
        winCount = pickUpAncestor.transform.childCount; // Check how many pickups are required to win.
        SetCountText(); // Show current count on the GUI
        Canvas.SetActive(true); // Make sure GUI is visible
        statusTextObject.SetActive(false); // Make sure status text is NOT visible.

        // Check player start position. Also freezes the player so the start position is accurate
        rb.constraints = RigidbodyConstraints.FreezeAll;
        playerOriginalPosition = transform.position;
        rb.constraints = RigidbodyConstraints.None;

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    //>--- Run every physics tick ---<\\
    void FixedUpdate()
    { 
        Vector3 movement = new Vector3(movementX, 0, movementY); // Move based on input values
        rb.AddForce(movement * Speed); // Apply force to player

        if (transform.position.y <= -4) // If player falls off map, reset scene.
        {
            SceneManager.LoadScene(currentSceneIndex); // Load next scene
        }
    }

    //>--- Collsion with PickUps and Enemies ---<\\
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    //>--- Testing Keybinds for Scenes ---<\\
    void OnDevTestPgUp() {
        // Test scene up

        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings) {
            Debug.Log("Next scene loading");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            Debug.Log("There was no next scene, so I didn't load anything");
        }
    }

    void OnDevTestPgDown() {
        // Test scene down

        if (SceneManager.GetActiveScene().buildIndex - 1 >= 0) {
            Debug.Log("Previous scene loading");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else {
            Debug.Log("There was no Previous scene, so I didn't load anything: ");
        }
    }
}