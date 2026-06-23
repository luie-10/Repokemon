using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems; // 마우스 레이캐스트(UI 감지)를 위해 필요[cite: 6]
using UnityEngine.InputSystem;  // 새로운 Input System 사용을 위해 필요[cite: 6]

public class wall : MonoBehaviour
{
    [Header("Player Pokemon Data")]
    public Pokemon[] player; //[cite: 6]

    [Header("Enemy Pokemon Data")]
    public Pokemon[] enemy; //[cite: 6]

    [Header("UI Components (Sliders & Texts)")]
    public Slider PlayerHpSlider; //[cite: 6]
    public Slider EnemyHpSlider; //[cite: 6]
    [SerializeField] private TextMeshProUGUI playerNameText; //[cite: 6]
    [SerializeField] private TextMeshProUGUI playerLevelText; //[cite: 6]
    [SerializeField] private TextMeshProUGUI enemyNameText; //[cite: 6]
    [SerializeField] private TextMeshProUGUI enemyLevelText; //[cite: 6]
    [SerializeField] private TextMeshProUGUI playerHpText; //[cite: 6]

    [Header("✨ 상태이상 UI 설정 (위치 및 이미지 관리 컴포넌트)")]
    public StatusUIContainer playerStatusUI; // 플레이어 상태이상 아이콘이 배치될 위치 컴포넌트
    public StatusUIContainer enemyStatusUI;  // 적 상태이상 아이콘이 배치될 위치 컴포넌트

    [Header("감시할 버튼 목록")]
    public List<Button> buttonsToWatch; //[cite: 6]

    [HideInInspector] public int maxHP; //[cite: 6]
    [HideInInspector] public int nowHP; //[cite: 6]
    [HideInInspector] public int attack; //[cite: 6]
    [HideInInspector] public int defense; //[cite: 6]
    [HideInInspector] public int spAtk; //[cite: 6]
    [HideInInspector] public int spDef; //[cite: 6]
    [HideInInspector] public int speed; //[cite: 6]

    // 현재 선택된 메뉴의 인덱스 (0: FIGHT, 1: BAG, 2: Pokémon, 3: RUN)[cite: 6]
    private int Choice = 0; //[cite: 6]
    private const int TotalMenuCount = 4; //[cite: 6]

    // 다른 스크립트(SlectButton 등)에서 구독하는 이벤트[cite: 6]
    public event Action<int> OnChoiceChanged; //[cite: 6]
    public event Action<int> OnChoiceConfirmed; //[cite: 6]

    [HideInInspector] public bool isSkillWindowOpen = false; //[cite: 6]

    void Start()
    {
        if (player != null && player.Length > 0 && player[0] != null) //[cite: 6]
        {
            Pokemon p = player[0]; //[cite: 6]
            attack = p.attack; //[cite: 6]
            defense = p.defense; //[cite: 6]
            spAtk = p.spAtk; //[cite: 6]
            spDef = p.spDef; //[cite: 6]
            speed = p.speed; //[cite: 6]

            maxHP = CalculateMaxHP(p.level, p.maxHp, 31, 252); //[cite: 6]
            nowHP = maxHP; //[cite: 6]
        }

        UpdateBattleUI(); // UI 초기화 및 첫 동기화[cite: 6]
        SlectNum(); // 시작할 때 0번 선택 상태 알림[cite: 6]

        if (buttonsToWatch != null) //[cite: 6]
        {
            foreach (var button in buttonsToWatch) //[cite: 6]
            {
                if (button != null) //[cite: 6]
                {
                    // 버튼을 직접 클릭했을 때도 해당 메뉴가 실행되도록 이벤트 연결[cite: 6]
                    button.onClick.AddListener(() => ExecuteCurrentChoice()); //[cite: 6]
                }
            }
        }
    }

