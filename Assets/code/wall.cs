using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class wall : MonoBehaviour
{
    [Header("player")]
    public Pokemon[] player;
    public TextMeshProUGUI PlayerHPbar;
    public Slider PlayerHpSlider;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI PlayerLv;

    [Header("감시할 버튼 목록")]
    public List<Button> buttonsToWatch;

    [Header("enemy")]
    public Pokemon[] enemy;
    public TextMeshProUGUI EnemyHPbar;
    public TextMeshProUGUI EnemyName;

    private int Choice = 0;
    private const int TotalMenuCount = 4;

    // 이벤트를 2개로 관리합니다.
    public event Action<int> OnChoiceChanged;    // 1. 단순 번호 변경 알림
    public event Action<int> OnChoiceConfirmed;  // 2. [추가] 엔터/클릭 최종 확정 알림

    void Start()
    {
        SlectNum();

        if (player != null && player.Length > 0 && player[0] != null)
        {
            PlayerName.text = player[0].pokemonName;
            PlayerHPbar.text = $"{player[0].nowHp.ToString()}/  {player[0].maxHp.ToString()}";
            PlayerLv.text = $"{player[0].level.ToString()}";
        }

        // [추가] 마우스로 버튼을 '직접 클릭'했을 때도 동일하게 확정 이벤트를 발생시키도록 세팅
        if (buttonsToWatch != null)
        {
            foreach (var button in buttonsToWatch)
            {
                if (button != null)
                {
                    // 버튼을 누르면 현재 마우스가 올라가 있는 Choice 번호를 가지고 확정 함수를 실행함
                    button.onClick.AddListener(() => ExecuteCurrentChoice());
                }
            }
        }
    }

    private void Update()
    {
        int previousChoice = Choice;

        GameObject hoveredUI = GetUIUnderMouse();

        if (hoveredUI != null)
        {
            Button hoveredButton = hoveredUI.GetComponentInParent<Button>();

            if (hoveredButton != null && buttonsToWatch.Contains(hoveredButton))
            {
                OnButtonHover(hoveredButton);
            }
        }
        else
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
                {
                    Choice--;
                    if (Choice < 0) Choice = TotalMenuCount - 1;
                }
                else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
                {
                    Choice++;
                    if (Choice >= TotalMenuCount) Choice = 0;
                }
            }
        }

        if (Choice != previousChoice)
        {
            SlectNum();
        }

        // 키보드 엔터나 스페이스바를 누르면 실행
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ExecuteCurrentChoice();
            }
        }
    }

    private GameObject GetUIUnderMouse()
    {
        if (Mouse.current == null) return null;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0) return results[0].gameObject;
        return null;
    }

    private void OnButtonHover(Button targetButton)
    {
        switch (targetButton.gameObject.name)
        {
            case "Fight": Choice = 0; break;
            case "BAG": Choice = 1; break;
            case "Pokémon": Choice = 2; break;
            case "RUN": Choice = 3; break;
        }
    }

    private void SlectNum()
    {
        OnChoiceChanged?.Invoke(Choice);
    }

    // 마우스 클릭이나 엔터를 치면 실행되는 최종 통로
    private void ExecuteCurrentChoice()
    {
        Debug.Log($"[wall] {Choice}번 메뉴 확정 전송!");
        // 구독하고 있는 SlectButton아, 방금 확정된 Choice 번호 실행해라! 하고 쏩니다.
        OnChoiceConfirmed?.Invoke(Choice);
    }
}