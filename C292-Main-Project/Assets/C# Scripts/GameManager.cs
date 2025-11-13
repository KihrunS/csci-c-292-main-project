using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public float respawnTime;

    string starKey = "StarCount";
    string dashKey = "MaxDashCount";

    public int CurrentStarCount { get; set; }
    public int MaxDashCount { get; set; }



    [SerializeField] private GameObject playerPrefab;
    private GameObject spawnPosition;
    private GameObject playerInstance;
    private Player playerScript;

    private bool dead = false;

    private void Awake()
    {
        if (FindObjectOfType<GameManager>() == null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayerPrefs.DeleteAll();
        }

        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");
        CurrentStarCount = PlayerPrefs.GetInt(starKey);
        MaxDashCount = PlayerPrefs.GetInt(dashKey) + 1;
        
    }

    private void Start()
    {
        Invoke("SpawnPlayer", 0.1f);
    }

    public void IncrementStarCount()
   {
        PlayerPrefs.SetInt(starKey, CurrentStarCount + 1);
   }

    public void UpdateMaxDashCount()
    {
        PlayerPrefs.SetInt(dashKey, MaxDashCount + 1);
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
        playerScript.SetDashCount(MaxDashCount);
    }

    public bool CanRefresh()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (playerScript.GetDashCount() < MaxDashCount)
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        else
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
    }

    // debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(PlayerPrefs.GetInt(starKey));
        }
    }
}
