using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlectButton : MonoBehaviour
{
    [Header("참조 연결")]
    [SerializeField] private wall wallManager;

    [Header("버튼 및 텍스트 매핑")]
    [SerializeField] private List<TextMeshProUGUI> buttonTexts;

    private List<string> originalNames = new List<string>();
    [SerializeField] protected List<Button> SetAttiveUI;
    public GameObject ChoiceSlecet;
    public GameObject ChoiceSkill;

    private void OnEnable()
    {
        if (wallManager != null)
        {
            wallManager.OnChoiceChanged += OnMenuChanged;

            // [추가] 확정 이벤트 구독 신청!
            wallManager.OnChoiceConfirmed += OnMenuConfirmed;
        }
    }

    private void OnDisable()
    {
        if (wallManager != null)
        {
            wallManager.OnChoiceChanged -= OnMenuChanged;

            // [추가] 구독 해제
            wallManager.OnChoiceConfirmed -= OnMenuConfirmed;
        }
    }

    void Start()
    {
        if (buttonTexts != null)
        {
            foreach (var textMesh in buttonTexts)
            {
                if (textMesh != null)
                {
                    string cleanName = textMesh.text.Replace("▶", "").Trim();
                    originalNames.Add(cleanName);
                }
            }
        }

        if (wallManager != null && wallManager.buttonsToWatch != null)
        {
            foreach (var button in wallManager.buttonsToWatch)
            {
                if (button != null && !SetAttiveUI.Contains(button))
                {
                    SetAttiveUI.Add(button);
                }
            }
        }

        // 시작 시 활성화하려면 true, 숨기려면 false (상황에 맞게 조절하세요)
        SetAllButtonsActive(true);
    }

    // [옵저버 1] 실시간 화살표 표시 전환 (기존 유지)
    private void OnMenuChanged(int currentChoice)
    {
        if (buttonTexts == null || buttonTexts.Count == 0 || originalNames.Count == 0) return;

        for (int i = 0; i < buttonTexts.Count; i++)
        {
            if (buttonTexts[i] == null || i >= originalNames.Count) continue;

            if (i == currentChoice)
                buttonTexts[i].text = $"▶ {originalNames[i]}";
            else
                buttonTexts[i].text = originalNames[i];
        }
    }

    // [옵저버 2 - 추가] 엔터를 치거나 마우스로 클릭했을 때 "최종 실행"되는 핵심 연출 공간!
    private void OnMenuConfirmed(int confirmedChoice)
    {
        Debug.Log($"[SlectButton 수신] {confirmedChoice}번 메뉴가 최종 선택되었습니다! 다음 연출을 실행합니다.");

        switch (confirmedChoice)
        {
            case 0: // FIGHT 
                Fight(); // 아래 만들어 두신 Fight 함수가 여기서 실행됩니다!
                break;

            case 1: // BAG
                OpenBag();
                break;

            case 2: // Pokémon
                OpenPokemonMenu();
                break;

            case 3: // RUN
                RunAway();
                break;
        }
    }

    public void SetAllButtonsActive(bool isActive)
    {
        foreach (var b in SetAttiveUI)
        {
            if (b != null) b.gameObject.SetActive(isActive);
        }
    }

    public void ON() => SetAllButtonsActive(true);

    // --- 각 메뉴가 선택되었을 때 실행될 세부 함수 공간 ---
    void Fight()
    {
        Debug.Log("싸우기 메뉴 진입! 기술 선택 UI창을 켭니다.");
        ChoiceSlecet.SetActive(false);
        ChoiceSkill.SetActive(true);
    }

    void OpenBag()
    {
        Debug.Log("가방 열기!");
    }

    void OpenPokemonMenu()
    {
        Debug.Log("포켓몬 교체 창 열기!");
    }

    void RunAway()
    {
        Debug.Log("전투에서 도망쳤습니다!");
    }
}