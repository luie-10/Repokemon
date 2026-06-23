using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class StatusUIContainer : MonoBehaviour
{
    [System.Serializable]
    public struct StatusIconMapping
    {
        public Status statusType;
        public Sprite statusSprite;
    }

    [Header("상태이상 이미지 매핑 리스트")]
    [SerializeField] private List<StatusIconMapping> statusIcons;

    [Header("상태이상 UI 타겟 컴포넌트")]
    [SerializeField] private Image targetImageComponent; // 상태이상 아이콘이 표시될 UI Image 위치

    private Dictionary<Status, Sprite> iconDictionary;

    private void Awake()
    {
        // 검색 최적화를 위해 딕셔너리로 변환
        iconDictionary = new Dictionary<Status, Sprite>();
        foreach (var mapping in statusIcons)
        {
            if (!iconDictionary.ContainsKey(mapping.statusType))
            {
                iconDictionary.Add(mapping.statusType, mapping.statusSprite);
            }
        }

        // 초기에는 상태이상이 없으므로 비활성화
        if (targetImageComponent != null)
            targetImageComponent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 현재 상태이상에 맞춰 지정된 위치의 이미지를 변경하고 활성화/비활성화합니다.
    /// </summary>
    public void UpdateStatusUI(Status currentStatus)
    {
        if (targetImageComponent == null) return;

        if (currentStatus == Status.None)
        {
            targetImageComponent.gameObject.SetActive(false);
            return;
        }

        if (iconDictionary.TryGetValue(currentStatus, out Sprite statusSprite))
        {
            targetImageComponent.sprite = statusSprite;
            targetImageComponent.gameObject.SetActive(true);
        }
        else
        {
            // 해당하는 이미지가 등록되어있지 않은 경우 숨김
            targetImageComponent.gameObject.SetActive(false);
        }
    }
}