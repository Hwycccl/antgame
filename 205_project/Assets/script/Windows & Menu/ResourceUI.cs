using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    public TMP_Text leafText;
    public TMP_Text foodText;
    public TMP_Text trashText;

    private int leaf = 0;
    private int food = 0;
    private int trash = 0;

    public void UpdateResources(int newLeaf, int newFood, int newTrash)
    {
        leaf = newLeaf;
        food = newFood;
        trash = newTrash;

        leafText.text = "Leaf: " + leaf;
        foodText.text = "Food: " + food;
        trashText.text = "Gabage: " + trash;
    }
}

