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
    public Slider EnemyHpSlider;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyLevelText;
    [SerializeField] private TextMeshProUGUI playerHpText;

    [Header("✨ 상태이상 UI 설정 (위치 및 이미지 관리 컴포넌트)")]
    public StatusUIContainer playerStatusUI;
    public StatusUIContainer enemyStatusUI;

    [Header("감시할 버튼 목록")]
    public List<Button> buttonsToWatch;

    // 중복 데이터 변수 제거 및 속성(Property)화를 통해 player[0].nowHp와 직접 연결
    public int maxHP { get; private set; }
    public int nowHP
    {
        get { return (player != null && player.Length > 0 && player[0] != null) ? player[0].nowHp : 0; }
        set { if (player != null && player.Length > 0 && player[0] != null) player[0].nowHp = Mathf.Clamp(value, 0, maxHP); }
    }

    [HideInInspector] public int attack;
    [HideInInspector] public int defense;
    [HideInInspector] public int spAtk;
    [HideInInspector] public int spDef;
    [HideInInspector] public int speed;

    private int Choice = 0;
    private const int TotalMenuCount = 4;

    public event Action<int> OnChoiceChanged;
    public event Action<int> OnChoiceConfirmed;

    [HideInInspector] public bool isSkillWindowOpen = false;

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
            p.maxHp = maxHP; // 포켓몬 자체 스펙의 maxHp도 함께 동기화
            nowHP = maxHP;
        }

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
        if (isSkillWindowOpen) return;
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

    public void UpdateBattleUI()
    {
        if (player != null && player.Length > 0 && player[0] != null)
        {
            if (playerNameText != null) playerNameText.text = player[0].pokemonName;
            if (playerLevelText != null) playerLevelText.text = $"{player[0].level}";

            if (PlayerHpSlider != null)
            {
                PlayerHpSlider.maxValue = maxHP;
                PlayerHpSlider.value = nowHP;
            }
            if (playerHpText != null) playerHpText.text = $"{nowHP} / {maxHP}";

            if (playerStatusUI != null)
            {
                playerStatusUI.UpdateStatusUI(player[0].currentStatus);
            }
        }

        if (enemy != null && enemy.Length > 0 && enemy[0] != null)
        {
            if (enemyNameText != null) enemyNameText.text = enemy[0].pokemonName;
            if (enemyLevelText != null) enemyLevelText.text = $"{enemy[0].level}";

            if (EnemyHpSlider != null)
            {
                EnemyHpSlider.maxValue = enemy[0].maxHp;
                EnemyHpSlider.value = enemy[0].nowHp;
            }

            if (enemyStatusUI != null)
            {
                enemyStatusUI.UpdateStatusUI(enemy[0].currentStatus);
            }
        }
    }

    // 슬라이더 애니메이션 도중 실시간 텍스트 갱신을 위한 보조 함수
    public void UpdatePlayerHpTextRealtime(int currentHp)
    {
        if (playerHpText != null)
        {
            playerHpText.text = $"{currentHp} / {maxHP}";
        }
    }

    public void TakeDamage(int damage)
    {
        nowHP -= damage;
        UpdateBattleUI();
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
        if (Mouse.current == null || EventSystem.current == null) return null;

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