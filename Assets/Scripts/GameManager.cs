using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        PlayerController.playerIsDead += EndGame;
    }
    void Update()
    {
        
    }
    void EndGame()
    {
        SceneManager.LoadScene("End");
    }
    private void OnDisable()
    {
        PlayerController.playerIsDead -= EndGame;
    }
}
