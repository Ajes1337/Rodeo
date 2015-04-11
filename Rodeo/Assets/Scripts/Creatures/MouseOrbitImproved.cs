using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
    public Transform target;
    private float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    private float yMinLimit = -90f;
    private float yMaxLimit = 90f;
    private float distanceMin = 1.5f;
    private float distanceMax = 1000f;
    private float x = 0.0f;
    private float y = 0.0f;
    public bool CameraDoCollide = false;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    private void LateUpdate()
    {
        if (target && Input.GetMouseButton(2))
        {
            x += Input.GetAxis("Mouse X") * xSpeed /** distance */* 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            if (CameraDoCollide)
            {
                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    distance = hit.distance;
                }
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;
            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            if (CameraDoCollide)
            {
                RaycastHit hit;
                if (Physics.Linecast(target.position, transform.position, out hit))
                {
                    distance = hit.distance;
                }
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;
            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}