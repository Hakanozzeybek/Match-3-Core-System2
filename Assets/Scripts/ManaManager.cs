using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManaManager : MonoBehaviour
{
    public static ManaManager instance;

    [Header("Temel Ayarlar")]
    public int totalMana = 100;
    public int currentMana = 0;
    public int move = 10;

    [Header("UI Referansları")]
    public Text textmove;
    public Slider Slider;
    public GameObject skillPanel;

    [Header("Yetenek Butonları ve Metinleri")]
    public Button skill1Button;
    public Button skill2Button;
    public Button skill3Button;
    public Text skill1Name, skill2Name, skill3Name;
    public Text skill1Description, skill2Description, skill3Description;

    [Header("Yetenek Görselleri (Sürükle-Bırak)")]
    public Sprite skill1Sprite; // Alev Pencesi
    public Sprite skill2Sprite; // Buz Kabugu
    public Sprite skill3Sprite; // Can Arttirma
    public Image selectedSkillSprite;

    [Header("Yetenek Prefabları")]
    public GameObject wave1FirePrefab;
    public GameObject wave1IcePrefab;
    public GameObject wave1LifePrefab;

    [Header("Düşman Veri Paketleri (Waves)")]
    public EnemyData[] enemyDatas;
    public int waveSpritesCount = 0;

    [Header("Player Animasyonları")]
    public SpriteRenderer playerSpriteRenderer;
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite deadSprite;

    private string currentSkillName;
    private Board board;
    private Enemy enemy;
    private Player player;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        enemy = FindAnyObjectByType<Enemy>();
        board = FindAnyObjectByType<Board>();

        // İlk düşmanı paketinden kur
        if (enemyDatas.Length > 0 && enemy != null)
            enemy.SetupEnemy(enemyDatas[waveSpritesCount]);

        currentMana = Mathf.Clamp(currentMana, 0, totalMana);
        StarterPanel();
    }

    public void StarterPanel()
    {
        // --- +2 HAMLE BONUSU MANTIĞI ---
        // Eğer düşman hala hayattaysa, her panel açıldığında oyuncuya ödül verilir.
        if (enemy != null && enemy.EnemyCurrentHealth > 0&& waveSpritesCount !=0)
        {
            move += 2;
            if (textmove != null) textmove.text = move.ToString();
            Debug.Log("Düşman ölmediği için +2 hamle eklendi. Mevcut: " + move);
        }

        board = FindAnyObjectByType<Board>();
        skillPanel.SetActive(true);

        // Görselleri ve İsimleri Atama
        if (skill1Sprite != null)
        {
            skill1Button.image.sprite = skill1Sprite;
            skill1Name.text = skill1Sprite.name;
        }
        if (skill2Sprite != null)
        {
            skill2Button.image.sprite = skill2Sprite;
            skill2Name.text = skill2Sprite.name;
        }
        if (skill3Sprite != null)
        {
            skill3Button.image.sprite = skill3Sprite;
            skill3Name.text = skill3Sprite.name;
        }

        // Açıklamaları Atama
        skill1Description.text = "100 Mana: Düşmana 100 Hasar";
        skill2Description.text = "100 Mana: Kalkan Açarak Hasarı Önler. +5 Hamle kazandırır.";
        skill3Description.text = "100 Mana: Can Sayısını 50 arttırır.";

        if (board != null) board.currentState = GameState.selection;
    }

    public void SelectSkillByButton(Button clickedButton)
    {
        skillPanel.SetActive(false);
        currentSkillName = clickedButton.image.sprite.name;
        selectedSkillSprite.sprite = clickedButton.image.sprite;

        if (board != null) board.currentState = GameState.move;
        
    }

    public void AddMana(int amount)
    {
        currentMana += amount;

        
        if (textmove != null) textmove.text = move.ToString();

        if (currentMana >= totalMana)
        {
            currentMana = totalMana;
            BattleManager();
        }
        Slider.value = (float)currentMana / totalMana;
    }

    public void BattleManager()
    {
        switch (currentSkillName)
        {
            case "Alev Pencesi":
                StartCoroutine(PlayAttackSequence());
                break;
            case "Buz Kabugu":
                player.ActiviteShield();
                move += 5;
                ResetState();
                break;
            case "Can Arttırma":
                player.Heal(50);
                ResetState();
                break;
        }
    }

    public void EnemyDied()
    {
        waveSpritesCount++;
        StarterPanel();
        Debug.Log("Düşman öldü, sıradaki dalga hazırlanıyor...");

        if (waveSpritesCount < enemyDatas.Length)
        {
            StartCoroutine(enemy.DeadSequence());
            // Not: Yeni dalga başladığında ResetState move'u 10 yapacak.
            ResetState();
            
        }
        else
        {
            Debug.Log("TÜM OYUN BİTTİ!");
            ResetState();
        }
    }

    IEnumerator PlayAttackSequence()
    {
        if (playerSpriteRenderer != null && attackSprite != null)
            playerSpriteRenderer.sprite = attackSprite;

        if (player != null)
            Instantiate(wave1FirePrefab, player.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        if (playerSpriteRenderer != null && idleSprite != null)
            playerSpriteRenderer.sprite = idleSprite;

        // Saldırı animasyonu bittikten sonra kısa bir bekleme ve yeni seçim
        yield return new WaitForSeconds(1.5f);

        if (enemy != null && enemy.EnemyCurrentHealth > 0)
        {
            StarterPanel();
        }

        ResetState();
    }

    public void ResetState()
    {
        currentMana = 0;
        Slider.value = 0;

        // Sadece düşman öldüğünde hamle sayısını 10'a sabitleriz.
        if (enemy != null && enemy.EnemyCurrentHealth <= 0)
        {
            move = 10;
            if (textmove != null) textmove.text = move.ToString();
        }
    }
}