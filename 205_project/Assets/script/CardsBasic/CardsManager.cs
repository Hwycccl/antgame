// CardsManager.cs 
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // ����Linq�Ա��ڲ�ѯ

/// <summary>
/// �����Ŀ��ƹ�����������׷�ٳ������п��Ʋ���̬������Դ��
/// ��Ϸ�߼��ĺ��ģ����ԣ���
/// </summary>
public class CardsManager : MonoBehaviour
{
    public static CardsManager Instance { get; private set; }

    // �Ƴ����ƿ�����ƶѣ�����ֻ׷�ٳ������еĿ�������
    [SerializeField] private List<CardsBasicData> cardsOnField = new List<CardsBasicData>();

    // ��������Դ��������UI�ű���ȡ
    public int FungusAmount { get; private set; }
    public int LeafFragmentAmount { get; private set; }
    public int FecesAmount { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // ��Ϸ��ʼʱ����һ�γ�ʼ��Դ
        CalculateResourceTotals();
    }

    /// <summary>
    /// ��һ�ſ��Ƶġ����ݡ���ӵ������߼���
    /// </summary>
    public void AddCardToLogic(CardsBasicData card)
    {
        if (card != null)
        {
            cardsOnField.Add(card);
            // ÿ�����������仯ʱ�������¼�������Դ
            CalculateResourceTotals();
        }
    }

    /// <summary>
    /// �ӳ����߼����Ƴ�һ�ſ��Ƶġ����ݡ�
    /// </summary>
    public void RemoveCardFromLogic(CardsBasicData card)
    {
        if (card != null && cardsOnField.Contains(card))
        {
            cardsOnField.Remove(card);
            // ÿ�����������仯ʱ�������¼�������Դ
            CalculateResourceTotals();
        }
    }

    /// <summary>
    /// ���Ĺ��ܣ������������п��ƣ�����ÿ����Դ��-�ܺ�
    /// </summary>
    private void CalculateResourceTotals()
    {
        // ������
        FungusAmount = 0;
        LeafFragmentAmount = 0;
        FecesAmount = 0;

        foreach (CardsBasicData card in cardsOnField)
        {
            // ������ſ��Ƿ�����Դ��
            if (card is ResourceBasicData resourceCard)
            {
                // ������Դ�����ۼ����ֵ
                switch (resourceCard.resourceType)
                {
                    case ResourceBasicData.ResourceType.Fungus:
                        FungusAmount += resourceCard.resourceValue;
                        break;
                    case ResourceBasicData.ResourceType.LeafFragment:
                        LeafFragmentAmount += resourceCard.resourceValue;
                        break;
                    case ResourceBasicData.ResourceType.Feces:
                        FecesAmount += resourceCard.resourceValue;
                        break;
                }
            }
        }

        // ���������ﴥ��һ���¼���֪ͨUI����
        // Ϊ�˼򵥣�������UI�ű���Update���Լ�����
        Debug.Log($"��Դ����: ���={FungusAmount}, ��Ҷ={LeafFragmentAmount}, ���={FecesAmount}");
    }
}