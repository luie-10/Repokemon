using UnityEngine;
using System.Collections;

public class PokemonFaintEffect : MonoBehaviour
{
    /// <summary>
    /// 포켓몬이 기절할 때 아래로 내려가며 점멸+페이드아웃 되는 연출 코루틴
    /// </summary>
    /// <param name="pokemonObject">쓰러질 포켓몬의 최상위 GameObject</param>
    /// <param name="duration">연출이 진행될 총 시간</param>
    public IEnumerator PlayFaintAnimation(GameObject pokemonObject, float duration = 1.5f)
    {
        if (pokemonObject == null) yield break;

        // 자식 오브젝트를 포함하여 SpriteRenderer를 찾습니다.
        SpriteRenderer spriteRenderer = pokemonObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // SpriteRenderer가 없으면 그냥 비활성화하고 종료
            pokemonObject.SetActive(false);
            yield break;
        }

        Vector3 startPosition = pokemonObject.transform.position;
        // 아래로 내려갈 목표 위치 (Y축으로 -2만큼 이동)
        Vector3 targetPosition = startPosition + new Vector3(0, -2f, 0);
        Color originalColor = spriteRenderer.color;

        float elapsedTime = 0f;
        float blinkInterval = 0.1f; // 깜빡임 간격
        float nextBlinkTime = 0f;
        bool isVisible = true;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 1. 아래로 부드럽게 하강 이동 (Lerp)
            pokemonObject.transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // 2. 투명도(Alpha)를 서서히 줄여서 흐려지게 만듦
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.Lerp(originalColor.a, 0f, progress);
            spriteRenderer.color = currentColor;

            // 3. 주기적으로 깜빡거리는(점멸) 효과 추가
            if (elapsedTime >= nextBlinkTime)
            {
                isVisible = !isVisible;
                // 깜빡일 때는 알파값을 극단적으로 낮추거나 원상복구
                currentColor.a = isVisible ? Mathf.Lerp(originalColor.a, 0f, progress) : 0f;
                spriteRenderer.color = currentColor;

                nextBlinkTime = elapsedTime + blinkInterval;
            }

            yield return null;
        }

        // 연출 완료 후 확실하게 투명화 및 오브젝트 비활성화
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;

        pokemonObject.SetActive(false);
        Debug.Log($"{pokemonObject.name}이(가) 쓰러졌습니다.");
    }
}