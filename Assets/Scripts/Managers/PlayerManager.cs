using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zinnia.Tracking.Follow;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    
    private float gold = 0;

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject playAreaAlias;
    [SerializeField] private GameObject headsetAlias;
    [SerializeField] private DeckController deckController;
    public int maxHealth;
    public int health;
    public int energyPerTurn;
    [HideInPlayMode, ChildGameObjectsOnly]
    public EnemyReadoutUI energyBar;
    [HideInPlayMode, ChildGameObjectsOnly]
    public EnemyReadoutUI healthBar;
    [HideInPlayMode, ChildGameObjectsOnly]
    public TextMeshPro goldText;
    [SerializeField] private List<Transform> leftControllerSources;
    [SerializeField] private List<Transform> rightControllerSources;
    [SerializeField] private List<Transform> headsetSources;
    private Hand right;
    private Hand left;
    private bool isEndingTurn;
    private Transform leftSource;
    private Transform rightSource;
    private Transform headsetSource;
    Rigidbody leftRB;
    Rigidbody rightRB;

    [HideInEditorMode]
    public AbstractClass playerClass;

    public float Gold {
        get => gold;
        set {
            gold = value;
            goldText.text = "" + gold;
        }
    }

    [Button(ButtonSizes.Large), HideInEditorMode]
    public void ResetGold() {
        Gold = 0;
    }

    [Button(ButtonSizes.Large), HideInEditorMode]
    public void MoreGold() {
        Gold += 100;
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        right = rightHand.GetComponentInChildren<Hand>();
        left = leftHand.GetComponentInChildren<Hand>();
    }

    void Start()
    {
#if UNITY_EDITOR
        leftSource = leftControllerSources[0];
        rightSource = rightControllerSources[0];
        headsetSource = headsetSources[0];
#else
        leftSource = leftControllerSources[0];
        rightSource = rightControllerSources[0];
        headsetSource = headsetSources[0];
        //leftSource = leftControllerSources[1];
        //rightSource = rightControllerSources[1];
        //headsetSource = headsetSources[1];
#endif
        leftRB = leftHand.GetComponent<Rigidbody>();
        rightRB = rightHand.GetComponent<Rigidbody>();

        energyBar.Init(energyPerTurn, "");
    }

    void Update()
    {
        leftHand.transform.position = leftSource.position;
        leftHand.transform.rotation = leftSource.rotation;
        rightHand.transform.position = rightSource.position;
        rightHand.transform.rotation = rightSource.rotation;

        if (right.state == Hand.HandAnimPose.CLOSED && left.state == Hand.HandAnimPose.CLOSED &&
            CombatManager.Instance.IsPlayerTurn() && !ActionsManager.Instance.HasAction(typeof(EndTurnAction))) {
            isEndingTurn = true;
        } else if (isEndingTurn) {
            isEndingTurn = false;
            ActionsManager.Instance.AddToBottom(new EndTurnAction());
        }
    }

    private void FixedUpdate()
    {
        //Move Hands
        Vector3 leftDelta = leftSource.position - leftHand.transform.position;
        //leftRB.AddForce(leftDelta, ForceMode.Acceleration);
        leftHand.transform.position = leftSource.position;
        Vector3 rightDelta = rightSource.position - rightHand.transform.position;
        rightHand.transform.position = rightSource.position;
        //rightRB.AddForce(rightDelta, ForceMode.Acceleration);
    }

    public void SetClass(AbstractClass selectedClass)
    {
        playerClass = selectedClass;
        if(selectedClass == null)
        {
            left.SetTheme(AbstractCard.ClassColor.COLORLESS);
            right.SetTheme(AbstractCard.ClassColor.COLORLESS);
        }
        else
        {
            left.SetTheme(selectedClass.color);
            right.SetTheme(selectedClass.color);
        }
        Gold = selectedClass.startingGold;
        maxHealth = selectedClass.startingHealth;
        energyPerTurn = selectedClass.baseEnergyPerTurn;
        energyBar.Init(energyPerTurn, "");
        energyBar.ChangeValue(0);
        health = maxHealth;
    }

    public void MovePlayer(Vector3 position, Quaternion rotation) {
        playAreaAlias.transform.SetPositionAndRotation(position, rotation);
    }

    public void SetupDropzones() {
        deckController.SetupDropzones();
    }

    public IEnumerator StartTurn(int turn) {
        deckController.SetDeckSize(CardsManager.Instance.drawPile.Count);
        RelicsManager.Instance.OnTurnStart(turn);
        CombatManager.Instance.player.FillEnergy();
        int targetCardNum = CombatManager.Instance.player.statusController.GetHandSize(CardsManager.Instance.handSize);
        int cardsToDraw = Mathf.Max(0, targetCardNum - CardsManager.Instance.hand.Count());
        ActionsManager.Instance.AddToTop(new DrawAction(cardsToDraw));
        CombatManager.Instance.player.statusController.OnTurnStart();
        RelicsManager.Instance.OnTurnStartLate(turn);
        yield return deckController.SlideOut();
    }

    public IEnumerator EndTurn() {
        CombatManager.Instance.player.statusController.OnTurnEnd();
        yield return deckController.SlideIn();
    }
    
    public IEnumerator Draw(AbstractCard[] cardsToDraw) {
        for (int i = 0; i < cardsToDraw.Count(); i++) {
            StartCoroutine(deckController.Draw(cardsToDraw[i]));
            yield return new WaitForSeconds(deckController.timeBetweenDraws);
        }
        yield return new WaitForSeconds(deckController.timeToRearrange);
    }

    public void AddCard(CardController card) {
        deckController.DropCardInHand(card);
    }

    public void RemoveCard(CardController card) {
        deckController.PickupCard(card);
    }

    public Vector3 GetHeadsetPos()
    {
        return headsetSource.position;
    }

    public void Grab(GameObject gameObject) {
        right.Grab(gameObject);
    }

    // This is used by DebugManager
    public DeckController GetDeckController() {
        return deckController;
    }

    public void ChangeMaxHealth(int amount)
    {
        if (amount > 0)
        {
            maxHealth += amount;
            health += amount;
        }
        else
        {
            maxHealth -= amount;
            health = Mathf.Min(health, maxHealth);
        }
        healthBar.Init(maxHealth, "");
        healthBar.ChangeValue(health);
    }

    public void NonCombatLoseHealth(int amount)
    {
        if(amount > 0)
        {
            health -= Mathf.Max(0, amount);
            healthBar.ChangeValue(health);
            if (health <= 0)
            {
                Die();
            }
        }
    }

    public void NonCombatHeal(int amount)
    {
        if(amount > 0)
        {
            health += amount;
            health = Mathf.Min(health, maxHealth);
            healthBar.ChangeValue(health);
        }
    }

    public void Die()
    {
        SetClass(null);
        gold = 0;
        CardsManager.Instance.deck = new List<AbstractCard>();
        RelicsManager.Instance.ResetRelics();
        ChangeSceneAction sceneChange = new ChangeSceneAction("Title");
        ActionsManager.Instance.AddToBottom(sceneChange);
    }

    public Hand GetRightHand()
    {
        return right;
    }
}
