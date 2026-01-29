using UnityEngine;

public class DoorManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Material DoorOpened;

    public Material DoorClosed;
    bool State = false;
    public AudioClip[] DoorSounds;

    public bool IsExitdoor = false;

    public GameObject OtherSideDoor;
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material = DoorClosed;

        OtherSideDoor.GetComponent<Renderer>().material = DoorClosed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        if (!State)
        {
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = DoorOpened;
            OtherSideDoor.GetComponent<Renderer>().material = DoorOpened;
            State = true;
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = DoorSounds[Random.Range(0, DoorSounds.Length)];
            audio.Play();
            Collider collider = GetComponent<Collider>();
            collider.enabled = false;
            if (IsExitdoor)
            {
                Gamemanager.Instance.UpdateRoomNumber(Gamemanager.Instance.CurrentRoomNumber + 1);
            }

        }
    }
}
