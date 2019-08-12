using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour {

    public int runAcceleration;
    public int jumpHeight;
    public int maxRunSpeed;
    bool grounded;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PlayerMovement();
	}

    void PlayerMovement()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 v3Velocity = rb.velocity;
        if (Input.GetKey("w"))
        {
            rb.AddForce(transform.forward * runAcceleration);
        }

        if (Input.GetKey("s"))
        {
            rb.AddForce((transform.forward * runAcceleration) * -1);
        }

        if (Input.GetKey(KeyCode.Space) && grounded) 
        {
            rb.AddForce(0, jumpHeight, 0);
        }

        if (Input.GetKey("d"))
        {
            rb.AddForce(transform.right * runAcceleration);
        }

        if (Input.GetKey("a"))
        {
            rb.AddForce((transform.right * runAcceleration) * -1);
        }

        Vector3 velocity = rb.velocity;
        if (velocity.magnitude >= maxRunSpeed)
        {
            velocity.Normalize();
            velocity *= maxRunSpeed;
            rb.velocity = velocity;
        }
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    void OnCollisionExit()
    {
        grounded = false;
    }
}
