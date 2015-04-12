using System.Collections;
using UnityEngine;

public class BlueScreenDeath : MonoBehaviour
{
    public Texture2D BlueScreenTex;

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BlueScreenTex);
        if (GUI.Button(new Rect(Screen.width - 60, 30, 30, 30), "X"))
        {
            Application.LoadLevel("TerrainTest");
        }
    }

    // Use this for initialization
    private void Start()
    {
        Screen.lockCursor = false;
        Cursor.visible = true;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}