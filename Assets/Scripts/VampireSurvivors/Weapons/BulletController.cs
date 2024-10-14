using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    private SpriteRenderer sptRend;

    private void Start()
    {
        sptRend = GetComponent<SpriteRenderer>();
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
