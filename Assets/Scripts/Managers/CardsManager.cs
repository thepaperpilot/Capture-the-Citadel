using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardsManager : SerializedMonoBehaviour
{
    public static CardsManager Instance;

    [Space, HideInPlayMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public List<AbstractCard> starterDeck;
    [Space, HideInEditorMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public  List<AbstractCard> deck;
    public int handSize = 5;

    [Title("Combat")]
    [Space, HideInEditorMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public  List<AbstractCard> hand;
    [HideInEditorMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public List<AbstractCard> drawPile;
    [HideInEditorMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public List<AbstractCard> discards;

    [HideInInspector]
    public PlayerController controller;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        SceneManager.sceneLoaded += (scene, mode) => ResetDeck();
        deck = new List<AbstractCard>(starterDeck);
    }

    public void ResetDeck() {
        hand = new List<AbstractCard>();
        drawPile = new List<AbstractCard>(deck);
        Shuffle(drawPile);
        discards = new List<AbstractCard>();
        ActionsManager.Instance.AddToTop(new DrawAction(handSize));
    }

    public IEnumerator Draw(int amount) {
        if (amount > drawPile.Count() && discards.Count() > 0) {
            ActionsManager.Instance.AddToTop(new DrawAction(amount - drawPile.Count()));
            ActionsManager.Instance.AddToTop(new ShuffleDiscardsAction());
        }
        int count = Mathf.Min(amount, drawPile.Count());
        IEnumerable<AbstractCard> cardsToDraw = drawPile.Take(count);
        drawPile.RemoveRange(0, count);
        hand.AddRange(cardsToDraw);
        yield return controller.Draw(cardsToDraw.ToArray());
    }

    public IEnumerator Discard(AbstractCard card) {
        hand.Remove(card);
        discards.Add(card);
        yield return null;
    }

    [HideInEditorMode, Button(ButtonSizes.Medium)]
    public IEnumerator ShuffleDiscards() {
        Shuffle(discards);
        drawPile.AddRange(discards);
        discards.Clear();
        RelicsManager.Instance.OnShuffle();
        yield return null;
    }

    public void Shuffle(List<AbstractCard> cards) {
        int n = cards.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            AbstractCard value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }
}
