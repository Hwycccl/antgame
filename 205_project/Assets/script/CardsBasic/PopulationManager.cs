// �ļ�·��: Assets/script/PopulationManager.cs
using UnityEngine;
using TMPro; // ��Ҫ���� TextMeshPro �������ռ�
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    [Header("UI���")]
    [Tooltip("������ʾ�˿�״̬��TextMeshPro�ı�")]
    public TextMeshProUGUI populationText;

    // --- �������� ---
    public int CurrentPopulation { get; private set; }
    public int MaxPopulation { get; private set; }

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

    private void Start()
    {
        // ��Ϸ��ʼʱ����������һ���˿�����
        UpdatePopulationStats();
    }

    // ���ڸ��£��Է������ϱ�������ʽ����
    private void Update()
    {
        // Ϊ�����ܿ��ǣ����Բ���ÿ֡�����£����Ը�Ϊһ����ʱ��
        // �����������Ŀ��ÿ֡����һ�����ⲻ��
        UpdatePopulationStats();
    }

    /// <summary>
    /// ���ĺ��������㵱ǰ�˿ں�����˿ڣ�������UI
    /// </summary>
    public void UpdatePopulationStats()
    {
        // 1. ���ҳ��������еĿ���
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

        // 2. ���㵱ǰ�˿� (��������ΪAnt�Ŀ���)
        // �������Ǽ����׳�(Larva)��cardTypeҲ����Ϊ��Ant
        CurrentPopulation = allCards.Count(c => c.CardData != null && c.CardData.cardType == CardsBasicData.CardType.Ant);

        // 3. ��������˿� (������Ѩ�ṩ���˿��ܺ�)
        MaxPopulation = 0;
        var antNests = allCards.Where(c => c.CardData is AntNestData);
        foreach (var nestCard in antNests)
        {
            AntNestData nestData = nestCard.CardData as AntNestData;
            if (nestData != null)
            {
                MaxPopulation += nestData.populationCapacity;
            }
        }

        // ���һ����Ѩ��û�У����Ը�һ��Ĭ��ֵ�����߾�Ϊ0
        if (MaxPopulation == 0)
        {
            // ���磬��Ϸ��ʼʱĬ�ϸ�2��λ��
            // MaxPopulation = 2; 
        }

        // 4. ����UI��ʾ
        UpdatePopulationUI();
    }

    /// <summary>
    /// �����Ⱥ�Ƿ�����
    /// </summary>
    public bool IsPopulationFull()
    {
        return CurrentPopulation >= MaxPopulation;
    }

    /// <summary>
    /// ����UI�ı�
    /// </summary>
    private void UpdatePopulationUI()
    {
        if (populationText != null)
        {
            populationText.text = $"AntNumber: {CurrentPopulation} / {MaxPopulation}";

            // ����˿����ˣ����Ըı���ɫ��ʾ����
            if (IsPopulationFull())
            {
                populationText.color = Color.red;
            }
            else
            {
                populationText.color = Color.white;
            }
        }
    }
}