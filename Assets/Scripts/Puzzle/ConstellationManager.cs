using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationManager : MonoBehaviour
{
    public static ConstellationManager Instance;

    [Header("Grid & Level Data")]
    public GameObject starPrefab;
    public float spacing = 1.5f;
    public List<LevelDataSO> levels;

    [Header("Animation & UI")]
    public float secondsPerLineSegment = 0.35f;
    public GameObject nextLevelButton;

    [HideInInspector]
    public bool isLevelComplete = false;

    // Stores the stars that get spawned automatically
    private StarBehaviour[] spawnedStars;

    // Creates a line automatically when you solve the puzzle
    private LineRenderer lineRenderer;

    private int currentLevelIndex;
    private LevelDataSO activeLevel;

    void Awake()
    {
        Instance = this;
        lineRenderer = (LineRenderer)GetComponent(typeof(LineRenderer));

        // this hides the line until player solves constellation puzzle
        lineRenderer.positionCount = 0;
    }

    void Start()
    {
        if (levels != null && levels.Count > 0)
        {
            int savedIndex = Mathf.Clamp(PlayerPrefs.GetInt("CurrentLevelIndex", 0), 0, levels.Count - 1);
            SetupLevel(savedIndex);
        }
        else
        {
            Debug.LogWarning("No levels assigned.");
        }
    }

    void RebuildGrid()
    {
        if (spawnedStars != null)
        {
            foreach (StarBehaviour star in spawnedStars)
                if (star != null) Destroy(star.gameObject);
        }

        int rows = activeLevel.rows;
        int cols = activeLevel.cols;

        // This will center the grid at the coords (0,0)
        Vector2 startOffset = new Vector2(-(cols - 1) * spacing / 2f, (rows - 1) * spacing / 2f);
        spawnedStars = new StarBehaviour[rows * cols];

        int i = 0;
        while (i != rows * cols)
        {
            int r = i / cols;
            int c = i % cols;

            Vector2 spawnPos = startOffset + new Vector2(c * spacing, -r * spacing);
            GameObject newObj = Instantiate(starPrefab, spawnPos, Quaternion.identity, transform);

            StarBehaviour node = (StarBehaviour)newObj.GetComponent(typeof(StarBehaviour));
            node.gridIndex = i;
            node.SetState(false); // Sets them all off at start (black)
            spawnedStars[i] = node;

            i++;
        }
    }

    void SetupLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        activeLevel = levels[currentLevelIndex]; // Takes the active Scriptable Object data for levels
        isLevelComplete = false;
        lineRenderer.positionCount = 0;

        if (nextLevelButton != null) nextLevelButton.SetActive(false);

        RebuildGrid();

        if (activeLevel.initialGridState.Length != spawnedStars.Length)
        {
            Debug.LogError($"Level '{activeLevel.name}' initialGridState size ({activeLevel.initialGridState.Length}) does not match rows * cols ({spawnedStars.Length})!");
            return;
        }

        // starts grid in exact solved state
        for (int i = 0; i < spawnedStars.Length; i++)
        {
            spawnedStars[i].gameObject.SetActive(true);
            spawnedStars[i].SetState(activeLevel.initialGridState[i]);
        }

        // Scramble
        List<int> usedIndices = new List<int>();
        int targetMoves = Mathf.Max(1, activeLevel.scrambleMoves);

        while (usedIndices.Count < targetMoves && usedIndices.Count < spawnedStars.Length)
        {
            int rand = Random.Range(0, spawnedStars.Length);
            if (!usedIndices.Contains(rand))
            {
                ToggleCrossPattern(rand);
                usedIndices.Add(rand);
            }
        }
    }

    public void OnStarClicked(int index)
    {
        if (isLevelComplete) return;
        ToggleCrossPattern(index);
        CheckSolution();
    }

    void ToggleCrossPattern(int index)
    {
        int cols = activeLevel.cols;
        int row = index / cols;
        int col = index % cols;

        // center star
        spawnedStars[index].Toggle();

        // Toggles neighbors around
        if (row > 0) spawnedStars[index - cols].Toggle();
        if (row < activeLevel.rows - 1) spawnedStars[index + cols].Toggle();
        if (col > 0) spawnedStars[index - 1].Toggle();
        if (col < cols - 1) spawnedStars[index + 1].Toggle();
    }

    //public void ShowSolutionPreview()
    //{
    //    for (int i=0; i < spawnedStars.Count; i++)
    //    {
    //        bool shouldbeOn = cancerSolution.Contains(i);
    //        spawnedStars[i].SetState(shouldbeOn);
    //    }
    //}

    public void CheckSolution()
    {
        if (isLevelComplete)
        {
            return;
        }

        for (int i = 0; i < spawnedStars.Length; i++)
            if (spawnedStars[i].isOn != activeLevel.initialGridState[i]) return;

        isLevelComplete = true;


        // Saves progress
        int nextIndex = (currentLevelIndex + 1) % levels.Count;
        PlayerPrefs.SetInt("CurrentLevelIndex", nextIndex);
        PlayerPrefs.SetInt("HasSaveData", nextIndex);
        PlayerPrefs.Save();

        StartCoroutine(AnimateConstellationLines());
    }

    IEnumerator AnimateConstellationLines()
    {
        // Hide all extra stars that are not apart of the constellation
        for (int i = 0; i < spawnedStars.Length; i++)
            if (!spawnedStars[i].isOn) spawnedStars[i].gameObject.SetActive(false);

        int[] order = activeLevel.lineOrder;
        if (order == null || order.Length == 0)
        {
            if (nextLevelButton != null) nextLevelButton.SetActive(true);
            yield break;
        }

        // Place starting point on first star
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, spawnedStars[order[0]].transform.position);

        for (int i = 1; i < order.Length; i++)
        {
            Vector3 startPos = spawnedStars[order[i - 1]].transform.position;
            Vector3 endPos = spawnedStars[order[i]].transform.position;

            // Add new line starting at startPosition
            lineRenderer.positionCount = i + 1;
            lineRenderer.SetPosition(i, startPos);

            float elapsed = 0f;
            while (elapsed < secondsPerLineSegment)
            {
                elapsed = Mathf.MoveTowards(elapsed, secondsPerLineSegment, Time.deltaTime);
                lineRenderer.SetPosition(i, Vector3.Lerp(startPos, endPos, elapsed / secondsPerLineSegment));
                yield return null;
            }
        }

        // Sets the next level button to active AFTER line is done animated-drawing
        if (nextLevelButton != null) nextLevelButton.SetActive(true);
    }

    public void OnNextLevelButtonClicked() => SetupLevel((currentLevelIndex + 1) % levels.Count);
}
