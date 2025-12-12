using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingBlock : MonoBehaviour
{
    new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;

    [SerializeField] float disappearTime;
    [SerializeField] float respawnTime;

    [SerializeField] AudioClip disappearingSound;
    [SerializeField] AudioClip respawningSound;


    bool disappearing;

    // Start is called before the first frame update
    void Start()
    {
        collider = transform.parent.GetComponent<BoxCollider2D>();
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        disappearing = false;
        audioSource = GameObject.Find("Sounds").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!disappearing)
            {
                disappearing = true;
                StartCoroutine("Disappear");
            }
        }
    }

    IEnumerator Disappear()
    {
        audioSource.PlayOneShot(disappearingSound, .1f);
        yield return new WaitForSeconds(disappearTime);
        spriteRenderer.enabled = collider.enabled = false;
        yield return new WaitForSeconds (respawnTime);
        spriteRenderer.enabled = collider.enabled = true;
        disappearing = false;
        audioSource.PlayOneShot(respawningSound, .1f);

    }
}
