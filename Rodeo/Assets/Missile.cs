using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {

    private Rigidbody daRigidbody;
    // Use this for initialization
    void Start() {
        daRigidbody = GetComponent<Rigidbody>();
        daRigidbody.AddForce(transform.forward * 5, ForceMode.VelocityChange);

    }

    // Update is called once per frame
    void Update() {



    }

    void FixedUpdate() {

        if (daRigidbody.velocity.magnitude < 25) {
            daRigidbody.AddForce(transform.forward * 10);
        }

    }

    private void OnCollisionEnter(Collision collision) {

        Destroy(this.gameObject);



    }


}
