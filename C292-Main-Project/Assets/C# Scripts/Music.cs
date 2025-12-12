using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("Music"); // Singleton pattern that destroys the old instance
        if (list.Length > 1)
        {
            Destroy(list[0]);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
