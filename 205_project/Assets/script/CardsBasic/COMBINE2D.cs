// COMBINE2D.cs (��K���ܰ�)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(STACK2D))]
public class COMBINE2D : MonoBehaviour
{
    [Header("�M��Ҏ�t������")]
    [Tooltip("Ո�����x�����кϳ��䷽�� ScriptableObject �ϵ���̎")]
    [SerializeField] private CardsCombination combinationDatabase;

    private STACK2D stackScript;

    private void Awake()
    {
        stackScript = GetComponent<STACK2D>();
    }

    /// <summary>
    /// �Lԇ��Ŀ�˶ѯB�M�кϳəz�� (�� CardsBehaviour �{��)
    /// </summary>
    /// <param name="targetStack">Ҫ�M�кϳəz���Ŀ�˶ѯB�ĸ�����</param>
    public void TryToCombineWithNearbyCards(STACK2D targetStack)
    {
        if (combinationDatabase == null)
        {
            Debug.LogError("�M��Ҏ�t������ (Combination Database) δ�O�ã�");
            return;
        }

        // 1. �@ȡĿ�˶ѯB�����еĿ��Ɣ���
        List<CardsBasicData> inputCardsData = targetStack.GetCardsDataInStack();

        // 2. �ڔ������в����Ƿ���ƥ��ĽM��Ҏ�t
        CardsCombinationRule matchedRule = combinationDatabase.GetCombination(inputCardsData);

        // 3. ����ҵ���ƥ���Ҏ�t���t���кϳ�
        if (matchedRule != null)
        {
            Debug.Log("�ɹ�ƥ��M��Ҏ�t: " + matchedRule.combinationName);
            ExecuteCombination(targetStack, matchedRule);
        }
    }

    /// <summary>
    /// ���кϳ��^�̣��N��ԭ�ϣ����ɮa��
    /// </summary>
    private void ExecuteCombination(STACK2D rootStack, CardsCombinationRule rule)
    {
        // --- ̎��ԭ�� ---
        List<STACK2D> allStacksInGroup = new List<STACK2D>();
        CollectStacksRecursively(rootStack, allStacksInGroup);
        allStacksInGroup.Reverse(); // ���Ӽ��_ʼ̎�����⸸���ȱ��N��

        foreach (var requiredGroup in rule.requiredCards)
        {
            if (requiredGroup.destroyOnCombine)
            {
                int countToDestroy = requiredGroup.requiredCount;
                // ������ǰ��v����ȫ�؄h��
                for (int i = allStacksInGroup.Count - 1; i >= 0; i--)
                {
                    if (countToDestroy <= 0) break;

                    STACK2D currentStack = allStacksInGroup[i];
                    CardsBasicData cardData = currentStack.GetComponent<CardsBehaviour>().GetCardData();

                    bool matchesSpecific = requiredGroup.specificCard != null && requiredGroup.specificCard == cardData;
                    bool matchesType = requiredGroup.specificCard == null && requiredGroup.cardType == cardData.cardType;

                    if (matchesSpecific || matchesType)
                    {
                        // ��߉݋�������͈������Ƴ�����
                        CardsManager.Instance.RemoveCardFromLogic(cardData);
                        Destroy(currentStack.gameObject);
                        countToDestroy--;
                    }
                }
            }
        }

        // --- ̎��a�� ---
        Vector3 spawnPosition = rootStack.transform.position; // �ڸ����Ƶ�λ������
        foreach (var resultCardInfo in rule.results)
        {
            // ���]���əC��
            if (Random.value <= resultCardInfo.probability)
            {
                for (int i = 0; i < resultCardInfo.quantity; i++)
                {
                    // ��΢�e�_λ�ã�������ȫ�دB
                    Vector3 offset = new Vector3(Random.Range(-0.1f, 0.1f), 0, 0);

                    // ֪ͨ߉݋��UI�����¿�
                    CardsManager.Instance.AddCardToLogic(resultCardInfo.resultCard);
                    HandUI.Instance.AddCardToView(resultCardInfo.resultCard, spawnPosition + offset);
                }
            }
        }
    }

    // �o���������fޒ�ռ��ѯB�е����� STACK2D �M��
    private void CollectStacksRecursively(STACK2D stack, List<STACK2D> stackList)
    {
        if (stack == null || stackList.Contains(stack)) return;

        stackList.Add(stack);
        foreach (var child in stack.ChildStacks)
        {
            CollectStacksRecursively(child, stackList);
        }
    }
}