using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public Button startGameButton;

	void Start ()
    {
        // adding on click listener
        Button startGameBtn = startGameButton.GetComponent<Button>();
        startGameBtn.onClick.AddListener(LoadGameScene);
    }

    /// <summary>
    /// loading game scene
    /// </summary>
    void LoadGameScene()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
