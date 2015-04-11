using UnityEngine;
using System.Collections;

public class BlueScreenDeath : MonoBehaviour {

    public Texture2D BlueScreenTex;

    void OnGUI() {

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BlueScreenTex);

    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
