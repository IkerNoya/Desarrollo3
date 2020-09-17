using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image map;
    [SerializeField] Transform target;
    [SerializeField] float offset;

    Camera cam;
    float distanceToGoal;
    void Start()
    {
        PlayerController.endGame += EndGame;
        map.fillAmount = 0f;
        cam = Camera.main;
        if(target!=null)
            distanceToGoal = target.position.y - cam.transform.position.y - offset;
    }
    void Update()
    {
        if (target != null)
        {
            float currentCameraDistance = target.position.y - cam.transform.position.y - offset;
            float progress = 1f - (currentCameraDistance / distanceToGoal);
            if (cam.transform.position.y >= target.position.y - offset)
            {
                map.fillAmount = 1f;
            }
            map.fillAmount = progress;
        }
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
