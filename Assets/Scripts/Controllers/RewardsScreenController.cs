using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RewardsScreenController : MonoBehaviour, IScreenSelector
{
    [SerializeField, AssetList, InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    private AbstractLevel level;
    [SerializeField, AssetList(Path="Prefabs/Toys", CustomFilterMethod="FindToy")]
    [InlineEditor(InlineEditorModes.LargePreview, InlineEditorObjectFieldModes.Foldout)]
    private GameObject toy;
    [SerializeField, ValueDropdown("FindScenes")]
    private string nextScene;

    [BoxGroup("Card weights")]
    public float common = 1;
    [BoxGroup("Card weights")]
    public float uncommon = .5f;
    [BoxGroup("Card weights")]
    public float rare = .1f;

    void Start() {
        LevelManager.Instance.SetLevel(level);

        CardRewardController[] controllers =
            LevelManager.Instance.GetComponentsInChildren<CardRewardController>();
        float total = PlayerManager.Instance.playerClass.cardPool.Sum(c =>
        c.rarity == AbstractCard.Rarities.COMMON ? common :
        c.rarity == AbstractCard.Rarities.UNCOMMON ? uncommon :
        c.rarity == AbstractCard.Rarities.RARE ? rare : 0);

        List<AbstractCard> cards = new List<AbstractCard>();
        while (cards.Count < controllers.Length) {
            float target = UnityEngine.Random.Range(0, total);
            float current = 0;
            AbstractCard card = PlayerManager.Instance.playerClass.cardPool.SkipWhile(c => {
                current += c.rarity == AbstractCard.Rarities.COMMON ? common :
                    c.rarity == AbstractCard.Rarities.UNCOMMON ? uncommon :
                    c.rarity == AbstractCard.Rarities.RARE ? rare : 0;
                return target > current;
            }).FirstOrDefault();
            if (card != null)
            {
                bool unique = true;
                foreach(AbstractCard addedCard in cards)
                {
                    if (addedCard.name.Equals(card.name))
                    {
                        unique = false;
                    }
                }
                if (unique)
                {
                    cards.Add(card);
                }
            }
                
                
        }
        for (int i = 0; i < controllers.Length; i++) {
            controllers[i].Setup(cards[i], this);
        }

        SceneSelectorController sceneSelector =
            LevelManager.Instance.GetComponentInChildren<SceneSelectorController>();
        if (sceneSelector != null)
            sceneSelector.Setup(nextScene, this);

        GameObject toyInstance = Instantiate(toy);
        PlayerManager.Instance.Grab(toyInstance);
    }

    public void Choose(AbstractCard selectedCard) {
        CardsManager.Instance.deck.Add(selectedCard);
        SelectScene(nextScene);
    }

    public void SelectScene(string scene) {
        ChangeSceneAction sceneChange = new ChangeSceneAction(scene);
        ActionsManager.Instance.AddToBottom(sceneChange);
    }

#if UNITY_EDITOR
    private bool FindToy(GameObject obj) {
        return obj.GetComponentInChildren<Toy>() != null;
    }

    private IEnumerable FindScenes() {
        int sceneCount = SceneManager.sceneCountInBuildSettings;     
        ValueDropdownItem[] scenes = new ValueDropdownItem[sceneCount];
        for( int i = 0; i < sceneCount; i++ ) {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            scenes[i] = new ValueDropdownItem(name, name);
        }
        return scenes;
    }
#endif
}
