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
            case 0:
                Fight();
                break;
            case 1:
                OpenBag();
                break;
            case 2:
                OpenPokemonMenu();
                break;
            case 3:
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

    void Fight()
    {
        Debug.Log("싸우기 메뉴 진입! 기술 선택창 활성화");

        SetAllButtonsActive(false);
        if (ChoiceSlecet != null) ChoiceSlecet.SetActive(false);

        if (ChoiceSkill != null) ChoiceSkill.SetActive(true);

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
