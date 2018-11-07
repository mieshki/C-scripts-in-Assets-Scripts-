using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIButtons : MonoBehaviour {

    public Button leftButton;
    public Button rightButton;
    public Button upButton;
    public Button downButton;

    public Text scoreText;
    public static int score = 0;

    public static bool isSnakeAlive = true;

    public GameObject gameOverScreen;

    public enum Direction
    {
        up,
        down,
        left,
        right
    }


    // made 2 variables to fix reverse bug, before fix u were able to
    // click fast twice direction button and snake was reversing into himself
    public static Direction currentDirection;
    public static Direction nextDirection;

    void Start () {
        // add on click listeners to the buttons
        Button leftBtn = leftButton.GetComponent<Button>();
        leftBtn.onClick.AddListener(TurnLeft);

        Button rightBtn = rightButton.GetComponent<Button>();
        rightBtn.onClick.AddListener(TurnRight);

        Button upBtn = upButton.GetComponent<Button>();
        upBtn.onClick.AddListener(TurnUp);

        Button downBtn = downButton.GetComponent<Button>();
        downBtn.onClick.AddListener(TurnDown);
    }
	
    /// <summary>
    /// Change snake's direction to left
    /// </summary>
    void TurnLeft()
    {       
        if (currentDirection != Direction.right)
            nextDirection = Direction.left;
    }

    /// <summary>
    /// Change snake's direction to right
    /// </summary>
    void TurnRight()
    {   
        if (currentDirection != Direction.left)
            nextDirection = Direction.right;
    }

    void TurnUp()
    {
        if (currentDirection != Direction.down)
            nextDirection = Direction.up;
    }

    void TurnDown()
    {
        if (currentDirection != Direction.up)
            nextDirection = Direction.down;
    }

    void Update()
    {
        // updating gui score text
        scoreText.text = "Score:" + score;

        // shows gameover screen if you lost
        if (isSnakeAlive)
            gameOverScreen.SetActive(false);
        else if (!isSnakeAlive)
            gameOverScreen.SetActive(true);
    }
}
