using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int EnemyCurrentHealth;
    int maxHealth;
    public Slider enemyHealthSlider;
    public Text enemyHealthText;

    public EnemyData myData;

    public void TakeDamage(int damage)
    {
        EnemyCurrentHealth -= damage;
        UpdateHealthSlider();

        if (EnemyCurrentHealth <= 0)
        {
            EnemyCurrentHealth = 0;
            UpdateHealthSlider();
            // ManaManager'a "Düşman Öldü" haberi gönderiliyor
            ManaManager.instance.EnemyDied();
            GetComponent<SpriteRenderer>().sprite = myData.deadSprite;

        }
        else
        {
            // Ölmediyse hasar alma (impact) sprite'ını göster
            StartCoroutine(PlayImpactEffect());
        }
    }

    IEnumerator PlayImpactEffect()
    {
        if (myData != null && myData.impactSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = myData.impactSprite;
            yield return new WaitForSeconds(0.2f);
            GetComponent<SpriteRenderer>().sprite = myData.idleSprite;
        }
    }

    public void UpdateHealthSlider()
    {
        if (enemyHealthSlider != null) enemyHealthSlider.value = (float)EnemyCurrentHealth / maxHealth;
        if (enemyHealthText != null) enemyHealthText.text = EnemyCurrentHealth + "/" + maxHealth;
    }

    public void SetupEnemy(EnemyData data)
    {
        myData = data;
        GetComponent<SpriteRenderer>().sprite = data.idleSprite;
        maxHealth = data.maxHealth;
        EnemyCurrentHealth = maxHealth;
        UpdateHealthSlider();
    }

    public IEnumerator DeadSequence()
    {
        // 1. Ölme resmini göster
        if (myData != null) GetComponent<SpriteRenderer>().sprite = myData.deadSprite;

        Debug.Log("Düşman can verdi, bekleniyor...");
        yield return new WaitForSeconds(1.5f); // Oyuncu ölümü görsün

        // 2. Eğer sonraki dalga varsa onu kur
        if (ManaManager.instance.waveSpritesCount < ManaManager.instance.enemyDatas.Length)
        {
            SetupEnemy(ManaManager.instance.enemyDatas[ManaManager.instance.waveSpritesCount]);
            Debug.Log("Yeni dalga düşmanı sahneye girdi.");
        }
    }

    public void EnemyAttack()
    {
        if (myData != null)
        {
            GetComponent<SpriteRenderer>().sprite = myData.attackSprite;
            GameObject projectile = Instantiate(myData.AttackObjectPrefab, transform.position, Quaternion.identity);
            Wave1attackPrefab bulletScript = projectile.GetComponent<Wave1attackPrefab>();
            if (bulletScript != null) bulletScript.Setup(myData);
        }
        StartCoroutine(ResetToIdleAfterAttack());
    }

    IEnumerator ResetToIdleAfterAttack()
    {
        yield return new WaitForSeconds(1f);
        if (myData != null) GetComponent<SpriteRenderer>().sprite = myData.idleSprite;
    }
}