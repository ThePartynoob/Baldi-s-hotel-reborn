using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InputActionReference MoveAction;

    [SerializeField] private InputActionReference InteractAction;

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField]
    [Range(0,1)]
    private float acceleration = 0.1f;

    public float mouseSensitivity = 2.0f;
    private Vector2 Velocity = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveAction.action.Enable();
        InteractAction.action.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 MoveVelocity = MoveAction.action.ReadValue<Vector2>();
        Vector2 DeltaCursorMove = Mouse.current.delta.ReadValue();
        CharacterController rb = GetComponent<CharacterController>();
        Velocity = Vector2.Lerp(Velocity, MoveVelocity, acceleration);

        Vector3 move = new Vector3(Velocity.x, 0, Velocity.y) * moveSpeed * Time.deltaTime;
        transform.Rotate(0, DeltaCursorMove.x * mouseSensitivity, 0);
        rb.Move(transform.TransformDirection(move));
        if (InteractAction.action.WasPressedThisFrame())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
            {
                DoorManager door = hit.collider.GetComponent<DoorManager>();
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }

    }
}
