using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    static public int seed = 0;
    public System.Random rng;
    public RoomType[] RoomTypes;
    public RoomType CurrentRoomType;
    public int RoomsToGenerate = 100;
    private List<int> RoomsWhenToSwitchRoomTypes;
    public Transform CurrentRoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
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
        for (int i = 0; i < RoomsToGenerate; i++)
        {
            if (RoomsWhenToSwitchRoomTypes.Contains(i))
            {
                CurrentRoomType = GetNextRoomType();
            }
            Room NextRoom = GetNextRoom(CurrentRoomType);
            Transform NextRoomObject = Instantiate(NextRoom.RoomObject);

            NextRoomObject.position = CurrentRoom.Find("Exit").position;

            CurrentRoom = NextRoomObject;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}

[System.Serializable]
public class Room
{
    public Transform RoomObject;
    public Transform Entrance;
    public Transform Exit;
    public int Weight = 100;



}
[System.Serializable]
public class RoomType
{
    public string Name;
    public Room[] Rooms;
    public int Weight;
}
