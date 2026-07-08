using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleStart : MonoBehaviour
{
    [Header("오브젝트 참조")]
    public GameObject playerPokemon;
    public GameObject Player;
    public AudioSource battleMusic;

    [Header("연출 설정")]
    [SerializeField] private float playerFadeDuration = 1.0f;
    [SerializeField] private Vector3 playerMoveDirection = new Vector3(-1f, -1f, 0f);
    [SerializeField] private float playerMoveSpeed = 5f;

    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float blinkInterval = 0.15f;

    private Image pokemonImage;

    void Start()
    {
        playerPokemon.SetActive(false);
        Player.SetActive(true);

        pokemonImage = playerPokemon.GetComponentInChildren<Image>();

        if (Player.GetComponent<Animator>() != null)
        {
            Player.GetComponent<Animator>().SetTrigger("BattleStart");
        }

        if (battleMusic != null && !battleMusic.isPlaying)
        {
            battleMusic.Play();
        }

        StartCoroutine(BattleStartSequence());
    }

    private IEnumerator BattleStartSequence()
    {
        yield return StartCoroutine(MovePlayerAway());

        Player.SetActive(false);

        yield return StartCoroutine(SpawnAndBlinkPokemon());
    }

    private IEnumerator MovePlayerAway()
    {
        float elapsedTime = 0f;
        Vector3 direction = playerMoveDirection.normalized;

        while (elapsedTime < playerFadeDuration)
        {
            Player.transform.Translate(direction * playerMoveSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SpawnAndBlinkPokemon()
    {
        playerPokemon.SetActive(true);

        if (pokemonImage != null)
        {
            Color originalColor = pokemonImage.color;

            for (int i = 0; i < blinkCount; i++)
            {
                pokemonImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
                yield return new WaitForSeconds(blinkInterval);

                pokemonImage.color = originalColor;
                yield return new WaitForSeconds(blinkInterval);
            }

            pokemonImage.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(blinkCount * blinkInterval * 2);
        }

        Debug.Log("배틀 등장 연출 완료! 이제 커맨드 UI를 조작할 수 있습니다.");
    }
}
