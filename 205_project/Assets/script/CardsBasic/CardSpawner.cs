// �����: CardSpawner.cs
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public static CardSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// �������Ɣ�����ָ��λ������һ���¿���
    /// </summary>
    /// <param name="cardData">Ҫ���ɵĿ��� ScriptableObject</param>
    /// <param name="position">����λ��</param>
    /// <returns>�������ɿ��Ƶ� Card �M��</returns>
    public Card SpawnCard(CardsBasicData cardData, Vector3 position)
    {
        if (cardData == null || cardData.cardPrefab == null)
        {
            Debug.LogError($"�o�����ɿ��ƣ�{cardData?.name} �Ĕ������A�u����գ�");
            return null;
        }

        // �������A�u��
        GameObject cardObject = Instantiate(cardData.cardPrefab, position, Quaternion.identity);
        cardObject.name = cardData.cardName; // �����ڈ������R�e

        // �@ȡ Card �M���K��ʼ��
        Card cardController = cardObject.GetComponent<Card>();
        if (cardController != null)
        {
            cardController.Initialize(cardData);
            UnlockedCardsManager.UnlockCard(cardData.cardName);
            return cardController;
        }
        else
        {
            Debug.LogError($"�����A�u�� {cardData.cardPrefab.name} �ϛ]�В��d Card �_����");
            Destroy(cardObject);
            return null;
        }
    }
}
