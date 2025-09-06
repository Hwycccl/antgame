// ·ÅÖÃì¶: CardStacker.cs (ÖÇÄÜ¶Ñ¯B×î½K°æ)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CardStacker : MonoBehaviour
{
    [Header("¶Ñ¯BÔOÖÃ")]
    [Tooltip("¿¨ÅÆÎü¸½„Ó®‹µÄ³ÖÀm•rég£¨Ãë£©")]
    [SerializeField] private float stackAnimationDuration = 0.15f;
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
        CardStacker potentialTarget = FindBestStackingTarget();

        if (potentialTarget != null)
        {
            CardStacker finalTarget = potentialTarget.GetTopmostCardInStack();
            StackOn(finalTarget);
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

        StopAllCoroutines();
        StartCoroutine(AnimateToStackPosition());

        GetRoot().card.Combiner.CheckForCombination();
    }

    private CardStacker FindBestStackingTarget()
    {
        return nearbyTargets
            .Where(t => t != null && t.gameObject.activeInHierarchy && !IsDescendant(t))
            .OrderBy(t => Vector2.Distance(transform.position, t.transform.position))
            .FirstOrDefault();
    }

    private IEnumerator AnimateToStackPosition()
    {
        if (Parent == null) yield break;

        int childIndex = Parent.children.IndexOf(this);
        Vector3 targetPosition = Parent.transform.position + new Vector3(0, -(childIndex + 1) * yOffset, 0);

        Vector3 startPosition = transform.position;
        float timeElapsed = 0f;

        while (timeElapsed < stackAnimationDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / stackAnimationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        GetRoot().UpdateStackVisuals();
    }

    public void UpdateStackVisuals(int depth = 0)
    {
        if (depth > SAFETY_LOOP_LIMIT) { Debug.LogError($"[{name}] UpdateStackVisuals recursion limit reached!"); return; }

        var myRenderer = card.GetArtworkRenderer();
        if (myRenderer == null) return;

        // ÐÞÕý: ÒÆ³ý¶Ô SetOriginalSortingOrder µÄµ÷ÓÃ
        // card.Dragger.SetOriginalSortingOrder(myRenderer.sortingOrder);

        for (int i = 0; i < children.Count; i++)
        {
            CardStacker child = children[i];
            if (child == null)
            {
                children.RemoveAt(i);
                i--;
                continue;
            }

            child.transform.position = transform.position + new Vector3(0, -(i + 1) * yOffset, 0);

            var childRenderer = child.card.GetArtworkRenderer();
            if (childRenderer != null)
            {
                childRenderer.sortingOrder = myRenderer.sortingOrder + i + 1;
            }
            child.UpdateStackVisuals(depth + 1);
        }
    }

    // --- ÝoÖúº¯”µ ---
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

    public CardStacker GetTopmostCardInStack()
    {
        CardStacker current = this;
        int count = 0;
        while (current.children.Count > 0)
        {
            current = current.children[current.children.Count - 1];
            if (++count > SAFETY_LOOP_LIMIT) { Debug.LogError($"[{name}] GetTopmostCardInStack loop limit reached!"); break; }
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

    public void ForceStackOn(CardStacker newParent)
    {
        Debug.Log($"[{gameObject.name}] is being force-stacked onto [{newParent.name}] by the system.");
        StackOn(newParent);
    }
}