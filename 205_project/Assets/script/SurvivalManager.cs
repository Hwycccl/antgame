// ������: SurvivalManager.cs (��Ϸ��������ǿ����)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ��������Ϸ��������ս������ʳ�����ĺ����������֡�
/// </summary>
public class SurvivalManager : MonoBehaviour
{
    
    public static SurvivalManager Instance { get; private set; }

    [Header("��ҹ��ʱ������")]
    [Tooltip("��Ϸ��һ���Ӧ��ʵ����Ķ�����")]
    [SerializeField] private float secondsPerDay = 60f;

    [Header("ʳ����������")]
    [Tooltip("ÿֻ����ÿ�����Ķ���ʳ��")]
    [SerializeField] private int foodConsumptionPerAnt = 1;
    [Tooltip("�����'ʳ��'�������ݣ�����'���'���ϵ�����")]
    [SerializeField] private ResourceBasicData foodCardData; // ����ʶ��ʳ�￨

    [Header("�����¼�����")]
    [Tooltip("��һ�����ַ����ڵڼ��죿")]
    [SerializeField] private int firstInvasionDay = 5;
    [Tooltip("ÿ�������췢��һ������")]
    [SerializeField] private int invasionIntervalDays = 5;
    [Tooltip("��������Ļ���ʿ����")]
    [SerializeField] private int baseSoldiersRequired = 3;
    [Tooltip("ÿ�����ֺ󣬶�����Ҫ���Ӷ���ʿ��")]
    [SerializeField] private int soldiersRequiredIncrease = 2;
    [Tooltip("�����'ʿ����'���������ϵ�����")]
    [SerializeField] private AntBasicData soldierAntCardData; // ����ʶ��ʿ����

    // --- �ڲ�״̬���� ---
    private int currentDay = 0;
    private float dayTimer = 0f;
    private int nextInvasionDay;
    private int invasionCount = 0;
    private bool isGameOver = false;

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
        // ȷ��ȫ��ʱ��������Ѿ�����
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.StartOrResumeTimer();
        }
        else
        {
            Debug.LogError("������ȱ�� GlobalTimeManager!");
            this.enabled = false; // ���û��ʱ�������������ô˽ű�
            return;
        }

        nextInvasionDay = firstInvasionDay;
        Debug.Log("�����������������");
    }

    private void Update()
    {
        Debug.Log("SurvivalManager Update is running...");
        if (isGameOver || GlobalTimeManager.Instance.IsPaused)
        {
            return;
        }

        dayTimer += Time.deltaTime;

        // һ�����
        if (dayTimer >= secondsPerDay)
        {
            dayTimer -= secondsPerDay;
            currentDay++;
            Debug.Log($"--- �� {currentDay} ����� ---");

            // 1. ִ��ÿ��ʳ����
            CheckFoodSupply();
            if (isGameOver) return; // �����Ϸ�򼢶���������ֹͣ�������

            // 2. �������Ƿ���������
            if (currentDay >= nextInvasionDay)
            {
                CheckInvasion();
                // ������һ�����ֵ�����
                nextInvasionDay += invasionIntervalDays;
            }
        }
    }

    /// <summary>
    /// ���ʳ�﹩Ӧ�Ƿ����
    /// </summary>
    private void CheckFoodSupply()
    {
        // �ҳ����������е����Ϻ�ʳ�￨
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

        int antPopulation = allCards.Count(c => c.CardData.cardType == CardsBasicData.CardType.Ant);
        // ֻ����ʳ�￨������Դֵ
        int totalFoodAvailable = allCards
            .Where(c => c.CardData == foodCardData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        int foodNeeded = antPopulation * foodConsumptionPerAnt;

        Debug.Log($"��������: {antPopulation}����Ҫʳ��: {foodNeeded}����ǰʳ�ﴢ��: {totalFoodAvailable}");

        if (totalFoodAvailable < foodNeeded)
        {
            // ʳ�ﲻ�㣬��Ϸ����
            GameOver($"��Ϸ���������ģ�\n�����Ⱥ��Ҫ {foodNeeded} ʳ���ֻ�� {totalFoodAvailable}��");
        }
        else
        {
            // ʳ����㣬����ʳ��
            Debug.Log("ʳ����㣬��ʼ����...");
            ConsumeFood(foodNeeded, allCards);
        }
    }

    /// <summary>
    /// �ӳ������Ƴ�ָ��������ʳ�￨��
    /// </summary>
    private void ConsumeFood(int amountToConsume, Card[] allCards)
    {
        // ɸѡ������ʳ�￨�ƣ���������Դ��ֵ�ӵ͵��������������ļ�ֵ�͵�
        List<Card> foodCards = allCards
            .Where(c => c.CardData == foodCardData)
            .OrderBy(c => (c.CardData as ResourceBasicData).resourceValue)
            .ToList();

        int consumedAmount = 0;
        foreach (var foodCard in foodCards)
        {
            if (consumedAmount >= amountToConsume)
                break;

            int cardValue = (foodCard.CardData as ResourceBasicData).resourceValue;
            consumedAmount += cardValue;

            Debug.Log($"�����˿��� '{foodCard.name}' (��ֵ: {cardValue})��");
            Destroy(foodCard.gameObject);
        }
        Debug.Log($"�ܹ������� {consumedAmount} ��ʳ�");
    }


    /// <summary>
    /// ��������¼�
    /// </summary>
    private void CheckInvasion()
    {
        invasionCount++;
        int requiredSoldiers = baseSoldiersRequired + (invasionCount - 1) * soldiersRequiredIncrease;

        Debug.Log($"!!! ���־��� (�� {invasionCount} ��) !!! ��Ҫ {requiredSoldiers} ��ʿ���Ͻ��з�����");

        // ͳ�Ƶ�ǰӵ�е�ʿ��������
        int currentSoldiers = FindObjectsByType<Card>(FindObjectsSortMode.None)
            .Count(c => c.CardData == soldierAntCardData);

        Debug.Log($"��ǰʿ��������: {currentSoldiers}");

        if (currentSoldiers < requiredSoldiers)
        {
            // ʿ�����㣬��Ϸ����
            GameOver($"��Ϸ����������ʧ�ܣ�\n����Ҫ {requiredSoldiers} ��ʿ�������������֣���ֻ�� {currentSoldiers} ����");
        }
        else
        {
            // �����ɹ�
            Debug.Log("�����ɹ�����Ⱥ�Ҵ���������");
            // ��δ���������ʿ���������߼���
        }
    }

    /// <summary>
    /// ��Ϸ��������
    /// </summary>
    /// <param name="reason">��Ϸ������ԭ��</param>
    private void GameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.LogError("==========================================");
        Debug.LogError(reason);
        Debug.LogError("==========================================");

        // ��ͣȫ��ʱ��
        if (GlobalTimeManager.Instance != null)
        {
            GlobalTimeManager.Instance.PauseTimer();
        }

        // --- �����޸ĵ㣺������Ϸ ---
        Debug.LogWarning("��Ϸ���������ڹرճ���...");

        // �����Unity�༭�������У���ֹͣ����ģʽ
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ����Ǳ�������Ϸ����رճ���
        Application.Quit();
#endif
    }
}