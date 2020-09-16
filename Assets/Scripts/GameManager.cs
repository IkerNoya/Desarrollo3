using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image map_L;
    [SerializeField] Image map_R;
    [SerializeField] Transform target;
    [SerializeField] float offset;

    Camera cam;
    float distanceToGoal;
    void Start()
    {
        PlayerController.endGame += EndGame;
        map_L.fillAmount = 0f;
        map_R.fillAmount = 0f;
        cam = Camera.main;
        distanceToGoal = target.position.y - cam.transform.position.y - offset;
    }
    void Update()
    {
        float currentCameraDistance = target.position.y - cam.transform.position.y - offset;
        float progress = 1f - (currentCameraDistance / distanceToGoal);
        if(cam.transform.position.y >= target.position.y - offset)
        {
            map_L.fillAmount = 1f;
            map_R.fillAmount = 1f;
        }
        map_L.fillAmount = progress;
        map_R.fillAmount = progress;
    }
    void EndGame()
    {
        SceneManager.LoadScene("End");
    }
    private void OnDisable()
    {
        PlayerController.endGame -= EndGame;
    }
}