    private void Update()
    {
        if (isSkillWindowOpen) return; //[cite: 6]
        int previousChoice = Choice; // 변경 전의 Choice 값을 저장 (변화 감지용)[cite: 6]

        // 🔍 [1단계] 현재 마우스 커서 바로 밑에 있는 UI 게임 오브젝트를 획득[cite: 6]
        GameObject hoveredUI = GetUIUnderMouse(); //[cite: 6]

        // 마우스 밑에 UI가 존재한다면 (마우스 입력 처리)[cite: 6]
        if (hoveredUI != null) //[cite: 6]
        {
            // 해당 UI나 그 부모에게서 Button 컴포넌트가 있는지 탐색[cite: 6]
            Button hoveredButton = hoveredUI.GetComponentInParent<Button>(); //[cite: 6]

            // 버튼을 찾았고, 그 버튼이 내가 감시하는 버튼 목록(buttonsToWatch)에 들어있다면[cite: 6]
            if (hoveredButton != null && buttonsToWatch.Contains(hoveredButton)) //[cite: 6]
            {
                // 🔍 [2단계] 마우스가 올라간 버튼에 맞춰 Choice(인덱스) 변경 함수 호출[cite: 6]
                OnButtonHover(hoveredButton); //[cite: 6]
            }
        }
        // 마우스 밑에 UI가 없다면 (키보드 입력 처리)[cite: 6]
        else
        {
            if (Keyboard.current != null) //[cite: 6]
            {
                // 위쪽 화살표나 W 키를 누르면 인덱스 감소[cite: 6]
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) //[cite: 6]
                {
                    Choice--; //[cite: 6]
                    if (Choice < 0) Choice = TotalMenuCount - 1; // 첫 번째에서 위로 가면 마지막으로[cite: 6]
                }
                // 아래쪽 화살표나 S 키를 누르면 인덱스 증가[cite: 6]
                else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) //[cite: 6]
                {
                    Choice++; //[cite: 6]
                    if (Choice >= TotalMenuCount) Choice = 0; // 마지막에서 아래로 가면 첫 번째로[cite: 6]
                }
            }
        }

        // 🔍 [3단계] 이번 프레임에서 Choice(인덱스) 번호에 변동이 생겼다면![cite: 6]
        if (Choice != previousChoice) //[cite: 6]
        {
            SlectNum(); // OnChoiceChanged 이벤트를 발동시켜 SlectButton UI를 갱신하게 함[cite: 6]
        }

        // 엔터나 스페이스바를 누르면 현재 Choice 번호 확정 및 실행[cite: 6]
        if (Keyboard.current != null) //[cite: 6]
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) //[cite: 6]
            {
                ExecuteCurrentChoice(); //[cite: 6]
            }
        }
    }

    /// <summary>
    /// 포켓몬 데이터와 상태이상을 종합하여 배틀 스크린 UI를 갱신합니다.
    /// </summary>
    public void UpdateBattleUI()
    {
        // 1. 플레이어 정보 및 상태이상 UI 갱신
        if (player != null && player.Length > 0 && player[0] != null) //[cite: 6]
        {
            if (playerNameText != null) playerNameText.text = player[0].pokemonName; //[cite: 6]
            if (playerLevelText != null) playerLevelText.text = $"{player[0].level}"; //[cite: 6]

            if (PlayerHpSlider != null) //[cite: 6]
            {
                PlayerHpSlider.maxValue = maxHP; //[cite: 6]
                PlayerHpSlider.value = nowHP; //[cite: 6]
            }
            if (playerHpText != null) playerHpText.text = $"{nowHP} / {maxHP}"; //[cite: 6]

            // ✨ [추가] 플레이어 인스펙터 지정 위치에 상태이상 아이콘 업데이트
            if (playerStatusUI != null)
            {
                playerStatusUI.UpdateStatusUI(player[0].currentStatus);
            }
        }

        // 2. 적 정보 및 상태이상 UI 갱신
        if (enemy != null && enemy.Length > 0 && enemy[0] != null) //[cite: 6]
        {
            if (enemyNameText != null) enemyNameText.text = enemy[0].pokemonName; //[cite: 6]
            if (enemyLevelText != null) enemyLevelText.text = $"{enemy[0].level}"; //[cite: 6]

            if (EnemyHpSlider != null) //[cite: 6]
            {
                EnemyHpSlider.maxValue = enemy[0].maxHp;
                EnemyHpSlider.value = enemy[0].nowHp;
            }

            // ✨ [추가] 적 인스펙터 지정 위치에 상태이상 아이콘 업데이트
            if (enemyStatusUI != null)
            {
                enemyStatusUI.UpdateStatusUI(enemy[0].currentStatus);
            }
        }
    }

    public void TakeDamage(int damage) //[cite: 6]
    {
        nowHP -= damage; //[cite: 6]
        if (nowHP < 0) nowHP = 0; //[cite: 6]

        if (PlayerHpSlider != null) PlayerHpSlider.value = nowHP; //[cite: 6]
        if (playerHpText != null) playerHpText.text = $"{nowHP} / {maxHP}"; //[cite: 6]
    }

    /// <summary>
    /// 플레이어 포켓몬의 체력 텍스트(ex: 75 / 100)를 실시간으로 갱신하는 함수[cite: 6]
    /// </summary>
    public void UpdatePlayerHpTextRealtime(int currentHp) //[cite: 6]
    {
        if (playerHpText != null && player != null && player.Length > 0 && player[0] != null) //[cite: 6]
        {
            playerHpText.text = $"{currentHp} / {player[0].maxHp}"; //[cite: 6]
        }
    }

    public int CalculateMaxHP(int lvl, int @base, int iv, int ev) //[cite: 6]
    {
        int evCalc = ev / 4; //[cite: 6]
        int coreCalc = ((@base * 2) + iv + evCalc) * lvl; //[cite: 6]
        int step1 = coreCalc / 100; //[cite: 6]
        return step1 + lvl + 10; //[cite: 6]
    }

    public int GetHPPercentage() //[cite: 6]
    {
        if (maxHP <= 0) return 0; //[cite: 6]
        float hpRatio = (float)nowHP / maxHP; //[cite: 6]
        return Mathf.RoundToInt(hpRatio * 100f); //[cite: 6]
    }

    /// <summary>
    /// 🔍 [핵심 로직 1] 현재 마우스 커서 위치에 있는 UI 오브젝트를 레이캐스트로 찾아 반환하는 함수[cite: 6]
    /// </summary>
    private GameObject GetUIUnderMouse() //[cite: 6]
    {
        if (Mouse.current == null) return null; //[cite: 6]

        PointerEventData eventData = new PointerEventData(EventSystem.current); //[cite: 6]
        eventData.position = Mouse.current.position.ReadValue(); //[cite: 6]

        List<RaycastResult> results = new List<RaycastResult>(); //[cite: 6]
        EventSystem.current.RaycastAll(eventData, results); //[cite: 6]

        if (results.Count > 0) return results[0].gameObject; //[cite: 6]
        return null; //[cite: 6]
    }

    /// <summary>
    /// 🔍 [핵심 로직 2] 마우스가 올라간 버튼의 이름을 체크하여 Choice 값을 세팅하는 함수[cite: 6]
    /// </summary>
    private void OnButtonHover(Button targetButton) //[cite: 6]
    {
        switch (targetButton.gameObject.name) //[cite: 6]
        {
            case "Fight": Choice = 0; break; //[cite: 6]
            case "BAG": Choice = 1; break; //[cite: 6]
            case "Pokémon": Choice = 2; break; //[cite: 6]
            case "RUN": Choice = 3; break; //[cite: 6]
        }
    }

    /// <summary>
    /// 현재 Choice 번호가 바뀌었음을 구독자들(SlectButton 등)에게 방송하는 함수[cite: 6]
    /// </summary>
    private void SlectNum() //[cite: 6]
    {
        OnChoiceChanged?.Invoke(Choice); //[cite: 6]
    }

    /// <summary>
    /// 현재 Choice 번호의 메뉴를 최종 실행(확정)하겠다고 방송하는 함수[cite: 6]
    /// </summary>
    private void ExecuteCurrentChoice() //[cite: 6]
    {
        OnChoiceConfirmed?.Invoke(Choice); //[cite: 6]
    }
}