using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMechanics : MonoBehaviour {

    public List<Transform> Tail = new List<Transform>();

    [Header("Things to setup in editor")]
    public GameObject tailPrefab;
    public GameObject normalFoodPrefab;
    public GameObject specialFoodPrefab;

    // to check food positions not collide each other
    private GameObject normalFoodSpawned;
    private GameObject specialFoodSpawned;

    [Header("Snake speed settings")]
    [Range(0.1f, 1f)]
    public float snakeSpeed = 0.2f;

    [Header("Time range before spawning special food and blink parameters")]
    public float minTimeToSpawnSpecialFood = 5f;
    public float maxTimeToSpawnSpecialFood = 10f;
    public float specialFoodExistTime = 5f;
    private float specialFoodAlreadyExistTime = 0f;
    public float specialFoodWhenStartBlinking = 2.5f;
    public float specialFoodBlinkFrequency = 0.2f;


    [Header("Map size to have range for spawning food")]
    public int xMapSizeMin = -5;
    public int xMapSizeMax = 4;
    public int yMapSize = 7;


    bool isTailFullySpawned = false;
    bool isSpecialFoodBlinking = false;


    void Start () {
        InvokeRepeating("MovementUpdate", snakeSpeed, snakeSpeed);
        StartCoroutine(WaitForTailSpawn());

        SpawnNormalFood();
        StartCoroutine(SpawnSpecialFood());
	}

    void Update()
    {
        // to avoid useless using CPU, checking if special food is spawned and haven't reached 
        // exist time, if so adding past time, if not just ignoring
        if (specialFoodAlreadyExistTime <= specialFoodExistTime && specialFoodSpawned)
        {
            specialFoodAlreadyExistTime += Time.deltaTime;
        }

        // checking if special food already exist time has reached the blink time limit
        // to avoid multiple coroutines using bool isSpecialFoodBlinking
        if (specialFoodAlreadyExistTime >= (specialFoodExistTime - specialFoodWhenStartBlinking) && !isSpecialFoodBlinking && specialFoodSpawned)
        {
            StartSpecialFoodBlinking();
        }
        // if player didn't pick special food, destroying and ending blinking coroutine
        else if (specialFoodAlreadyExistTime >= 5f && specialFoodSpawned)
        {
            StopSpecialFoodBlinking();
            DestroySpecialFood();
        }

    }



    #region Bug Fixes

    /// <summary>
    /// To avoid tail collision on start.
    /// </summary>
    IEnumerator WaitForTailSpawn()
    {
        yield return new WaitForSeconds(Tail.Count * snakeSpeed);
        isTailFullySpawned = true;
    }

    #endregion

    #region Snake

    void MovementUpdate()
    {
        MoveSnake();
    }

    // vector used for snake's tail
    Vector3 lastPos;

    /// <summary>
    /// Makes snake moving
    /// </summary>
    void MoveSnake()
    {
        // saving last head's position for
        lastPos = transform.position;

        GUIButtons.currentDirection = GUIButtons.nextDirection;

        // clear last saved vector
        Vector3 nextPos = Vector3.zero;

        // setting snake's direction 
        if (GUIButtons.nextDirection == GUIButtons.Direction.up)
            nextPos = Vector3.up;
        else if (GUIButtons.nextDirection == GUIButtons.Direction.down)
            nextPos = Vector3.down;
        else if (GUIButtons.nextDirection == GUIButtons.Direction.left)
            nextPos = Vector3.left;
        else if (GUIButtons.nextDirection == GUIButtons.Direction.right)
            nextPos = Vector3.right;



        // checking if next move will be last made alive
        RaycastHit2D hit = Physics2D.Raycast(transform.position + 0.5f * nextPos, nextPos, 1f);
        if (hit.collider != null) 
        {   
            // if so set game to end and return from function
            if (hit.collider.tag == "Wall" || (hit.collider.tag == "SnakeTail" && isTailFullySpawned))
            {
                GameOver();
                return;
            }
        }

        // if move isn't last made alive, move snake's head and tail
        transform.position += nextPos;
        MoveTail();

    }



    /// <summary>
    /// Makes tail follow the head
    /// </summary>
    void MoveTail()
    {
        // looping throught Tail list to move every snake's tail piece
        for (int i = 0; i < Tail.Count; i++)
        {
            Vector3 temp = Tail[i].position;
            Tail[i].position = lastPos;
            lastPos = temp;
        }
    }

    /// <summary>
    /// Extending the tail
    /// </summary>
    void AddTailPiece()
    {
        Tail.Add(Instantiate(tailPrefab, Tail[Tail.Count - 1].position, Quaternion.identity).transform);

    }

    #endregion

    #region Food

    // variables for food position
    int xFoodPos;
    int yFoodPos;
    Vector3 foodPos;

    /// <summary>
    /// Draws a random number, between scene size
    /// </summary>
    void ChooseRandomFoodPosition()
    {
        xFoodPos = Random.Range(xMapSizeMin, xMapSizeMax);
        yFoodPos = Random.Range(-yMapSize, yMapSize);

        foodPos = new Vector3(xFoodPos, yFoodPos, 0);

        for (int i = 0; i < Tail.Count; i++)
        {
            // Check if food position collide with snake or special food not collide with normal food, if so repeat
            if ((foodPos == Tail[i].position) || foodPos == transform.position || (normalFoodSpawned != null && foodPos == normalFoodSpawned.transform.position) || (specialFoodSpawned != null && foodPos == specialFoodSpawned.transform.position))
            {
                ChooseRandomFoodPosition();
                return;
            }
        }

    }

    #region Normal food
    /// <summary>
    /// Spawning normal food at random position, which not collide with snake
    /// </summary>
    void SpawnNormalFood()
    {
        ChooseRandomFoodPosition();
        // Spawning normal food at random position
        normalFoodSpawned = Instantiate(normalFoodPrefab, foodPos, Quaternion.identity);
    }
    #endregion

    #region Special food

    /// <summary>
    /// Spawning special food at random position after random time, which not collide with snake
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnSpecialFood()
    {
        // draws random number to wait before spawning special food
        float timeToWait = Random.Range(minTimeToSpawnSpecialFood, maxTimeToSpawnSpecialFood);

        yield return new WaitForSeconds(timeToWait);

        ChooseRandomFoodPosition();
        // Spawning special food at random position
        specialFoodSpawned = Instantiate(specialFoodPrefab, foodPos, Quaternion.identity);

        // reseting exist time of special food
        specialFoodAlreadyExistTime = 0f;
    }

    /// <summary>
    /// Destroying special food and starts next special food spawn coroutine
    /// </summary>
    void DestroySpecialFood()
    {
        Destroy(specialFoodSpawned);
        specialFoodSpawned = null;
        StartCoroutine(SpawnSpecialFood());
    }


    #region Special food blinking

    /// <summary>
    /// Changing special food alpha from 0 to 1 in a period of time
    /// </summary>
    IEnumerator SpecialFoodBlink()
    {
        while (true && specialFoodSpawned)
        {
            switch (specialFoodSpawned.GetComponent<Renderer>().material.color.a.ToString())
            {
                case "0":
                    specialFoodSpawned.GetComponent<Renderer>().material.color = new Color(specialFoodSpawned.GetComponent<Renderer>().material.color.r, specialFoodSpawned.GetComponent<Renderer>().material.color.g, specialFoodSpawned.GetComponent<Renderer>().material.color.b, 1);
                    yield return new WaitForSeconds(specialFoodBlinkFrequency);
                    break;
                case "1":
                    specialFoodSpawned.GetComponent<Renderer>().material.color = new Color(specialFoodSpawned.GetComponent<Renderer>().material.color.r, specialFoodSpawned.GetComponent<Renderer>().material.color.g, specialFoodSpawned.GetComponent<Renderer>().material.color.b, 0);
                    yield return new WaitForSeconds(specialFoodBlinkFrequency);
                    break;
            }
        }
    }
    

    /// <summary>
    /// Stopping previous coroutine (if was) and starting a new one and setting
    /// isSpecialFoodBlinking to true
    /// </summary>
    void StartSpecialFoodBlinking()
    {
        StopCoroutine("StartSpecialFoodBlinking");      
        StartCoroutine("SpecialFoodBlink");
        isSpecialFoodBlinking = true;
    }

    /// <summary>
    /// Stopping blinking coroutine and setting isSpecialFoodBlinking to false
    /// </summary>
    void StopSpecialFoodBlinking()
    {        
        StopCoroutine("StartBlinking");
        isSpecialFoodBlinking = false;
    }

    #endregion



    #endregion

    #endregion

    #region Collisions and others

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "NormalFood")
        {
            AddTailPiece();

            //Add point to score
            GUIButtons.score++;

            //Destroy normal food
            Destroy(collision.gameObject);

            SpawnNormalFood();
        }
        else if(collision.tag == "SpecialFood")
        {
            AddTailPiece();

            //Add point to score
            GUIButtons.score += 10;

            DestroySpecialFood();
        }
    }



    /// <summary>
    /// Setting snake's live to false, coloring to red and cancel invoke "MovementUpdate"
    /// </summary>
    void GameOver()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;

        // looping throught every tail piece and changing color to red
        for (int i = 0; i < Tail.Count; i++)
        {
            Tail[i].GetComponent<Renderer>().material.color = Color.red;
        }

        // setting snake's live to false
        GUIButtons.isSnakeAlive = false;
        // stopping snake
        CancelInvoke("MovementUpdate");

        Time.timeScale = 0;
    }


    #endregion


}
