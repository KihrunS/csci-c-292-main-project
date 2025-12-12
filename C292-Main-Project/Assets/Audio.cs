using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{

    private void Awake()
    {
        AudioSource[] list = FindObjectsOfType<AudioSource>(); // Singleton pattern, destroys new game manager if one exists and calls the initialization method in the existing game manager
        if (list.Length > 1)
        {
            Destroy(gameObject);
            return;
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
