using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    [Header("Grid Dimensions")]
    public int rows = 4;
    public int cols = 5;

    [Header("Difficulty")]
    public int scrambleMoves = 4; // Number of clicks from solved state

    [Header("Grid State")]
    public bool[] initialGridState;

    [Header("Constellation Line Order")]
    public int[] lineOrder;

    public int GetIndex(int x, int y) => y * rows + x;

    private void Validate()
    {
        int totalSize = rows * cols;
        if (totalSize <= 0) return;

        if (initialGridState == null || initialGridState.Length != totalSize)
            System.Array.Resize(ref initialGridState, totalSize);
    }
}
