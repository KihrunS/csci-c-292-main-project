using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

    SpriteRenderer spriteRenderer;

    [SerializeField] GameObject starPrefab;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject.Instantiate(starPrefab, transform.parent.position, Quaternion.identity);
            spriteRenderer.enabled = false;
            Destroy(this.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }
}
