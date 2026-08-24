using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float mouseSensitivity = 0.1f;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;

        yRotation = transform.eulerAngles.y;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput += value.Get<Vector2>();
    }

    private void Update()
    {
        // Handle mouse look
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        lookInput = Vector2.zero;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void FixedUpdate()
    {
        Quaternion bodyRotation = Quaternion.Euler(0f, yRotation, 0f);
        rb.MoveRotation(bodyRotation);

        Vector3 move = bodyRotation * new Vector3(moveInput.x, 0f, moveInput.y);
        move = Vector3.ClampMagnitude(move, 1f);

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}