using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOnHit : MonoBehaviour
{
    public Rigidbody rb;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Player")
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
            rb.transform.parent = collision.gameObject.transform;
            if(collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<Animator>().StopPlayback();
            }
        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag !="Player")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<Animator>().StartPlayback();
            }
        }

    }
}
