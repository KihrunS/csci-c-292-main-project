using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float respawnTime;

    [SerializeField] private GameObject playerPrefab;
    private GameObject spawnPosition;
    private GameObject playerInstance;
    private Player playerScript;

    private bool dead = false;
    private bool levelComplete = false;
    public int starCount;
    [SerializeField] public int maxDashCount;

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

    public void InitializeScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            starCount = 0;
            maxDashCount = 1;
        }

        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");
        Invoke("SpawnPlayer", 0.1f);
        levelComplete = false;
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
        Instantiate(playerPrefab, spawnPosition.transform.position, Quaternion.identity);
        dead = false;
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

    // debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(starCount);
        }
    }
}
