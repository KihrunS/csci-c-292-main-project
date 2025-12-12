using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    [SerializeField] GameObject starPrefab;
    [SerializeField] AudioClip keyGrabSound;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        audioSource = FindObjectOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(keyGrabSound, .25f);
            GameObject.Instantiate(starPrefab, transform.parent.position, Quaternion.identity);
            spriteRenderer.enabled = false;
            Destroy(this.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }
}
