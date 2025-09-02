// �����: CardCombiner.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardCombiner : MonoBehaviour
{
    [Header("�M�ϔ�����")]
    [Tooltip("�������кϳ��䷽�� ScriptableObject")]
    [SerializeField] private CardsCombination combinationDatabase;
    [SerializeField] private Vector3 spawnOffset = new Vector3(-2f, 0, 0); // ����߅�����¿�

    private Card card;
    private Coroutine combinationCoroutine;

    private void Awake()
    {
        card = GetComponent<Card>();
    }

    // �� CardStacker �ڶѯB��������
    public void CheckForCombination()
    {
        // ֻ�жѯB�ĸ����Ʋ�ؓ؟�z��͈��кϳ�
        if (card.Stacker.Parent != null) return;

        // ������ںϳ��У��t���M���µęz�y
        if (combinationCoroutine != null) return;

        List<Card> stackCards = card.Stacker.GetCardsInStack();
        List<CardsBasicData> inputData = stackCards.Select(c => c.CardData).ToList();

        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputData);

        if (matchedRule != null)
        {
            // �ҵ����䷽���_ʼӋ�r�ϳ�
            combinationCoroutine = StartCoroutine(CombinationProcess(matchedRule, stackCards));
        }
    }

    private IEnumerator CombinationProcess(CardsCombinationRule rule, List<Card> ingredientCards)
    {
        Debug.Log($"�ҵ��M��: {rule.combinationName}���_ʼӋ�r {rule.time} �롣");

        // �������@�e����һ���M�ȗlUI
        // ...

        yield return new WaitForSeconds(rule.time);

        Debug.Log("�ϳ���ɣ�");

        // 1. ���ɮa��
        Vector3 rootPosition = transform.position;
        foreach (var result in rule.results)
        {
            if (Random.value <= result.probability) // ���]�C��
            {
                for (int i = 0; i < result.quantity; i++)
                {
                    CardSpawner.Instance.SpawnCard(result.resultCard, rootPosition + spawnOffset);
                }
            }
        }

        // 2. �N��ԭ�� (���Ӽ��_ʼ�N����������e)
        List<Card> cardsToDestroy = new List<Card>();
        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                // �ҳ���Ҫ���N���Ŀ��ƌ���
                var matchingCards = ingredientCards
                    .Where(c => c.CardData == requiredGroup.specificCard || c.CardData.cardType == requiredGroup.cardType)
                    .Take(requiredGroup.requiredCount);
                cardsToDestroy.AddRange(matchingCards);
            }
        }

        // �����N��
        foreach (var cardToDestroy in cardsToDestroy.Distinct().Reverse())
        {
            Destroy(cardToDestroy.gameObject);
        }

        combinationCoroutine = null;
    }
}
