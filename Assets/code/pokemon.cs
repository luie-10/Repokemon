using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pokemon", menuName = "Pokemon/Pokemon Data")]
public class Pokemon : ScriptableObject // 클래스 첫 글자는 대문자 관례
{
    [Header("기본 정보")]
    public string pokemonName;
    public int level = 1;

    // 이전에 만든 PokemonType(또는 physics) Enum 활용
    public PokemonType type1;
    public PokemonType type2; // 듀얼 타입 지원용 (없으면 None 혹은 노말)

    [Header("체력 및 경험치")]
    public int maxHp;
    public int nowHp;
    public int currentExp;
    public int nextLevelExp; // 다음 레벨까지 필요한 경험치 (EXLeft 대치)

    [Header("기본 능력치 (기본 스탯)")]
    public int attack;
    public int defense;
    public int spAtk;
    public int spDef;
    public int speed;

    [Header("배운 기술 목록 (최대 4개)")]
    // [SerializeField]를 붙여야 인스펙터에 노출되고 저장됩니다.
    [SerializeField] private List<Skill> skills = new List<Skill>();

    // 외부에서 기술 목록에 안전하게 접근할 수 있도록 프로퍼티 제공
    public List<Skill> Skills => skills;

    /// <summary>
    /// 새로운 기술을 배울 때 사용하는 메서드 (최대 4개 제한 부가 로직 가능)
    /// </summary>
    public bool LearnSkill(Skill newSkill)
    {
        if (skills.Count >= 4)
        {
            Debug.Log($"{pokemonName}은(는) 이미 기술을 4개 모두 배우고 있습니다.");
            return false;
        }

        skills.Add(newSkill);
        return true;
    }
}
