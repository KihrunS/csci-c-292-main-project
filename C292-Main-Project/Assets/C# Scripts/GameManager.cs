using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float respawnTime;
    [SerializeField] private float uiTimer;

    [SerializeField] private GameObject playerPrefab;
    private GameObject spawnPosition;
    private GameObject playerInstance;
    private Player playerScript;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RectTransform timerBackground;

    private bool dead = false;
    private bool levelComplete = false;
    public int starCount;
    [SerializeField] public int maxDashCount;
    [SerializeField] private double timeSinceStart;
    [SerializeField] private bool timerActive = false;
    private string hours;
    private string minutes;
    private string seconds;
    private int room = 0;

    private void Awake()
    {
        GameManager[] list = FindObjectsOfType<GameManager>(); // Singleton pattern, destroys new game manager if one exists and calls the initialization method in the existing game manager
        if (list.Length > 1)
        {
            list[0].InitializeScene();
            Destroy(gameObject);
            return;
        }

        maxDashCount = 1;
        InitializeScene();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (timerActive) // So timer can be disabled at the end or when pausing, if added
        {
            timeSinceStart += Time.deltaTime;
            hours = (((int)timeSinceStart / 3600).ToString().Length == 1) ? "0" + ((int)timeSinceStart / 3600).ToString() : ((int)timeSinceStart / 3600).ToString(); // each of these check for 1 digit, and adds a 0 in front if yes
            minutes = (((int)timeSinceStart / 60 % 60).ToString().Length == 1) ? "0" + ((int)timeSinceStart / 60 % 60).ToString() : ((int)timeSinceStart / 60 % 60).ToString();
            seconds = (((int)timeSinceStart % 60).ToString().Length == 1) ? "0" + ((int)timeSinceStart % 60).ToString() : ((int)timeSinceStart % 60).ToString();


            if (timerText != null) // failsafe in case if update runs before the timer game object is found
            {
                timerText.text = hours + " : " + minutes + " : " + seconds;
            }
        }

        if (timeSinceStart >= 359999f)
        {
            timerActive = false;
        }

        // debug
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(starCount);
        }
    }

    public void InitializeScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1 && room != 1)
        {
            starCount = 0;
            maxDashCount = 1;
            timeSinceStart = 0;
            timerActive = true; // sets timer to 0 and starts it upon loading the first level for the first time
        }

        if (SceneManager.GetActiveScene().buildIndex != room) // only runs on first time entering this room
        {
            timerText = GameObject.FindGameObjectWithTag("Timer").GetComponent<TextMeshProUGUI>();
            timerBackground = timerText.rectTransform.parent.GetComponent<RectTransform>();
            spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");
            Invoke("SpawnPlayer", 0.1f);
            StartCoroutine("ShowUI");
            levelComplete = false;
            room = SceneManager.GetActiveScene().buildIndex;
        }
        else // runs on respawn
        {
            SpawnPlayer();
            StartCoroutine("ShowUI");

        }
        
    }

    public void IncrementStarCount()
   {
        starCount++;
    }

    public void UpdateMaxDashCount()
    {
        maxDashCount = 2;
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerScript.UpdateDashCount();
    }

    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, spawnPosition.transform.position, Quaternion.identity);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        InitializeScene();
        dead = false;
    }

    private IEnumerator ShowUI() // shows timer, waits, hides timer
    {
        timerBackground.gameObject.SetActive(true);
        yield return new WaitForSeconds(uiTimer);
        timerBackground.gameObject.SetActive(false);
    }

    public void KillPlayer()
    {
        if (!dead)
        {
            dead = true;
            playerInstance = GameObject.FindGameObjectWithTag("Player");
            Destroy(playerInstance);
            StartCoroutine("Respawn");
        }
    }

    public void DashRefresh()
    {
        playerScript.SetDashCount(maxDashCount);
    }

    public bool CanRefresh()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (playerScript.GetDashCount() < maxDashCount)
        {
            return true;
        }
        return false;
    }

    public void SpringJump()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerScript.SpringJump();
    }

    public bool BlockBreak()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (playerScript.isDashing)
        {
            playerScript.Knockback();
            return true;
        }
        return false;

    }

    public void NextLevel()
    {
        if (!levelComplete) // Prevents the player from hitting 2 triggers at once
        {
            levelComplete = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public bool SpikeDirection(string dir)
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (dir == "up")
        {
            if (playerScript.GetYVelocity() <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (dir == "down")
        {
            if (playerScript.GetYVelocity() >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (dir == "right")
        {
            if (playerScript.GetXVelocity() <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (playerScript.GetXVelocity() >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void EndGame()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerScript.EndGame();
    }
}
