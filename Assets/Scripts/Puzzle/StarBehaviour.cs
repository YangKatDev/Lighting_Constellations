using UnityEngine;

public class StarBehaviour : MonoBehaviour
{

    public bool isOn = false;
    public int gridIndex;

    private SpriteRenderer spriteRenderer;

    // sets the color and avoids a random bug of Unity having Alpha as 0
    private Color offColor = Color.black;
    private Color onColor = Color.white;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }

    void OnMouseDown()
    {
        if (ConstellationManager.Instance.isLevelComplete)
        {
            return;
        }
        ConstellationManager.Instance.OnStarClicked(gridIndex);
    }

    public void Toggle()
    {
        isOn = !isOn;
        UpdateVisuals();

        // Tells manager if constellation is solved
        ConstellationManager.Instance.CheckSolution();
    }

    public void SetState(bool state)
    {
        isOn = state;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (isOn)
        {
            spriteRenderer.color = onColor;
        }
        else
        {
            spriteRenderer.color = offColor;
        }
    }
}
