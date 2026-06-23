using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pokemon", menuName = "Pokemon/Pokemon Data")]
public class Pokemon : ScriptableObject
{
    [Header("기본 정보")]
    public string pokemonName;
    public int level = 1;
    public PokemonType type1;
    public PokemonType type2;

    [Header("체력 및 경험치")]
    public int maxHp;
    public int nowHp;
    public int currentExp;
    public int nextLevelExp;

    [Header("기본 능력치")]
    public int attack;
    public int defense;
    [HideInInspector] public int currentDefense => (currentStatus == Status.Burn) ? Mathf.Max(1, defense / 2) : defense; // 화상 시 공격력/방어력 디버프 구현용 (포켓몬 본가 룰에 맞게 커스텀 가능)
    public int spAtk;
    public int spDef;
    public int speed;
    public int currentSpeed => (currentStatus == Status.Paralysis) ? Mathf.Max(1, speed / 2) : speed; // 마비 시 스피드 절반

    [Header("배운 기술 목록")]
    [SerializeField] private List<Skill> skills = new List<Skill>();
    public List<Skill> Skills => skills;

    [Header("현재 상태이상 정보 (런타임 관리)")]
    public Status currentStatus = Status.None;
    public int statusTurnCount = 0; // 수면, 혼란 등 지속 턴수가 필요한 상태이상용

    /// <summary>
    /// 포켓몬에게 상태이상을 부여합니다.
    /// </summary>
    public bool ApplyStatus(Status newStatus)
    {
        // 이미 상태이상이 있다면 중복 처리 방지 (본가 룰: 주요 상태이상은 하나만 걸림)
        if (currentStatus != Status.None && newStatus != Status.None) return false;

        currentStatus = newStatus;

        // 상태이상별 특수 턴수 초기화
        if (currentStatus == Status.Sleep) statusTurnCount = Random.Range(1, 4); // 1~3턴 수면
        if (currentStatus == Status.Confusion) statusTurnCount = Random.Range(1, 5); // 1~4턴 혼란

        return true;
    }

    /// <summary>
    /// 상태이상을 치료합니다.
    /// </summary>
    public void CureStatus()
    {
        currentStatus = Status.None;
        statusTurnCount = 0;
    }

    public bool LearnSkill(Skill newSkill)
    {
        if (skills.Count >= 4) return false;
        skills.Add(newSkill);
        return true;
    }
}