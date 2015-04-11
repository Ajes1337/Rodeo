using System.Collections;
using UnityEngine;

public class TempPlayerController : MonoBehaviour {
    private float speed = 20f;
    private Rigidbody rigidbodyHer;
    public Missile MissileFab;
    private bool mouseIsLocked = true;

    // Use this for initialization
    private void Start() {
        rigidbodyHer = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update() {

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Debug.Log("spawn missile");
            Instantiate(MissileFab, transform.position + transform.forward * 2, Quaternion.LookRotation(transform.forward));
        }


        if (Input.GetKeyDown(KeyCode.Escape)) {
            mouseIsLocked = !mouseIsLocked;
        }

        if (mouseIsLocked) {
            Screen.lockCursor = true;
            Cursor.visible = false;
        }
        else {
            Screen.lockCursor = false;
            Cursor.visible = true;
        }

        float yaxis = 0;
        if (Input.GetKey(KeyCode.Space)) {
            yaxis += 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift)) {
            yaxis += -1f;
        }

        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), yaxis, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= speed;

        float maxVelocityChange = 10.0f;
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

        GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);


        /* if (Input.GetKey(KeyCode.W)) {
             rigidbodyHer.AddForce();
             transform.Translate(0, 0, speed * Time.fixedDeltaTime);
         }
         if (Input.GetKey(KeyCode.S)) {
             transform.Translate(0, 0, -speed * Time.fixedDeltaTime);
         }
         if (Input.GetKey(KeyCode.A)) {
             transform.Translate(-speed * Time.fixedDeltaTime, 0, 0);
         }
         if (Input.GetKey(KeyCode.D)) {
             transform.Translate(speed * Time.fixedDeltaTime, 0, 0);
         }

         if (Input.GetKey(KeyCode.Space)) {
             transform.Translate(0, speed * Time.fixedDeltaTime, 0);
         }
         if (Input.GetKey(KeyCode.LeftShift)) {
             transform.Translate(0, -speed * Time.fixedDeltaTime, 0);
         }*/
    }

    private void FixedUpdate() {
    }
}