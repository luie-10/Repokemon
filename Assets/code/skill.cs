using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Skill", menuName = "Pokemon/Skill Data")]
public class Skill : ScriptableObject // 클래스 첫 글자는 대문자가 관례입니다.
{
    [Header("기본 정보")]
    public string skillName; // name 대치
    [TextArea(2, 5)]
    public string description;

    [Header("기술 속성")]
    public PokemonType engineeringType; // 기술의 속성 (physics 대신 직관적인 이름 추천)
    public int power;
    public int accuracy; // 명중률
    public int pp;       // 파워포인트
    public int priority; //   우선도

    [Header("부가 효과")]
    public Status effectStatus;         // 부여하는 상태이상
    public int statusChance;            // 상태이상 확률

    public Buff skillBuffs;             // 자신 혹은 아군 버프
    public Debuff skillDebuffs;         // 적 디버프
    
}

[Flags]
public enum PokemonType // 명확하게 속성(Type)으로 명칭 변경 권장
{
    [InspectorName("노말")] Normal = 1 << 0,
    [InspectorName("격투")] Fighting = 1 << 1,
    [InspectorName("비행")] Flying = 1 << 2,
    [InspectorName("독")] Poison = 1 << 3,
    [InspectorName("땅")] Ground = 1 << 4,
    [InspectorName("바위")] Rock = 1 << 5,
    [InspectorName("벌레")] Bug = 1 << 6,
    [InspectorName("고스트")] Ghost = 1 << 7,
    [InspectorName("강철")] Steel = 1 << 8,
    [InspectorName("불꽃")] Fire = 1 << 9,
    [InspectorName("물")] Water = 1 << 10,
    [InspectorName("풀")] Grass = 1 << 11,
    [InspectorName("전기")] Electric = 1 << 12,
    [InspectorName("에스퍼")] Psychic = 1 << 13,
    [InspectorName("얼음")] Ice = 1 << 14,
    [InspectorName("드래곤")] Dragon = 1 << 15,
    [InspectorName("악")] Dark = 1 << 16,
}

[Flags]
public enum Status
{
    [InspectorName("없음")] None = 0,
    [InspectorName("마비")] Paralysis = 1 << 0,
    [InspectorName("화상")] Burn = 1 << 1,
    [InspectorName("독")] Poison = 1 << 2,
    [InspectorName("수면")] Sleep = 1 << 3,
    [InspectorName("얼음")] Freeze = 1 << 4,
    [InspectorName("혼란")] Confusion = 1 << 5,
}

[Flags]
public enum Debuff
{
    [InspectorName("없음")] None = 0,
    [InspectorName("공격 하락")] AttackDown = 1 << 0,
    [InspectorName("방어 하락")] DefenseDown = 1 << 1,
    [InspectorName("특공 하락")] SpAtkDown = 1 << 2,
    [InspectorName("특방 하락")] SpDefDown = 1 << 3,
    [InspectorName("스피드 하락")] SpeedDown = 1 << 4,
    [InspectorName("명중 하락")] AccuracyDown = 1 << 5,
    [InspectorName("회피 하락")] EvasionDown = 1 << 6,
}

[Flags]
public enum Buff
{
    [InspectorName("없음")] None = 0,
    [InspectorName("공격 상승")] AttackUp = 1 << 0,
    [InspectorName("방어 상승")] DefenseUp = 1 << 1,
    [InspectorName("특공 상승")] SpAtkUp = 1 << 2,
    [InspectorName("특방 상승")] SpDefUp = 1 << 3,
    [InspectorName("스피드 상승")] SpeedUp = 1 << 4,
    [InspectorName("명중 상승")] AccuracyUp = 1 << 5,
    [InspectorName("회피 상승")] EvasionUp = 1 << 6,
    [InspectorName("급소율 상승")] CriticalUp = 1 << 7,
}
