// �ļ���: CardSpawner.cs (�޸ĺ�)
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public static CardSpawner Instance { get; private set; }

    private void Awake()
    {
        // ... (����ģʽ���벻��) ...
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // --- �����޸� ---
    public Card SpawnCard(CardsBasicData cardData, Vector3 position)
    {
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogError($"�޷����ɿ��ƣ�{cardData?.name} �����ݻ�Ԥ�Ƽ�Ϊ�գ�");
            return null;
        }

        // �Ӷ���ػ�ȡ���ƣ�������Instantiate
        Card cardController = CardPool.Instance.Get(cardData.cardPrefab, position, Quaternion.identity);

        if (cardController != null)
        {
            cardController.Initialize(cardData);
            // --- ������������޸����롿 ---
            // ȷ�����ۺ�ʱ���ɿ��ƣ�����ק���ܶ��ǿ�����
            if (cardController.Dragger != null)
            {
                cardController.Dragger.enabled = true;
            }
            // --- ���޸���������� ---
            UnlockedCardsManager.UnlockCard(cardData.cardName);
            return cardController;
        }
        else
        {
            // �����ϣ����������߼���ȷ�����ﲻ��ִ��
            Debug.LogError($"����Ԥ�Ƽ� {cardData.cardPrefab.name} ��û�й��� Card �ű���");
            return null;
        }
    }
}