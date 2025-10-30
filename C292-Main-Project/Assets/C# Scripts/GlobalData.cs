using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float respawnTime;

    string starKey = "StarCount";
    string dashKey = "MaxDashCount";

    public int CurrentStarCount { get; set; }
    public int MaxDashCount { get; set; }



    [SerializeField] private GameObject player;
    private GameObject spawnPosition;

    private void Awake()
    {
        spawnPosition = GameObject.FindGameObjectWithTag("SpawnPosition");

        CurrentStarCount = PlayerPrefs.GetInt(starKey);
        MaxDashCount = PlayerPrefs.GetInt(dashKey) + 1;
        
    }

    private void Start()
    {
        Invoke("SpawnPlayer", 0.1f);
    }

    public void SetStarCount(int starCount)
   {
        PlayerPrefs.SetInt(starKey, starCount);
   }

    public void UpdateMaxDashCount()
    {
        PlayerPrefs.SetInt(dashKey, MaxDashCount + 1);
    }

    public void SpawnPlayer()
    {
        StartCoroutine("Respawn");
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        Instantiate(player, spawnPosition.transform.position, Quaternion.identity);
    }
}
