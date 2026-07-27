using UnityEngine;

public class AlevPençesiObjectMove : MonoBehaviour
{
    float speed = 3f; 
    Enemy enemy;
   public float distanceToEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy= FindAnyObjectByType<Enemy>();
        Debug.Log("Alev Pençesi hareket başladı!");
        transform.rotation = Quaternion.Euler(0, 180, 0); // Rotasyonu sıfırla
    }

    // Update is called once per frame
    void Update()
    {
        
            if (enemy != null)
            {
                // 1. Hedefe doğru kilitlenerek git
                transform.position = Vector2.MoveTowards(transform.position, enemy.transform.position, speed * Time.deltaTime);

                // 2. Mesafeyi ölç
                distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                // 3. Çarptı mı?
                if (distanceToEnemy < 0.1f)
                {
                    Debug.Log("Alev Pençesi düşmana çarptı!");
                    enemy.TakeDamage(100);

                    // ÇOK ÖNEMLİ: Hasarı verdikten sonra objeyi yok etmelisin
                    Destroy(gameObject);
                }
            }
        
    }
}
