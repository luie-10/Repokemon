using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요

public class BattleStart : MonoBehaviour
{
    [Header("오브젝트 참조")]
    public GameObject playerPokemon; // 플레이어 포켓몬 프리팹 (또는 오브젝트)
    public GameObject Player; // 플레이어 게임 오브젝트
    public AudioSource battleMusic; // 배틀 음악 오디오 소스

    [Header("연출 설정")]
    [SerializeField] private float playerFadeDuration = 1.0f; // 플레이어가 사라지는 시간
    [SerializeField] private Vector3 playerMoveDirection = new Vector3(-1f, -1f, 0f); // 왼쪽 대각선 아래 방향
    [SerializeField] private float playerMoveSpeed = 5f; // 플레이어 이동 속도

    [SerializeField] private int blinkCount = 3; // 포켓몬 점멸 횟수
    [SerializeField] private float blinkInterval = 0.15f; // 점멸 간격 (초)

    private SpriteRenderer pokemonSpriteRenderer;
    // 만약 UI(Canvas) 환경이라면 아래 주석을 해제하고 사용하세요.
    // private UnityEngine.UI.Image pokemonImage;

    void Start()
    {
        // 초기 상태 설정
        playerPokemon.SetActive(false);
        Player.SetActive(true);

        // 컴포넌트 미리 가져오기 (2D Sprite 기준)
        pokemonSpriteRenderer = playerPokemon.GetComponentInChildren<SpriteRenderer>();

        // 애니메이션 트리거 실행
        if (Player.GetComponent<Animator>() != null)
        {
            Player.GetComponent<Animator>().SetTrigger("BattleStart");
        }

        // 음악 재생
        if (battleMusic != null && !battleMusic.isPlaying)
        {
            battleMusic.Play();
        }

        // 🎬 배틀 시작 연출 시퀀스 시작!
        StartCoroutine(BattleStartSequence());
    }

    /// <summary>
    /// 전체적인 배틀 시작 연출을 제어하는 메인 코루틴
    /// </summary>
    private IEnumerator BattleStartSequence()
    {
        // 1. 플레이어가 왼쪽 대각선 아래로 이동하면서 자연스럽게 퇴장
        yield return StartCoroutine(MovePlayerAway());

        // 2. 플레이어 오브젝트 비활성화 (화면에서 완전히 사라짐)
        Player.SetActive(false);

        // 3. 포켓몬 등장 및 점멸 연출
        yield return StartCoroutine(SpawnAndBlinkPokemon());
    }

    /// <summary>
    /// 플레이어를 왼쪽 대각선 아래로 이동시키는 코루틴
    /// </summary>
    private IEnumerator MovePlayerAway()
    {
        float elapsedTime = 0f;
        Vector3 direction = playerMoveDirection.normalized;

        while (elapsedTime < playerFadeDuration)
        {
            // 매 프레임 지정된 방향과 속도로 플레이어 이동
            Player.transform.Translate(direction * playerMoveSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 포켓몬을 활성화하고 하얀색/원래색으로 점멸시키는 코루틴
    /// </summary>
    private IEnumerator SpawnAndBlinkPokemon()
    {
        // 포켓몬 활성화
        playerPokemon.SetActive(true);

        if (pokemonSpriteRenderer != null)
        {
            Color originalColor = pokemonSpriteRenderer.color;

            // 정해진 횟수만큼 점멸 반복
            for (int i = 0; i < blinkCount; i++)
            {
                // ⚪ 하얀색 점멸 (또는 투명도를 이용한 깜빡임)
                // 완벽한 하얀색 실루엣을 원한다면 custom 셰이더가 필요하지만, 
                // 기본적으론 알파값(투명도)을 조절하거나 무채색을 입혀 표현합니다.
                pokemonSpriteRenderer.color = new Color(1f, 1f, 1f, 0.2f); // 깜빡이는 순간 (약간 투명한 흰색 느낌)
                yield return new WaitForSeconds(blinkInterval);

                // 🎨 원래 색상으로 복구
                pokemonSpriteRenderer.color = originalColor;
                yield return new WaitForSeconds(blinkInterval);
            }

            // 확실하게 원래 색상으로 초기화 보장
            pokemonSpriteRenderer.color = originalColor;
        }
        else
        {
            // SpriteRenderer를 찾지 못했을 때를 위한 예외 처리 (기본 활성화만 진행)
            yield return new WaitForSeconds(blinkCount * blinkInterval * 2);
        }

        Debug.Log("배틀 등장 연출 완료! 이제 커맨드 UI를 조작할 수 있습니다.");
    }
}