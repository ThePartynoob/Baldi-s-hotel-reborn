using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InputActionReference MoveAction;

    [SerializeField] private InputActionReference InteractAction;

    [SerializeField] private InputActionReference SprintAction;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 5f;

    public Slider staminabar;

    private float stamina = 100f;

    private bool isSprinting = false;

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
        SprintAction.action.Enable();
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
        staminabar.value = stamina;
        
        if (SprintAction.action.IsPressed() && stamina > 0)
        {
            isSprinting = true;
            stamina = Mathf.Clamp(stamina - 20f * Time.deltaTime, 0, 100f);
        }
        else
        {
            isSprinting = false;
            stamina = Mathf.Clamp(stamina + 10f * Time.deltaTime, 0, 100f);
        }
        stamina = Mathf.Clamp(stamina, 0, 100f);

        Vector2 MoveVelocity = MoveAction.action.ReadValue<Vector2>();
        Vector2 DeltaCursorMove = Mouse.current.delta.ReadValue();
        CharacterController rb = GetComponent<CharacterController>();
        Velocity = Vector2.Lerp(Velocity, MoveVelocity, acceleration);
        var speed = isSprinting ? runSpeed : moveSpeed;
        Vector3 move = new Vector3(Velocity.x, 0, Velocity.y) * speed * Time.deltaTime;
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
