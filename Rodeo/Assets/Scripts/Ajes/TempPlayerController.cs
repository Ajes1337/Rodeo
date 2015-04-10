using UnityEngine;
using System.Collections;

public class TempPlayerController : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {



        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(0, 0, 10 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(0, 0, -10 * Time.deltaTime);

        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(-10 * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(10 * Time.deltaTime, 0, 0);

        }





    }
}
