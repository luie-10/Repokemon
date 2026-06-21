using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems; // 마우스 레이캐스트(UI 감지)를 위해 필요
using UnityEngine.InputSystem;  // 새로운 Input System 사용을 위해 필요

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

    [Header("감시할 버튼 목록")]
    public List<Button> buttonsToWatch;

    [HideInInspector] public int maxHP;
    [HideInInspector] public int nowHP;
    [HideInInspector] public int attack;
    [HideInInspector] public int defense;
    [HideInInspector] public int spAtk;
    [HideInInspector] public int spDef;
    [HideInInspector] public int speed;

    // 현재 선택된 메뉴의 인덱스 (0: FIGHT, 1: BAG, 2: Pokémon, 3: RUN)
    private int Choice = 0;
    private const int TotalMenuCount = 4;

    // 다른 스크립트(SlectButton 등)에서 구독하는 이벤트
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
            nowHP = maxHP;
        }

        UpdateBattleUI();
        SlectNum(); // 시작할 때 0번 선택 상태 알림

        if (buttonsToWatch != null)
        {
            foreach (var button in buttonsToWatch)
            {
                if (button != null)
                {
                    // 버튼을 직접 클릭했을 때도 해당 메뉴가 실행되도록 이벤트 연결
                    button.onClick.AddListener(() => ExecuteCurrentChoice());
                }
            }
        }
    }

    private void Update()
    {
        if (isSkillWindowOpen) return;
        int previousChoice = Choice; // 변경 전의 Choice 값을 저장 (변화 감지용)

        // 🔍 [1단계] 현재 마우스 커서 바로 밑에 있는 UI 게임 오브젝트를 획득
        GameObject hoveredUI = GetUIUnderMouse();

        // 마우스 밑에 UI가 존재한다면 (마우스 입력 처리)
        if (hoveredUI != null)
        {
            // 해당 UI나 그 부모에게서 Button 컴포넌트가 있는지 탐색
            Button hoveredButton = hoveredUI.GetComponentInParent<Button>();

            // 버튼을 찾았고, 그 버튼이 내가 감시하는 버튼 목록(buttonsToWatch)에 들어있다면
            if (hoveredButton != null && buttonsToWatch.Contains(hoveredButton))
            {
                // 🔍 [2단계] 마우스가 올라간 버튼에 맞춰 Choice(인덱스) 변경 함수 호출
                OnButtonHover(hoveredButton);
            }
        }
        // 마우스 밑에 UI가 없다면 (키보드 입력 처리)
        else
        {
            if (Keyboard.current != null)
            {
                // 위쪽 화살표나 W 키를 누르면 인덱스 감소
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
                {
                    Choice--;
                    if (Choice < 0) Choice = TotalMenuCount - 1; // 첫 번째에서 위로 가면 마지막으로
                }
                // 아래쪽 화살표나 S 키를 누르면 인덱스 증가
                else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
                {
                    Choice++;
                    if (Choice >= TotalMenuCount) Choice = 0; // 마지막에서 아래로 가면 첫 번째로
                }
            }
        }

        // 🔍 [3단계] 이번 프레임에서 Choice(인덱스) 번호에 변동이 생겼다면!
        if (Choice != previousChoice)
        {
            SlectNum(); // OnChoiceChanged 이벤트를 발동시켜 SlectButton UI를 갱신하게 함
        }

        // 엔터나 스페이스바를 누르면 현재 Choice 번호 확정 및 실행
        if (Keyboard.current != null)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ExecuteCurrentChoice();
            }
        }
    }

    private void UpdateBattleUI()
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
        }

        if (enemy != null && enemy.Length > 0 && enemy[0] != null)
        {
            if (enemyNameText != null) enemyNameText.text = enemy[0].pokemonName;
            if (enemyLevelText != null) enemyLevelText.text = $"{enemy[0].level}";
        }
    }

    public void TakeDamage(int damage)
    {
        nowHP -= damage;
        if (nowHP < 0) nowHP = 0;

        if (PlayerHpSlider != null) PlayerHpSlider.value = nowHP;
        if (playerHpText != null) playerHpText.text = $"{nowHP} / {maxHP}";
    }

    /// <summary>
    /// 플레이어 포켓몬의 체력 텍스트(ex: 75 / 100)를 실시간으로 갱신하는 함수
    /// </summary>
    public void UpdatePlayerHpTextRealtime(int currentHp)
    {
        if (playerHpText != null && player != null && player.Length > 0 && player[0] != null)
        {
            playerHpText.text = $"{currentHp} / {player[0].maxHp}";
        }
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

    /// <summary>
    /// 🔍 [핵심 로직 1] 현재 마우스 커서 위치에 있는 UI 오브젝트를 레이캐스트로 찾아 반환하는 함수
    /// </summary>
    private GameObject GetUIUnderMouse()
    {
        if (Mouse.current == null) return null;

        // 마우스의 현재 스크린 위치 좌표를 가져옴
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        // 마우스 위치에서 쏜 레이에 부딪힌 UI 인포들을 담을 리스트
        List<RaycastResult> results = new List<RaycastResult>();

        // UI 시스템 전체에 레이캐스트를 발사하여 검사
        EventSystem.current.RaycastAll(eventData, results);

        // 부딪힌 UI가 하나라도 있다면 가장 앞에 레이어된(첫 번째) 오브젝트를 반환
        if (results.Count > 0) return results[0].gameObject;
        return null;
    }

    /// <summary>
    /// 🔍 [핵심 로직 2] 마우스가 올라간 버튼의 이름(Hierarchy 창 이름 기준)을 체크하여 Choice 값을 세팅하는 함수
    /// </summary>
    private void OnButtonHover(Button targetButton)
    {
        switch (targetButton.gameObject.name)
        {
            case "Fight": Choice = 0; break; // Fight 버튼 위라면 인덱스 0
            case "BAG": Choice = 1; break; // BAG 버튼 위라면 인덱스 1
            case "Pokémon": Choice = 2; break; // Pokémon 버튼 위라면 인덱스 2
            case "RUN": Choice = 3; break; // RUN 버튼 위라면 인덱스 3
        }
    }

    /// <summary>
    /// 현재 Choice 번호가 바뀌었음을 구독자들(SlectButton 등)에게 방송하는 함수
    /// </summary>
    private void SlectNum()
    {
        OnChoiceChanged?.Invoke(Choice);
    }

    /// <summary>
    /// 현재 Choice 번호의 메뉴를 최종 실행(확정)하겠다고 방송하는 함수
    /// </summary>
    private void ExecuteCurrentChoice()
    {
        OnChoiceConfirmed?.Invoke(Choice);
    }
}