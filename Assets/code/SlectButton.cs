using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
            wallManager.OnChoiceConfirmed += OnMenuConfirmed;
        }
    }

    private void OnDisable()
    {
        if (wallManager != null)
        {
            wallManager.OnChoiceChanged -= OnMenuChanged;
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
        OnMenuChanged(0);
    }

    public void OnMenuChanged(int currentChoice)
    {
        if (buttonTexts == null || originalNames.Count == 0) return;

        for (int i = 0; i < buttonTexts.Count; i++)
        {
            if (i >= originalNames.Count || buttonTexts[i] == null) continue;

            if (i == currentChoice)
                buttonTexts[i].text = $"▶ {originalNames[i]}";
            else
                buttonTexts[i].text = originalNames[i];
        }
    }

    private void OnMenuConfirmed(int confirmedChoice)
    {
        Debug.Log($"[SlectButton 수신] {confirmedChoice}번 메뉴가 최종 선택되었습니다!");

        switch (confirmedChoice)
        {
            case 0: // FIGHT 
                Fight();
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
        Debug.Log("싸우기 메뉴 진입! 기술 선택창 활성화");

        // 1. 메인 선택 버튼 인터랙션 비활성화 및 숨김
        SetAllButtonsActive(false);
        if (ChoiceSlecet != null) ChoiceSlecet.SetActive(false);

        // 2. 기술 선택 UI 창 활성화
        if (ChoiceSkill != null) ChoiceSkill.SetActive(true);

        // ⭐ [핵심 추가] 같은 오브젝트 또는 자식에 있는 SkillSlecet 스크립트를 활성화하여 조작권 전환
        SkillSlecet skillSelectComp = GetComponentInChildren<SkillSlecet>(true);
        if (skillSelectComp != null)
        {
            skillSelectComp.enabled = true;
        }
    }

    void OpenBag() { Debug.Log("가방 열기 실행"); }
    void OpenPokemonMenu() { Debug.Log("포켓몬 엔트리 열기 실행"); }
    void RunAway() { Debug.Log("도망치기 실행"); }
}