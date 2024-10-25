using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(RectTransform))]
public class MirroredButtonHandler : Mover, IPointerDownHandler
{
    [Header("Child Images")]
    public Image leftHalf;
    public Image rightHalf;
    public float scaleSize = 1.1f;
    public float distanceFromLeft = 50f;
    public float distanceFromBottom = 50f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color highlightedColor = Color.yellow;

    public Vector2 buttonSize = new Vector2(200f, 100f);

    TMP_Text text;

    RectTransform rectTransform;

    LevelDataHandler levelDataHandler;


    void Awake()
    {
         levelDataHandler = FindObjectOfType<LevelDataHandler>();
        // If not assigned in the Inspector, find the child images by name
        if (leftHalf == null)
            leftHalf = transform.Find("LeftHalf").GetComponent<Image>();
        if (rightHalf == null)
            rightHalf = transform.Find("RightHalf").GetComponent<Image>();

        rectTransform = GetComponent<RectTransform>();


        text = GetComponentInChildren<TMP_Text>();

        SetupRectTransform();

    }

    void Start()
    {
        string leveltext = levelDataHandler.levelData.level_number.ToString();
        SetButtonText(text, leveltext);

    }

    void SetButtonText(TMP_Text tmpText, string textToSet)
    {
        tmpText.text = textToSet;
    }

    void SetupRectTransform()
    {
        // Set anchors to bottom-left
        rectTransform.anchorMin = new Vector2(0.5f, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 0);

        // Set pivot to bottom-left
        rectTransform.pivot = new Vector2(0.5f, 0);



        // Set anchored position to desired pixel offsets
        rectTransform.anchoredPosition = new Vector2(0, distanceFromBottom);

        // Set the size of the button
        rectTransform.sizeDelta = buttonSize;
    }




    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {

        yield return StartCoroutine(CartoonishScaleToTarget(2, 1.1f, 1f));
        SceneManager.LoadScene("LevelScene");
    }
}
