using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int EntityId = 0;
    float progressMovement = 0f;
    int CurrentPathIndex = 0;
    Vector3 SpawnPos;
    Vector3 StartPos;
    Vector3 EndPos;
    float speed = 125f;
    
    void Start()
    {
        transform.position = SpawnPos;
        if (Gamemanager.Instance.CurrentRoomNumber >= 40)
        {
            speed = 250f;
        }
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        yield return new WaitForSeconds(1f);
        foreach (var room in Gamemanager.Instance.Rooms)
        {
            progressMovement = 0f;
            StartPos = room.Find("Entrance").position;
            EndPos = room.Find("Exit").position;
            while (progressMovement < 1f && CurrentPathIndex < Gamemanager.Instance.CurrentRoomNumber + 5 && CurrentPathIndex > Gamemanager.Instance.CurrentRoomNumber - 10)
            {
                progressMovement += Time.deltaTime * speed / Vector3.Distance(StartPos, EndPos);
                transform.position = Vector3.Lerp(StartPos, EndPos, progressMovement);
                yield return null;
            }
            CurrentPathIndex++;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 dirToPlayer = (Gamemanager.Instance.PM.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, dirToPlayer , out hit, 65f))
        {
            if (hit.collider.GetComponent<PlayerManager>() && !Gamemanager.Instance.PM.isHidden)
            {
                Debug.Log("Entity " + EntityId.ToString() + " caught the player!");
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
