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
    [SerializeField] private GameObject skillWindowContainer;
    [SerializeField] private TextMeshProUGUI battleLogText;

    [Header("메인 메뉴 UI 컴포넌트")]
    [SerializeField] private GameObject mainMenuContainer;

    private List<Skill> currentSkills = new List<Skill>();
    private int currentSkillIndex = 0;
    private int totalSkillsCount = 0;
    private bool isAttacking = false;

    #region [타입 상성 매트릭스 테이블]
    private readonly float[,] typeMatchupTable = new float[17, 17]
    {
        { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        { 2.0f, 1.0f, 0.5f, 0.5f, 1.0f, 2.0f, 0.5f, 0.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f },
        { 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f },
        { 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        { 1.0f, 1.0f, 0.0f, 2.0f, 1.0f, 2.0f, 0.5f, 1.0f, 2.0f, 2.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f },
        { 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f },
        { 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f },
        { 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f },
        { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f },
        { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f },
        { 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f },
        { 1.0f, 1.0f, 0.5f, 0.5f, 2.0f, 2.0f, 0.5f, 1.0f, 0.5f, 0.5f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f },
        { 1.0f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f },
        { 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.0f },
        { 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f },
        { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f },
        { 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f }
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

    private void InitSkillWindow()
    {
        if (wallManager == null || wallManager.player == null || wallManager.player.Length == 0 || wallManager.player[0] == null) return;

        Pokemon activePokemon = wallManager.player[0];
        currentSkills = activePokemon.Skills;
        totalSkillsCount = currentSkills != null ? currentSkills.Count : 0;

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
            yield return StartCoroutine(ExecuteSkillWithStatusCheck(enemyPoke, playerPoke, enemySkill, wallManager.PlayerHpSlider, true));

            if (playerPoke.nowHp <= 0 || enemyPoke.nowHp <= 0)
            {
                yield return StartCoroutine(HandleBattleEndFlow(playerPoke, enemyPoke));
                yield break;
            }

            yield return StartCoroutine(ExecuteSkillWithStatusCheck(playerPoke, enemyPoke, playerSkill, wallManager.EnemyHpSlider, false));
        }
        else
        {
            yield return StartCoroutine(ExecuteSkillWithStatusCheck(playerPoke, enemyPoke, playerSkill, wallManager.EnemyHpSlider, false));

            if (playerPoke.nowHp <= 0 || enemyPoke.nowHp <= 0)
            {
                yield return StartCoroutine(HandleBattleEndFlow(playerPoke, enemyPoke));
                yield break;
            }

            yield return StartCoroutine(ExecuteSkillWithStatusCheck(enemyPoke, playerPoke, enemySkill, wallManager.PlayerHpSlider, true));
        }

        if (playerPoke.nowHp > 0 && enemyPoke.nowHp > 0)
        {
            yield return StartCoroutine(ProcessTurnEndStatusEffects(playerPoke, wallManager.PlayerHpSlider, true));
            yield return StartCoroutine(ProcessTurnEndStatusEffects(enemyPoke, wallManager.EnemyHpSlider, false));
        }

        yield return new WaitForSeconds(0.3f);

        if (playerPoke.nowHp <= 0 || enemyPoke.nowHp <= 0)
        {
            yield return StartCoroutine(HandleBattleEndFlow(playerPoke, enemyPoke));
            yield break;
        }

        isAttacking = false;

        if (wallManager != null)
        {
            wallManager.isSkillWindowOpen = false;
            wallManager.UpdateBattleUI();
        }

        if (mainMenuContainer != null) mainMenuContainer.SetActive(true);
        if (battleLogText != null) battleLogText.text = "어떤 행동을 할까?";

        this.gameObject.SetActive(false);
    }

    private IEnumerator HandleBattleEndFlow(Pokemon playerPoke, Pokemon enemyPoke)
    {
        if (playerPoke.nowHp <= 0)
        {
            if (battleLogText != null) battleLogText.text = $"{playerPoke.pokemonName}은(는) 쓰러졌다...";
        }
        else if (enemyPoke.nowHp <= 0)
        {
            if (battleLogText != null) battleLogText.text = "상대 포켓몬을 쓰러뜨렸다!";
        }

        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
    }

    private IEnumerator ExecuteSkillWithStatusCheck(Pokemon attacker, Pokemon defender, Skill skill, Slider targetHpSlider, bool isTargetPlayer)
    {
        if (attacker.currentStatus == Status.Sleep)
        {
            attacker.statusTurnCount--;
            if (attacker.statusTurnCount <= 0)
            {
                attacker.CureStatus();
                battleLogText.text = $"{attacker.pokemonName}은(는) 잠에서 깨어났다!";
                yield return new WaitForSeconds(1.3f);
            }
            else
            {
                battleLogText.text = $"{attacker.pokemonName}은(는) 깊은 잠에 빠져있다...";
                yield return new WaitForSeconds(1.3f);
                yield break;
            }
        }

        if (attacker.currentStatus == Status.Freeze)
        {
            if (Random.Range(0, 100) < 20)
            {
                attacker.CureStatus();
                battleLogText.text = $"{attacker.pokemonName}의 얼음이 풀렸다!";
                yield return new WaitForSeconds(1.3f);
            }
            else
            {
                battleLogText.text = $"{attacker.pokemonName}은(는) 얼어서 움직일 수 없다!";
                yield return new WaitForSeconds(1.3f);
                yield break;
            }
        }

        if (attacker.currentStatus == Status.Paralysis && Random.Range(0, 100) < 25)
        {
            battleLogText.text = $"{attacker.pokemonName}은(는) 몸이 마비되어 움직일 수 없다!";
            yield return new WaitForSeconds(1.3f);
            yield break;
        }

        if (attacker.currentStatus == Status.Confusion)
        {
            attacker.statusTurnCount--;
            if (attacker.statusTurnCount <= 0)
            {
                attacker.CureStatus();
                battleLogText.text = $"{attacker.pokemonName}의 혼란이 풀렸다!";
                yield return new WaitForSeconds(1.3f);
            }
            else
            {
                battleLogText.text = $"{attacker.pokemonName}은(는) 혼란에 빠져있다!";
                yield return new WaitForSeconds(1.3f);

                if (Random.Range(0, 100) < 50)
                {
                    battleLogText.text = "혼란스러워 자신을 공격했다!";
                    yield return new WaitForSeconds(1.3f);

                    int selfDamage = Mathf.RoundToInt(((2 * attacker.level / 5 + 2) * 40 * attacker.attack / Mathf.Max(1, attacker.defense) / 50 + 2));
                    attacker.nowHp = Mathf.Max(0, attacker.nowHp - selfDamage);

                    // 자해 대상에 따른 Slider 매칭 버그 수정 (isTargetPlayer 기반이 아님)
                    Slider selfSlider = isTargetPlayer ? wallManager.EnemyHpSlider : wallManager.PlayerHpSlider;
                    yield return StartCoroutine(UpdateHPSliderRoutine(selfSlider, attacker, !isTargetPlayer));
                    yield break;
                }
            }
        }

        yield return StartCoroutine(ExecuteSkillAction(attacker, defender, skill, targetHpSlider, isTargetPlayer));
    }

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

            int targetDefense = Mathf.Max(1, defender.defense);
            int damage = Mathf.RoundToInt(((2 * attacker.level / 5 + 2) * skill.power * attacker.attack / targetDefense / 50 + 2) * typeMultiplier);

            defender.nowHp = Mathf.Max(0, defender.nowHp - damage);

            yield return StartCoroutine(UpdateHPSliderRoutine(targetHpSlider, defender, isTargetPlayer));

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

        if (skill.effectStatus != Status.None && defender.nowHp > 0)
        {
            if (Random.Range(0, 100) < skill.statusChance)
            {
                if (defender.ApplyStatus(skill.effectStatus))
                {
                    string statusMsg = "";
                    switch (skill.effectStatus)
                    {
                        case Status.Paralysis: statusMsg = $"{defender.pokemonName}은(는) 마비되어 저려왔다!"; break;
                        case Status.Burn: statusMsg = $"{defender.pokemonName}은(는) 화상을 입었다!"; break;
                        case Status.Poison: statusMsg = $"{defender.pokemonName}은(는) 독에 걸렸다!"; break;
                        case Status.Sleep: statusMsg = $"{defender.pokemonName}은(는) 깊은 잠에 빠졌다!"; break;
                        case Status.Freeze: statusMsg = $"{defender.pokemonName}은(는) 꽁꽁 얼어붙었다!"; break;
                        case Status.Confusion: statusMsg = $"{defender.pokemonName}은(는) 혼란에 빠졌다!"; break;
                    }
                    battleLogText.text = statusMsg;
                    wallManager.UpdateBattleUI();
                    yield return new WaitForSeconds(1.3f);
                }
            }
        }
    }

    private IEnumerator ProcessTurnEndStatusEffects(Pokemon pokemon, Slider hpSlider, bool isPlayer)
    {
        if (pokemon.currentStatus == Status.Poison || pokemon.currentStatus == Status.Burn)
        {
            int dotDamage = Mathf.Max(1, pokemon.maxHp / 8);
            pokemon.nowHp = Mathf.Max(0, pokemon.nowHp - dotDamage);

            if (pokemon.currentStatus == Status.Poison)
                battleLogText.text = $"{pokemon.pokemonName}은(는) 독의 데미지를 입고 있다!";
            else
                battleLogText.text = $"{pokemon.pokemonName}은(는) 화상의 데미지를 입고 있다!";

            yield return new WaitForSeconds(1.3f);
            yield return StartCoroutine(UpdateHPSliderRoutine(hpSlider, pokemon, !isPlayer)); // target이 플레이어인지 여부 반전 맞춤
        }
    }

    private IEnumerator UpdateHPSliderRoutine(Slider targetHpSlider, Pokemon targetPokemon, bool isTargetPlayer)
    {
        if (targetHpSlider != null)
        {
            float targetValue = Mathf.Clamp01((float)targetPokemon.nowHp / targetPokemon.maxHp);
            float startValue = targetHpSlider.value;
            float elapsedTime = 0f;
            float duration = 0.5f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float currentProgress = Mathf.Clamp01(elapsedTime / duration);

                targetHpSlider.value = Mathf.Lerp(startValue, targetValue, currentProgress);

                if (isTargetPlayer)
                {
                    int visualHp = Mathf.RoundToInt(Mathf.Lerp(targetPokemon.maxHp * startValue, targetPokemon.nowHp, currentProgress));
                    // 전체 UI를 덮어씌우는 대신 플레이어 텍스트만 실시간으로 안전하게 갱신하도록 우회
                    wallManager.UpdatePlayerHpTextRealtime(visualHp);
                }
                yield return null;
            }
            targetHpSlider.value = targetValue;
            wallManager.UpdateBattleUI(); // 연출이 완전히 종료된 후 최종 UI 정보 전체 동기화
        }
        yield return new WaitForSeconds(0.2f);
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
        // 2진수 문자열 변환 방식 대신 안전하고 직관적인 정수 캐스팅 인덱싱 적용
        int value = (int)type;
        if (value < 0 || value >= 17) return 0;
        return value;
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