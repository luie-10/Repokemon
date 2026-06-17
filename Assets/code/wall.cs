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
    [Header("Player Pokemon Data")]
    public Pokemon[] player;

    [Header("Enemy Pokemon Data")]
    public Pokemon[] enemy;

    [Header("UI Components (Sliders & Texts)")]
    public Slider PlayerHpSlider;
    // 🌟 [추가] 플레이어 및 적의 이름, 레벨을 표시할 UI 텍스트 참조
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyLevelText;
    // (선택사항) 체력 수치를 텍스트로도 보고 싶다면 추가하세요 (예: 100 / 100)
    [SerializeField] private TextMeshProUGUI playerHpText;

    [Header("감시할 버튼 목록")]
    public List<Button> buttonsToWatch;

    [HideInInspector] public int maxHP;
    [HideInInspector] public int nowHP;
    [HideInInspector] public int attack;
    [HideInInspector] public int defense;
    [HideInInspector] public int spAtk;
    [HideInInspector] public int spDef;
    [HideInInspector] public int speed;

    private int Choice = 0;
    private const int TotalMenuCount = 4;

    public event Action<int> OnChoiceChanged;
    public event Action<int> OnChoiceConfirmed;

    void Start()
    {
        if (player != null && player.Length > 0 && player[0] != null)
        {
            Pokemon p = player[0];

            attack = p.attack;
            defense = p.defense;
            spAtk = p.spAtk;
            spDef = p.spDef;
            speed = p.speed;

            maxHP = CalculateMaxHP(p.level, p.maxHp, 31, 252);
            nowHP = maxHP;
        }

        // 🌟 [추가] 전투 시작 시 UI 정보(이름, 레벨, 체력)들을 시각적으로 갱신
        UpdateBattleUI();

        SlectNum();

        if (buttonsToWatch != null)
        {
            foreach (var button in buttonsToWatch)
            {
                if (button != null)
                {
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

        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ExecuteCurrentChoice();
            }
        }
    }

    /// <summary>
    /// 🌟 [추가] 스크립터블 오브젝트의 데이터와 현재 체력을 UI에 반영하는 함수
    /// </summary>
    private void UpdateBattleUI()
    {
        // 1. 플레이어 UI 갱신
        if (player != null && player.Length > 0 && player[0] != null)
        {
            if (playerNameText != null) playerNameText.text = player[0].pokemonName;
            if (playerLevelText != null) playerLevelText.text = $"Lv.{player[0].level}";

            // 체력 슬라이더 연동
            if (PlayerHpSlider != null)
            {
                PlayerHpSlider.maxValue = maxHP;
                PlayerHpSlider.value = nowHP;
            }
            // 체력 텍스트가 있다면 반영 (ex: 45 / 45)
            if (playerHpText != null) playerHpText.text = $"{nowHP} / {maxHP}";
        }

        // 2. 적 UI 갱신
        if (enemy != null && enemy.Length > 0 && enemy[0] != null)
        {
            if (enemyNameText != null) enemyNameText.text = enemy[0].pokemonName;
            if (enemyLevelText != null) enemyLevelText.text = $"Lv.{enemy[0].level}";

            // 적 체력 슬라이더도 나중에 기회가 된다면 여기에 같은 방식으로 추가하시면 됩니다.
        }
    }

    /// <summary>
    /// 🌟 [추가] 데미지를 받거나 체력이 변할 때 호출하는 외부 메서드
    /// </summary>
    public void TakeDamage(int damage)
    {
        nowHP -= damage;
        if (nowHP < 0) nowHP = 0;

        // 체력이 바뀔 때마다 UI를 다시 그려줍니다.
        if (PlayerHpSlider != null) PlayerHpSlider.value = nowHP;
        if (playerHpText != null) playerHpText.text = $"{nowHP} / {maxHP}";
    }

    public int CalculateMaxHP(int lvl, int @base, int iv, int ev)
    {
        int evCalc = ev / 4;
        int coreCalc = ((@base * 2) + iv + evCalc) * lvl;
        int step1 = coreCalc / 100;
        return step1 + lvl + 10;
    }

    public int GetHPPercentage()
    {
        if (maxHP <= 0) return 0;
        float hpRatio = (float)nowHP / maxHP;
        return Mathf.RoundToInt(hpRatio * 100f);
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

    private void ExecuteCurrentChoice()
    {
        OnChoiceConfirmed?.Invoke(Choice);
    }
}