// �����: CardStacker.cs (�ޏ� MissingReferenceException ��)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CardStacker : MonoBehaviour
{
    [Header("�ѯB�O��")]
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float detectionRadius = 1.5f;

    public CardStacker Parent { get; private set; }
    private readonly List<CardStacker> children = new List<CardStacker>();

    private readonly List<CardStacker> nearbyTargets = new List<CardStacker>();

    private Card card;
    private CircleCollider2D triggerCollider;
    private Rigidbody2D rb;

    private const int SAFETY_LOOP_LIMIT = 100;

    private void Awake()
    {
        card = GetComponent<Card>();
        SetupComponents();
    }

    private void SetupComponents()
    {
        triggerCollider = GetComponent<CircleCollider2D>();
        if (triggerCollider == null) triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = detectionRadius;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<CardStacker>(out var target))
        {
            if (!nearbyTargets.Contains(target))
            {
                nearbyTargets.Add(target);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<CardStacker>(out var target))
        {
            nearbyTargets.Remove(target);
        }
    }

    public void OnBeginDrag()
    {
        if (Parent != null)
        {
            var oldRoot = GetRoot();
            DetachFromParent();
            oldRoot.UpdateStackVisuals();
        }
    }

    public void OnEndDrag()
    {
        CardStacker target = FindBestStackingTarget();
        if (target != null)
        {
            StackOn(target);
        }
        else
        {
            UpdateStackVisuals();
        }
    }

    private void DetachFromParent()
    {
        if (Parent == null) return;
        Parent.RemoveChild(this);
        Parent = null;
        transform.SetParent(null, worldPositionStays: true);
    }

    private void StackOn(CardStacker newParent)
    {
        if (newParent == this || IsDescendant(newParent))
        {
            Debug.LogError($"[{gameObject.name}] Invalid stack attempt prevented!");
            return;
        }

        DetachFromParent();
        transform.SetParent(newParent.transform);
        Parent = newParent;
        newParent.AddChild(this);

        var root = GetRoot();
        root.UpdateStackVisuals();
        root.card.Combiner.CheckForCombination();
    }
    // Ո���@���º�����ӵ���� CardStacker.cs �_����
    public void ForceStackOn(CardStacker newParent)
    {
        // �@���������Sϵ�y�����̎��ScoutingZone���|�l�ѯB��
        // ����������Ҳ����|�l��
        Debug.Log($"[{gameObject.name}] is being force-stacked onto [{newParent.name}] by the system.");
        StackOn(newParent);
    }
    private CardStacker FindBestStackingTarget()
    {
        return nearbyTargets
            .Where(t => t != null && t.gameObject.activeInHierarchy && !IsDescendant(t))
            .OrderBy(t => Vector2.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    public void UpdateStackVisuals(int depth = 0)
    {
        if (depth > SAFETY_LOOP_LIMIT) { Debug.LogError($"[{name}] UpdateStackVisuals recursion limit reached!"); return; }

        var myRenderer = card.GetArtworkRenderer();
        if (myRenderer == null) return;

        card.Dragger.SetOriginalSortingOrder(myRenderer.sortingOrder);

        // --- �����ޏ��c �_ʼ ---
        // �҂�������ǰ��v�б��@������Ƴ��˱��N�������������Ӱ����m�ı�v
        for (int i = children.Count - 1; i >= 0; i--)
        {
            CardStacker child = children[i];

            // ���Ӱ�ȫ�z�飺���������� null (�ѱ��N��)���͌������б����Ƴ��K���^
            if (child == null)
            {
                children.RemoveAt(i);
                continue;
            }

            // ֻ���ڴ_�J child ���ڕr���ň������m����
            child.transform.position = transform.position + new Vector3(0, -(i + 1) * yOffset, 0);

            var childRenderer = child.card.GetArtworkRenderer();
            if (childRenderer != null)
            {
                childRenderer.sortingOrder = myRenderer.sortingOrder + i + 1;
            }
            child.UpdateStackVisuals(depth + 1);
        }
        // --- �����ޏ��c �Y�� ---
    }

    private void AddChild(CardStacker child) { if (!children.Contains(child)) children.Add(child); }
    private void RemoveChild(CardStacker child) { children.Remove(child); }

    public CardStacker GetRoot()
    {
        CardStacker current = this;
        int count = 0;
        while (current.Parent != null)
        {
            current = current.Parent;
            if (++count > SAFETY_LOOP_LIMIT) { Debug.LogError($"[{name}] GetRoot loop limit reached!"); break; }
        }
        return current;
    }

    public bool IsDescendant(CardStacker potentialDescendant, int depth = 0)
    {
        if (depth > SAFETY_LOOP_LIMIT) { Debug.LogError($"[{name}] IsDescendant recursion limit reached!"); return false; }
        foreach (var child in children)
        {
            if (child == potentialDescendant || child.IsDescendant(potentialDescendant, depth + 1)) return true;
        }
        return false;
    }

    public List<Card> GetCardsInStack()
    {
        var list = new List<Card>();
        CollectCardsRecursively(GetRoot(), list);
        // ����һ�¿��ܴ��ڵ� null ����
        list.RemoveAll(item => item == null);
        return list;
    }

    private void CollectCardsRecursively(CardStacker current, List<Card> list, int depth = 0)
    {
        if (current == null) return;
        if (depth > SAFETY_LOOP_LIMIT) { Debug.LogError($"[{name}] CollectCardsRecursively recursion limit reached!"); return; }

        list.Add(current.card);
        foreach (var child in current.children)
        {
            CollectCardsRecursively(child, list, depth + 1);
        }
    }
}