using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class SkillSlecet : MonoBehaviour
{
    [Header("메인 전투 관리자 참조")]
    [SerializeField] private wall wallManager;

    [Header("기술 버튼 및 텍스트 매핑 (최대 4개)")]
    [SerializeField] private List<Button> skillButtons;
    [SerializeField] private List<TextMeshProUGUI> skillButtonTexts;

    [Header("스킬 실시간 정보 UI 컴포넌트")]
    [SerializeField] private TextMeshProUGUI ppText;
    [SerializeField] private TextMeshProUGUI typeText;

    [Header("배틀 로그 및 연출용 UI")]
    [SerializeField] private GameObject skillWindowContainer; // 스킬 버튼들을 담은 UI 패널 (로그 연출 시 숨김용)
    [SerializeField] private TextMeshProUGUI battleLogText;   // 배틀 메시지 출력용 텍스트

    private List<Skill> currentSkills = new List<Skill>();
    private int currentSkillIndex = 0;
    private int totalSkillsCount = 0;
    private bool isAttacking = false;

    #region 📊 타입 상성 매트릭스 테이블
    private readonly float[,] typeMatchupTable = new float[17, 17]
    {
        /* 노    격    비    독    땅    바    벌    고    강    불    물    풀    전    에    얼    드    악  */
        /*노말(0)*/  { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        /*격투(1)*/  { 2.0f, 1.0f, 0.5f, 0.5f, 1.0f, 2.0f, 0.5f, 0.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f },
        /*비행(2)*/  { 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f },
        /*독(3)*/    { 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        /*땅(4)*/    { 1.0f, 1.0f, 0.0f, 2.0f, 1.0f, 2.0f, 0.5f, 1.0f, 2.0f, 2.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        /*바위(5)*/  { 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f },
        /*벌레(6)*/  { 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f },
        /*고스트(7)*/{ 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f },
        /*강철(8)*/  { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f },
        /*불꽃(9)*/  { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f },
        /*물(10)*/   { 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f },
        /*풀(11)*/   { 1.0f, 1.0f, 0.5f, 0.5f, 2.0f, 2.0f, 0.5f, 1.0f, 0.5f, 0.5f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f },
        /*전기(12)*/ { 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f },
        /*에스퍼(13)*/{ 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.0f },
        /*얼음(14)*/ { 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f },
        /*드래곤(15)*/{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f },
        /*악(16)*/   { 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f }
    };
    #endregion

    private void OnEnable()
    {
        if (wallManager != null) wallManager.isSkillWindowOpen = true;

        isAttacking = false;
        if (skillWindowContainer != null) skillWindowContainer.SetActive(true);
        if (battleLogText != null) battleLogText.text = "어떤 기술을 선택할까?";

        InitSkillWindow();
    }

    private void OnDisable()
    {
        if (wallManager != null) wallManager.isSkillWindowOpen = false;
    }

    private void InitSkillWindow()
    {
        if (wallManager == null || wallManager.player == null || wallManager.player.Length == 0 || wallManager.player[0] == null) return;

        Pokemon activePokemon = wallManager.player[0];
        currentSkills = activePokemon.Skills;
        totalSkillsCount = currentSkills.Count;

        for (int i = 0; i < skillButtonTexts.Count; i++)
        {
            if (i < totalSkillsCount && currentSkills[i] != null)
            {
                skillButtons[i].gameObject.SetActive(true);
                skillButtonTexts[i].text = currentSkills[i].skillName;
            }
            else
            {
                skillButtonTexts[i].text = "-";
                skillButtons[i].gameObject.SetActive(false);
            }
        }

        currentSkillIndex = 0;
        UpdateSkillButtonDisplay();
        UpdateSkillDetails();
    }

    void Update()
    {
        if (isAttacking || totalSkillsCount == 0) return;

        int previousIndex = currentSkillIndex;

        GameObject hoveredUI = GetUIUnderMouse();
        if (hoveredUI != null)
        {
            Button hoveredButton = hoveredUI.GetComponentInParent<Button>();
            if (hoveredButton != null && skillButtons.Contains(hoveredButton))
            {
                int index = skillButtons.IndexOf(hoveredButton);
                if (index < totalSkillsCount) currentSkillIndex = index;
            }
        }
        else
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame ||
                    Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
                {
                    currentSkillIndex--;
                    if (currentSkillIndex < 0) currentSkillIndex = totalSkillsCount - 1;
                }
                else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame ||
                         Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
                {
                    currentSkillIndex++;
                    if (currentSkillIndex >= totalSkillsCount) currentSkillIndex = 0;
                }
            }
        }

        if (currentSkillIndex != previousIndex)
        {
            UpdateSkillButtonDisplay();
            UpdateSkillDetails();
        }

        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
        {
            ExecuteSelectedSkill();
        }
    }

    private void UpdateSkillButtonDisplay()
    {
        for (int i = 0; i < skillButtonTexts.Count; i++)
        {
            if (i >= totalSkillsCount || currentSkills[i] == null) continue;

            string cleanName = currentSkills[i].skillName;
            if (i == currentSkillIndex)
                skillButtonTexts[i].text = $"▶ {cleanName}";
            else
                skillButtonTexts[i].text = cleanName;
        }
    }

    private void UpdateSkillDetails()
    {
        if (currentSkillIndex < 0 || currentSkillIndex >= totalSkillsCount) return;

        Skill selectedSkill = currentSkills[currentSkillIndex];
        if (selectedSkill != null)
        {
            if (ppText != null) ppText.text = $"{selectedSkill.pp} / {selectedSkill.pp}";
            if (typeText != null) typeText.text = $"{selectedSkill.engineeringType}";
        }
    }

    public void ExecuteSelectedSkill()
    {
        if (isAttacking || currentSkillIndex < 0 || currentSkillIndex >= totalSkillsCount) return;

        StartCoroutine(BattleSequenceRoutine(currentSkills[currentSkillIndex]));
    }

    private IEnumerator BattleSequenceRoutine(Skill playerSkill)
    {
        isAttacking = true;

        if (skillWindowContainer != null) skillWindowContainer.SetActive(false);

        Pokemon playerPoke = wallManager.player[0];
        Pokemon enemyPoke = wallManager.enemy[0];

        Skill enemySkill = (enemyPoke.Skills != null && enemyPoke.Skills.Count > 0)
            ? enemyPoke.Skills[Random.Range(0, enemyPoke.Skills.Count)]
            : playerSkill;

        bool isEnemyFaster = enemyPoke.speed > playerPoke.speed;

        if (isEnemyFaster)
        {
            // 1. 적이 더 빠름 -> 적의 공격 (타겟: 플레이어)
            yield return StartCoroutine(ExecuteSkillAction(enemyPoke, playerPoke, enemySkill, wallManager.PlayerHpSlider, true));

            if (playerPoke.nowHp > 0)
            {
                // 플레이어의 반격 (타겟: 적)
                yield return StartCoroutine(ExecuteSkillAction(playerPoke, enemyPoke, playerSkill, wallManager.EnemyHpSlider, false));
            }
        }
        else
        {
            // 2. 플레이어가 더 빠름 -> 플레이어의 공격 (타겟: 적)
            yield return StartCoroutine(ExecuteSkillAction(playerPoke, enemyPoke, playerSkill, wallManager.EnemyHpSlider, false));

            if (enemyPoke.nowHp > 0)
            {
                // 적의 반격 (타겟: 플레이어)
                yield return StartCoroutine(ExecuteSkillAction(enemyPoke, playerPoke, enemySkill, wallManager.PlayerHpSlider, true));
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;

        // 🛠️ 중요: 매니저 오브젝트가 통째로 꺼지지 않게 컨테이너 UI 패널만 정밀하게 비활성화
        if (skillWindowContainer != null)
        {
            skillWindowContainer.SetActive(false);
        }
    }

    /// <summary>
    /// 스킬 행동 처리 서브 코루틴
    /// </summary>
    /// <param name="isTargetPlayer">피격자가 플레이어(피카츄)인지 여부 판단 플래그</param>
    private IEnumerator ExecuteSkillAction(Pokemon attacker, Pokemon defender, Skill skill, Slider targetHpSlider, bool isTargetPlayer)
    {
        if (battleLogText != null)
            battleLogText.text = $"{attacker.pokemonName}의\n{skill.skillName}!";

        yield return new WaitForSeconds(1.3f);

        if (skill.power <= 0)
        {
            if (battleLogText != null)
                battleLogText.text = $"{defender.pokemonName}의 능력치에 변화를 주었다!";
            yield return new WaitForSeconds(1.3f);
        }
        else
        {
            float typeMultiplier = GetTypeMultiplier(skill.engineeringType, defender.type1);
            if (defender.type2 != defender.type1 && (int)defender.type2 > 0)
            {
                typeMultiplier *= GetTypeMultiplier(skill.engineeringType, defender.type2);
            }

            int targetDefense = defender.defense;
            if (targetDefense <= 0) targetDefense = 1;

            int damage = Mathf.RoundToInt(((2 * attacker.level / 5 + 2) * skill.power * attacker.attack / targetDefense / 50 + 2) * typeMultiplier);

            // 데미지 이전 체력 기록 (부드러운 텍스트 감소용)
            int hpBeforeDamage = defender.nowHp;

            defender.nowHp -= damage;
            if (defender.nowHp < 0) defender.nowHp = 0;

            // HP 바 및 텍스트 감산 실시간 연출
            if (targetHpSlider != null)
            {
                float targetValue = (float)defender.nowHp / defender.maxHp;
                float startValue = targetHpSlider.value;
                float elapsedTime = 0f;
                float duration = 0.5f;

                while (elapsedTime < duration)
                {
                    elapsedTime += Time.unscaledDeltaTime;
                    float currentProgress = elapsedTime / duration;

                    // 슬라이더 바 조정
                    targetHpSlider.value = Mathf.Lerp(startValue, targetValue, currentProgress);

                    // 💥 [추가]: 피격자가 플레이어(피카츄)일 때 텍스트 글자 수치도 슬라이더 속도에 맞춰 실시간 감산
                    if (isTargetPlayer)
                    {
                        int visualHp = Mathf.RoundToInt(Mathf.Lerp(hpBeforeDamage, defender.nowHp, currentProgress));
                        wallManager.UpdatePlayerHpTextRealtime(visualHp);
                    }

                    yield return null;
                }
                targetHpSlider.value = targetValue;

                // 최종 연출 오차 고정
                if (isTargetPlayer)
                {
                    wallManager.UpdatePlayerHpTextRealtime(defender.nowHp);
                }
            }
            yield return new WaitForSeconds(0.3f);

            if (battleLogText != null)
            {
                if (typeMultiplier >= 2.0f)
                {
                    battleLogText.text = "효과가 굉장했다!";
                    yield return new WaitForSeconds(1.3f);
                }
                else if (typeMultiplier > 0f && typeMultiplier <= 0.5f)
                {
                    battleLogText.text = "효과가 별로인 듯하다...";
                    yield return new WaitForSeconds(1.3f);
                }
                else if (typeMultiplier == 0f)
                {
                    battleLogText.text = $"{defender.pokemonName}에게는\n효과가 없는 것 같다...";
                    yield return new WaitForSeconds(1.3f);
                }
            }
        }
    }

    private float GetTypeMultiplier(PokemonType attackType, PokemonType defenseType)
    {
        int atkIndex = GetTypeIndex(attackType);
        int defIndex = GetTypeIndex(defenseType);

        if (atkIndex < 0 || atkIndex >= 17 || defIndex < 0 || defIndex >= 17) return 1.0f;

        return typeMatchupTable[atkIndex, defIndex];
    }

    private int GetTypeIndex(PokemonType type)
    {
        int value = (int)type;
        if (value <= 0) return 0;
        return System.Convert.ToString(value, 2).Length - 1;
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