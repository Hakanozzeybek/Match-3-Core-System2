using UnityEngine;

public class Wave1attackPrefab : MonoBehaviour
{
    float speed = 3f;
    Player player;
    EnemyData myData; // Bu merminin kullanacağı veri paketi

    // Düşman mermiyi yarattığında bu fonksiyonu çağıracak
    public void Setup(EnemyData data)
    {
        myData = data;
    }

    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {
        if (player != null && myData != null) // myData boş değilse çalış
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 0.1f)
            {
                // Artık elimizde myData olduğu için içindeki damage'ı okuyabiliriz
                player.TakeDamage(myData);
                Destroy(gameObject);
            }
        }
    }
}