using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class BagManager : MonoBehaviour
{
    [Header("아이템 UI 매핑")]
    [SerializeField] private List<Button> itemButtons;
    [SerializeField] private List<TextMeshProUGUI> itemButtonTexts;

    [Header("아이템 설명 텍스트 (선택사항)")]
    [SerializeField] private TextMeshProUGUI itemDescriptionText;

    private List<string> itemNames = new List<string>();
    private List<string> itemDescriptions = new List<string>();

    private int currentItemIndex = 0;

    void Awake()
    {
        itemNames.Add("상처약");
        itemDescriptions.Add("포켓몬 1마리의 HP를 20만큼 회복시킨다.");
    }

    void OnEnable()
    {
        currentItemIndex = 0;
        InitItemWindow();
    }

    private void InitItemWindow()
    {
        for (int i = 0; i < itemButtonTexts.Count; i++)
        {
            if (i < itemNames.Count)
            {
                itemButtons[i].gameObject.SetActive(true);
                itemButtonTexts[i].text = itemNames[i];
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }

        UpdateItemDisplay();
    }

    void Update()
    {
        if (itemNames.Count == 0) return;

        int previousIndex = currentItemIndex;

        GameObject hoveredUI = GetUIUnderMouse();
        if (hoveredUI != null)
        {
            Button hoveredButton = hoveredUI.GetComponentInParent<Button>();
            if (hoveredButton != null && itemButtons.Contains(hoveredButton))
            {
                int index = itemButtons.IndexOf(hoveredButton);
                if (index < itemNames.Count)
                {
                    currentItemIndex = index;
                }
            }
        }

        if (currentItemIndex != previousIndex)
        {
            UpdateItemDisplay();
        }

        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
        {
            UseCurrentItem();
        }
    }

    private void UpdateItemDisplay()
    {
        for (int i = 0; i < itemButtonTexts.Count; i++)
        {
            if (i >= itemNames.Count) continue;

            string baseName = itemNames[i];
            if (i == currentItemIndex)
            {
                itemButtonTexts[i].text = $"▶ {baseName}";
                if (itemDescriptionText != null) itemDescriptionText.text = itemDescriptions[i];
            }
            else
            {
                itemButtonTexts[i].text = baseName;
            }
        }
    }

    public void UseCurrentItem()
    {
        Debug.Log($"{itemNames[currentItemIndex]}을(를) 사용했습니다! (효과: 체력 20 회복)");
    }

    private GameObject GetUIUnderMouse()
    {
        if (Mouse.current == null || EventSystem.current == null) return null;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        if (results.Count > 0) return results[0].gameObject;
        return null;
    }
}
