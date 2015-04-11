using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
    public static Player Instance;
    private Rigidbody _rigidbody;
    private float _forceZ = 1500;
    private float _forceY = 1000;
    private float _forceX = 1000;
    private float _rotationYSpeed = 1;
    private float _rotationXSpeed = 1;
    private float _tiltSpeed = 30;
    private float _maxTiltZ = 60;
    public Missile MissileFab;
    private bool mouseIsLocked = true;
    private int selectedWeapon = 0;

    void OnGUI() {

        TerrainGen.AjesGuiLabel(new Rect(0, Screen.height - 160, 200, 20), "Weapons:");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 140, 200, 20), "Hole");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 120, 200, 20), "LightningRotateBall");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 100, 200, 20), "Portal");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 80, 200, 20), "RuneOfMagic");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 60, 200, 20), "Strom");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 40, 200, 20), "SummonMagicCircle2");
        TerrainGen.AjesGuiLabel(new Rect(20, Screen.height - 20, 200, 20), "SummonMagicCircle3");

        TerrainGen.AjesGuiLabel(new Rect(0, Screen.height - 140 + selectedWeapon * 20, 200, 20), "->");



    }

    private void Start() {
        this._rigidbody = this.GetComponent<Rigidbody>();
        Instance = this;
    }

    void Update() {

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

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectedWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            selectedWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            selectedWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            selectedWeapon = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            selectedWeapon = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)) {
            selectedWeapon = 6;
        }

        if (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.LeftControl)) {
            Missile mis = (Missile)Instantiate(MissileFab, transform.position + transform.forward * 2, Quaternion.LookRotation(transform.forward));
            mis.SetMissileType(selectedWeapon);
        }
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.W)) {
            this._rigidbody.AddForce(this.transform.forward * Time.fixedDeltaTime * _forceZ);
        }
        if (Input.GetKey(KeyCode.S)) {
            this._rigidbody.AddForce(this.transform.forward * -1 * Time.fixedDeltaTime * _forceZ);
        }
        if (Input.GetKey(KeyCode.A)) {
            this._rigidbody.AddForce(this.transform.TransformDirection(Vector3.left) * Time.fixedDeltaTime * _forceX);
        }
        if (Input.GetKey(KeyCode.D)) {
            this._rigidbody.AddForce(this.transform.TransformDirection(Vector3.right) * Time.fixedDeltaTime * _forceX);
        }
        if (Input.GetKey(KeyCode.Space)) {
            this._rigidbody.AddForce(this.transform.TransformDirection(Vector3.up) * Time.fixedDeltaTime * _forceY);
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            this._rigidbody.AddForce(this.transform.TransformDirection(Vector3.down) * Time.fixedDeltaTime * _forceY);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            this._rigidbody.AddTorque(this.transform.up * _rotationYSpeed * -1, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            this._rigidbody.AddTorque(this.transform.up * _rotationYSpeed, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            this._rigidbody.AddTorque(this.transform.right * _rotationYSpeed, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            this._rigidbody.AddTorque(this.transform.right * _rotationYSpeed * -1, ForceMode.Force);
        }

        if (Input.GetKey(KeyCode.Q)) {
            this._rigidbody.AddTorque(this.transform.forward * _rotationYSpeed * 0.2f, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.E)) {
            this._rigidbody.AddTorque(this.transform.forward * _rotationYSpeed * -1 * 0.2f, ForceMode.Force);
        }
        this._rigidbody.angularVelocity *= 1 - Time.fixedDeltaTime * 4;
    }
}