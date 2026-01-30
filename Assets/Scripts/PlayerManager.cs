using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private InputActionReference MoveAction;

    [SerializeField] private InputActionReference InteractAction;

    [SerializeField] private InputActionReference SprintAction;

    public Camera PC;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 5f;

    public Slider staminabar;
    public Slider hidebar;
    public GameObject InteractText;
    public float stamina = 100f;

    private bool isSprinting = false;
    private bool isMoving = false;
    public bool isHidden { get; private set; } = false;
    [SerializeField]
    [Range(0, 1)]
    private float acceleration = 0.1f;
    private float timeelapsed = 0f;
    public float mouseSensitivity = 2.0f;
    public float HideTime = 10f;
    private Vector2 Velocity = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PC = Camera.main;
        MoveAction.action.Enable();
        InteractAction.action.Enable();
        SprintAction.action.Enable();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Hide(RaycastHit hit)
    {
        isHidden = true;
        PC.transform.rotation = hit.collider.transform.rotation;
        PC.transform.position = hit.collider.transform.position;

    }
    void Unhide()
    {
        isHidden = false;
        PC.transform.localPosition = new Vector3(0, 1f, 0);
        PC.transform.rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        staminabar.value = stamina;
        if (isHidden)
        {
            if (Gamemanager.Instance.CurrentRoomNumber >= 40)
            {
                HideTime -= Time.deltaTime * 2.5f;

            }
            else
            {
                HideTime -= Time.deltaTime;
            }
            if (HideTime <= 0f)
            {
                isHidden = false;
                Unhide();

            }
        }
        else
        {
            HideTime = Mathf.Clamp(HideTime + 0.35f * Time.deltaTime, 0f, 10f);
        }
        hidebar.value = HideTime;
        if (hidebar.value >= 10f)
        {
            hidebar.gameObject.SetActive(false);
        }
        else
        {
            hidebar.gameObject.SetActive(true);
        }
        if (SprintAction.action.IsPressed() && stamina > 0 && isMoving)
        {
            isSprinting = true;
            stamina = Mathf.Clamp(stamina - 4.5f * Time.deltaTime, 0, 100f);
        }
        else
        {
            isSprinting = false;

        }
        if (!isMoving)
        {
            stamina = Mathf.Clamp(stamina + 7.5f * Time.deltaTime, 0, 100f);
        }
        stamina = Mathf.Clamp(stamina, 0, 100f);

        Vector2 MoveVelocity = MoveAction.action.ReadValue<Vector2>();
        Vector2 DeltaCursorMove = Mouse.current.delta.ReadValue();
        CharacterController rb = GetComponent<CharacterController>();
        if (!isHidden)
        {
            Velocity = Vector2.Lerp(Velocity, MoveVelocity, acceleration);
        }
        if (Velocity.sqrMagnitude > 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        var speed = isSprinting ? runSpeed : moveSpeed;
        timeelapsed += (isSprinting ? 5f : isMoving ? 2.5f : 0.125f) * Time.deltaTime;
        Vector3 move = new Vector3(Velocity.x, 0, Velocity.y) * speed * Time.deltaTime;
        if (!isHidden)
        {
            PC.transform.localPosition = new Vector3(0, 1f + Mathf.Sin(timeelapsed * 2.5f) * 0.2f, 0);
            transform.Rotate(0, DeltaCursorMove.x * mouseSensitivity, 0);
        }

        rb.Move(transform.TransformDirection(move));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
        {
            if (InteractAction.action.WasPressedThisFrame())
            {
                DoorManager door = hit.collider.GetComponent<DoorManager>();
                if (door != null)
                {
                    door.OpenDoor();
                }

                if (hit.collider.CompareTag("Closet") && HideTime > 1.5f)
                {
                    isHidden = !isHidden;
                    Velocity = Vector2.zero;
                    if (isHidden)
                    {
                        Hide(hit);
                    }
                    else
                    {
                        Unhide();
                    }


                }
            }
            
    }


}
}
