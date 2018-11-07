using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    public Text yourScoreText;
    public Text bestScoreText;

    public Button restartGameButton;
    public Button goToMenuButton;

    void Start()
    {
        // adding on click listeners
        Button restartGameBtn = restartGameButton.GetComponent<Button>();
        restartGameBtn.onClick.AddListener(ReloadGameScene);

        Button goToMenuBtn = goToMenuButton.GetComponent<Button>();
        goToMenuBtn.onClick.AddListener(LoadMenuScene);
    }


    void Update()
    {
        // if game over screen is active, updating score
        if (!GUIButtons.isSnakeAlive)
            yourScoreText.text = "Your score:" + GUIButtons.score;
    }

    /// <summary>
    /// Reloading game scene and fixing variables
    /// </summary>
    void ReloadGameScene()
    {
        SceneManager.LoadScene(1);
        FixVariables();
    }

    /// <summary>
    /// Loading menu scene and fixing variables
    /// </summary>
    void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
        FixVariables();
    }


    /// <summary>
    /// Resetting variables
    /// </summary>
    void FixVariables()
    {        
        GUIButtons.isSnakeAlive = true;
        GUIButtons.nextDirection = GUIButtons.Direction.up;
        GUIButtons.score = 0;
        Time.timeScale = 1;
    }
}
