using UnityEngine;
using TMPro;

public class DescriptionManager : MonoBehaviour
{
    public static DescriptionManager Instance { get; private set; }

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject descriptionBox;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HideDescription();
    }

    public void ShowDescription(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        descriptionBox.SetActive(true);
    }

    public void HideDescription()
    {
        descriptionBox.SetActive(false);
    }
}
