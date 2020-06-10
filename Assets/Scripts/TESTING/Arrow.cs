using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public Vector3 direction = Vector3.one;

    public float speed;

    bool finished = false;

    void Update()
    {
        Destroy(this.gameObject);
        if (!finished)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            Invoke("Die", 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor")
        {
            finished = true;
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }
}
