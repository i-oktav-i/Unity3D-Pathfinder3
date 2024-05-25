using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    float leftBound = 80f;
    float rightBound = 20f;

    public float speed = 10f;

    bool moveRight = true;

    void Start() { }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime * (moveRight ? 1 : -1)));

        if (transform.position.z > leftBound)
        {
            moveRight = false;
        }
        else if (transform.position.z < rightBound)
        {
            moveRight = true;
        }
    }
}
