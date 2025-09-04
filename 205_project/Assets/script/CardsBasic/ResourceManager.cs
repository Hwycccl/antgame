// �ļ�·��: Assets/script/ResourceManager.cs (�Ѹ���)
using UnityEngine;
using TMPro; // ���� TextMeshPro �����ռ�
using System.Linq;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("UI ��� (�ӳ��� Hierarchy ����)")]
    public TextMeshProUGUI leafText;
    public TextMeshProUGUI fungusText;

    [Header("��Դ�������� (����Ŀ�ļ�������)")]
    [Tooltip("���'Ҷ��'��Դ���� ScriptableObject (Leaf.asset)")]
    public ResourceBasicData leafData; // ��ƬҶ��
    [Tooltip("���'��Ҷ'��Դ���� ScriptableObject (LeafFragment.asset)")]
    public ResourceBasicData leafFragmentData; // ��Ҷ
    [Tooltip("���'���'��Դ���� ScriptableObject (Fungus.asset)")]
    public ResourceBasicData fungusData;

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

    private void Update()
    {
        UpdateResourceCounts();
    }

    /// <summary>
    /// ���ĺ���������������Դ�ĵ�ǰ����
    /// </summary>
    public void UpdateResourceCounts()
    {
        var allCards = FindObjectsByType<Card>(FindObjectsSortMode.None);

        // 1. ����������Ҷ���ܼ�ֵ
        int leafFragmentValue = allCards
            .Where(c => c.CardData == leafFragmentData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        // 2. ������������Ҷ�ӵ��ܼ�ֵ
        int leafValue = allCards
            .Where(c => c.CardData == leafData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        // 3. ���� 1Ҷ��=2��Ҷ �Ĺ���ϲ����㣬����ʾ
        int totalCombinedLeafValue = leafFragmentValue + (leafValue * 2);


        // 4. ����������Դ (�߼�����)
        int totalFungusValue = allCards
            .Where(c => c.CardData == fungusData)
            .Sum(c => (c.CardData as ResourceBasicData)?.resourceValue ?? 0);

        // �����ռ��������µ�UI��
        UpdateResourceUI(totalCombinedLeafValue, totalFungusValue);
    }

    /// <summary>
    /// ����Դ������ʾ�ڶ�Ӧ��UI�ı���
    /// </summary>
    private void UpdateResourceUI(int leaf, int fungus)
    {
        // ������ʾ�� leaf �Ѿ��Ǻϲ�����ܼ�ֵ��
        if (leafText != null) leafText.text = leaf.ToString();
        if (fungusText != null) fungusText.text = fungus.ToString();
    }
}