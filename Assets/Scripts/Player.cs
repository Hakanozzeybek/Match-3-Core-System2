using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Sağlık Ayarları")]
    public int currentHealth;
    public int maxHealth = 100;

    [Header("UI Elemanları")]
    public Slider playerHealthSlider;
    public Text playerHealthText;

    private ManaManager manaManager;

    void Start()
    {
        manaManager = ManaManager.instance; // Singleton üzerinden erişmek daha performanslıdır
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    public void TakeDamage(EnemyData enemyData)
    {
        if (enemyData == null) return;

        currentHealth -= enemyData.damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Dead();
        }
        UpdateHealthSlider();
    }

    public void UpdateHealthSlider()
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = (float)currentHealth / maxHealth;
        }

        if (playerHealthText != null)
        {
            playerHealthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        // Canın maksimum değeri aşmasını engelliyoruz
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthSlider();
    }

    public void ActiviteShield()
    {
        // ManaManager'daki prefab üzerinden kalkanı oluşturur
        if (manaManager != null && manaManager.wave1IcePrefab != null)
        {
            Instantiate(manaManager.wave1IcePrefab, transform.position, Quaternion.identity);
            Destroy(manaManager.wave1IcePrefab, 5f); // Kalkan 5 saniye sonra yok olur
            Debug.Log("Kalkan Aktif! Bu tur hasar engellenecek.");
            manaManager.StarterPanel(); // Kalkan aktif olduktan sonra yetenek seçme panelini tekrar açar
        }
    }

    public void Dead()
    {
        // ManaManager'daki referansları kullanarak ölme sprite'ını basar
        if (manaManager != null && manaManager.playerSpriteRenderer != null && manaManager.deadSprite != null)
        {
            manaManager.playerSpriteRenderer.sprite = manaManager.deadSprite;
        }

        Debug.Log("Oyuncu yenildi!");
        // Buraya "Game Over" paneli tetikleyicisi eklenebilir
    }
}