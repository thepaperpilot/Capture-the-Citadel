using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardsManager : SerializedMonoBehaviour
{
    public static CardsManager Instance;

    [Space, HideInEditorMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView=true, DrawDropdownForListElements=false, IsUniqueList=false)]
    public  List<AbstractCard> deck;
    [AssetList(AutoPopulate=true, CustomFilterMethod="FindCards")]
    public AbstractCard[] allCards;
    public int startingHandSize = 5;
    public int cardDrawPerTurn = 3;

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
    [HideInEditorMode, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [AssetSelector(FlattenTreeView = true, DrawDropdownForListElements = false, IsUniqueList = false)]
    public List<AbstractCard> exhausts;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ResetDeck() {
        hand = new List<AbstractCard>();
        drawPile = new List<AbstractCard>(deck);
        Shuffle(drawPile);
        discards = new List<AbstractCard>();
        exhausts = new List<AbstractCard>();
        PlayerManager.Instance.GetDeckController().Clear();
        ActionsManager.Instance.AddToTop(new DrawAction(startingHandSize - cardDrawPerTurn));
    }

    public IEnumerator Draw(int amount) {
        amount = Mathf.Min(drawPile.Count + discards.Count, amount); //Don't try to draw more cards than exist outside of hand
        if(hand.Count + amount > 10)
        {
            amount = Mathf.Max(0,10 - hand.Count);
        }
        if (amount > drawPile.Count() && discards.Count() > 0) {
            ActionsManager.Instance.AddToTop(new DrawAction(amount - drawPile.Count()));
            ActionsManager.Instance.AddToTop(new ShuffleDiscardsAction());
        }
        int count = Mathf.Min(amount, drawPile.Count());
        AbstractCard[] cardsToDraw = drawPile.Take(count).ToArray();
        hand.AddRange(cardsToDraw);
        drawPile.RemoveRange(0, count);
        yield return PlayerManager.Instance.Draw(cardsToDraw);
    }

    public void Discard(AbstractCard card) {
        hand.Remove(card);
        discards.Add(card);
    }

    public void Exhaust(AbstractCard card)
    {
        hand.Remove(card);
        exhausts.Add(card);
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

#if UNITY_EDITOR
    public bool FindCards(AbstractCard card) {
        return card.rarity != AbstractCard.Rarities.CLASS;
    }
#endif
}
