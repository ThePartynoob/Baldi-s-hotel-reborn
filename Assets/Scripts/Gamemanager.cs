using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;
using RenderSettings = UnityEngine.RenderSettings;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance { get; private set; }
    [SerializeField]
    static public int seed = 0;
    public int seedOverride = 0;
    public bool OverrideSeed = false;
    public System.Random rng;
    public RoomType[] RoomTypes;
    public RoomType CurrentRoomType;
    public int CurrentRoomNumber { get; protected set; } = 0;
    public int RoomsToGenerate = 100;
    public Material blackskybox;
    [SerializeField]
    private List<int> RoomsWhenToSwitchRoomTypes;
    public Transform CurrentRoom;
    public List<Transform> Rooms;
    public int FirstEntitySpawnAt { get; protected set; } = -1;
    public AudioSource Song;
    public AudioSource Ambiance;
    public AudioReverbFilter reverb;
    public Light globalLight;
    bool[] debugs = { false };
    public GameObject HideText;
    public GameObject[] EntityPrefabs;

    bool debugmode = false;
    public PlayerManager PM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnGUI()
    {
        if (debugmode)  {
            GUILayout.Label("CurRoom: " + CurrentRoomNumber.ToString() + "\n Startat: " + FirstEntitySpawnAt.ToString());
            if (GUILayout.Button("Reload scene"))
            {
                SceneManager.LoadScene("Game");
            }
            if (GUILayout.Button("Inf stamina"))
            {
                debugs[0] = !debugs[0];

            }
        }
    }

    void UpdateDebugs()
    {
        if (debugs[0])
        {
            PM.stamina = 100f;
        }
    }
    private void Awake()
    {
        Instance = this;

        

        seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        if (OverrideSeed)
        {
            seed = seedOverride;
        }
        rng = new System.Random(seed);


        CurrentRoomType = GetNextRoomType();
        RoomsWhenToSwitchRoomTypes = new List<int>();
        for (int i = 0; i < rng.Next(1,5); i++)
        {
            RoomsWhenToSwitchRoomTypes.Add(rng.Next(10, RoomsToGenerate-5));
        }
    }
    void Start()
    {
        Rooms = new List<Transform>();
        for (int i = 0; i < RoomsToGenerate; i++)
        {
            if (RoomsWhenToSwitchRoomTypes.Contains(i))
            {
                CurrentRoomType = GetNextRoomType();
            }
            
            Room NextRoom = GetNextRoom(CurrentRoomType);
            if (FirstEntitySpawnAt == -1)
            {
                if (NextRoom.HasHidingSpot && i >= 5)
                {
                    FirstEntitySpawnAt = i + 1;
                }
            }
            Transform NextRoomObject = Instantiate(NextRoom.RoomObject);
            Rooms.Add(NextRoomObject);

            NextRoomObject.position = CurrentRoom.Find("Exit").position;

            CurrentRoom = NextRoomObject;
        }


        foreach (var item in Rooms)
        {
            Debug.DrawLine(item.Find("Entrance").position, item.Find("Exit").position, Color.red,Int32.MaxValue);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebugs();
    }

    IEnumerator Event(int door)
    {
        yield return null;

        if (door == FirstEntitySpawnAt)
        {
            while (Song.volume > 0)
            {
                if (Song.pitch >= 0.05f)
                {

                    Song.pitch = Mathf.Lerp(Song.pitch, 0, 0.007f);
                    RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.2f, 0.01f);
                    RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, 100, 0.005f);
                    RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, 20, 0.005f);
                    globalLight.shadowStrength = Mathf.Lerp(globalLight.shadowStrength, 0, 0.01f);
                    globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.05f, 0.01f);
                    RenderSettings.skybox = blackskybox;
                    
                } else
                {
                    
                    Song.volume = 0;
                }
                yield return null;
            }
            HideText.SetActive(true);
            Instantiate(GetRandomEntity());
            reverb.reverbPreset = AudioReverbPreset.SewerPipe;
            Ambiance.Play();
            yield return new WaitForSeconds(5f);
            HideText.SetActive(false);
        }

        if (door > FirstEntitySpawnAt + 4) {
            if (rng.Next(0, 100) < 20)
            {
                yield return new WaitForSeconds(rng.Next(0,20)/10f);
                Instantiate(GetRandomEntity());
            }
        }
        if (door == RoomsToGenerate - 1)
        {
            SceneManager.LoadScene("Win");
        }
    }


    public void UpdateRoomNumber(int number)
    {
        CurrentRoomNumber = number;

        StartCoroutine(Event(CurrentRoomNumber));

    }

    

    RoomType GetNextRoomType()
    {
        int totalWeight = 0;
        foreach (RoomType roomType in RoomTypes)
        {
            totalWeight += roomType.Weight;
        }
        int randomValue = rng.Next(0, totalWeight);
        int cumulativeWeight = 0;
        foreach (RoomType roomType in RoomTypes)
        {
            cumulativeWeight += roomType.Weight;
            if (randomValue < cumulativeWeight)
            {
                return roomType;
            }
        }
        return null; 

    }

    Room GetNextRoom(RoomType roomType)
    {
        int totalWeight = 0;
        foreach (Room room in roomType.Rooms)
        {
            totalWeight += room.Weight;
        }
        int randomValue = rng.Next(0, totalWeight);
        int cumulativeWeight = 0;
        foreach (Room room in roomType.Rooms)
        {
            cumulativeWeight += room.Weight;
            if (randomValue < cumulativeWeight)
            {
                return room;
            }
        }
        return null;
    }

    GameObject GetRandomEntity()
    {
        int index = rng.Next(0, EntityPrefabs.Length);
        return EntityPrefabs[index];
    }
}

[System.Serializable]
public class Room
{
    public Transform RoomObject;
    public Transform Entrance;
    public Transform Exit;
    public int Weight = 100;
    public bool HasHidingSpot = false;



}
[System.Serializable]
public class RoomType
{
    public string Name;
    public Room[] Rooms;
    public int Weight;
}

