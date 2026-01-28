using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
public class UIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isLoading = false;
    public string[] tips;
    public string chosentip;
    AsyncOperation asyncLoad;
    void Start()
    {
        chosentip = tips[Random.Range(0, tips.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 3.5f)
        {
            timeElapsed = 0f;
            chosentip = tips[Random.Range(0, tips.Length)];
        }
        Panel.anchoredPosition = new Vector2(0, -(Camera.position.y - 5)*15 );
        if (isLoading) {
            LoadingText.text = "<color=green>Loading</color>\n" + chosentip + "\nProgress: " + (asyncLoad.progress * 100f).ToString() + "%";
        }
        tipText.text = chosentip;
    }

    public void PlayGame() {
        Panel.gameObject.SetActive(false);
        LoadingText.gameObject.SetActive(true);
        isLoading = true;
        asyncLoad = SceneManager.LoadSceneAsync("Game");
    }

    [SerializeField]
    private RectTransform Panel;
    [SerializeField]
    private TextMeshProUGUI LoadingText;
    [SerializeField]
    private Transform Camera;
    [SerializeField]
    private TextMeshProUGUI tipText;
    private float timeElapsed = 0f;
   
}
