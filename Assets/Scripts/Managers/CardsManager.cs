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
        ResetDeck();
    }

    private void ResetDeck() {
        hand = new List<AbstractCard>();
        drawPile = new List<AbstractCard>(deck);
        discards = new List<AbstractCard>();
    }

    public IEnumerator Draw(int amount) {
        if (amount > drawPile.Count() && discards.Count() > 0) {
            ActionsManager.Instance.AddToTop(new DrawAction(amount - drawPile.Count()));
            ActionsManager.Instance.AddToTop(new ShuffleDiscardsAction());
        }
        hand.AddRange(drawPile.Take(Mathf.Max(amount, drawPile.Count())));
        yield return null;
    }

    public IEnumerator Discard(AbstractCard card) {
        hand.Remove(card);
        discards.Add(card);
        yield return null;
    }

    [HideInEditorMode, Button(ButtonSizes.Medium)]
    public IEnumerator ShuffleDiscards() {
        int n = discards.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            AbstractCard value = discards[k];
            discards[k] = discards[n];
            discards[n] = value;
        }
        drawPile.AddRange(discards);
        discards.Clear();
        yield return null;
    }
}
