using UnityEngine;

public class CamController : MonoBehaviour
{
    public float dragSpeed;
    public float scrollSpeed;
    private float yaw;
    private float pitch;
    private float roll;
    private float radius;
    private Vector3 startRotation = new Vector3(0, 0, 0);
    private float startRadius = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        radius = startRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Escape to Unlock Mouse
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButton(0) && Cursor.lockState == CursorLockMode.None) //Click inside window to re-lock mouse if unlocked
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetCam();
        }
        
        TranslateCam();
    }

    private void TranslateCam()
    {
        if (Input.GetMouseButton(1)) // Right mouse drag
        {
            yaw += Input.GetAxis("Mouse X") * dragSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime;
        }

        radius -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        radius = Mathf.Clamp(radius, 0f, 9f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, roll);
        Vector3 offset = rotation * new Vector3(0, 0, radius);

        transform.position = offset;
        transform.LookAt(Vector3.zero, rotation * Vector3.up);
    }

    private void ResetCam()
    {
        yaw = 0.0f;
        pitch = 0.0f;
        radius = startRadius;
        Quaternion rotation = Quaternion.Euler(startRotation);
        transform.position = rotation * new Vector3(0, 0, startRadius);
    }
}
