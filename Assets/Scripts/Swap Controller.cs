using UnityEngine;

public class SwapController : MonoBehaviour
{
    public GameObject firstTouchObject;
    public GameObject secondTouchObject;
    Board board;
    public Dots firstDot;
    public Dots secondDot;
    ManaManager manager;
    Enemy enemy;
    DestroyMatches destroyMatches; // YENİ: Doğrudan yok etme fonksiyonunu çağırmak için ekledik

    void Start()
    {
        enemy = FindAnyObjectByType<Enemy>();
        manager = FindAnyObjectByType<ManaManager>();
        board = FindAnyObjectByType<Board>();
        destroyMatches = FindAnyObjectByType<DestroyMatches>(); // YENİ
    }

    void Update()
    {
        if (board.currentState == GameState.selection) return;
        if (board.currentState == GameState.wait) return;

        // OYUNCU EKRANA DOKUNDUĞUNDA
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null)
            {
                firstTouchObject = hit.collider.gameObject;
                firstDot = firstTouchObject.GetComponent<Dots>();

                // YENİ: Eğer tıklanan taş bir ÖZEL TAŞ ise (Satır veya Sütun patlatıcıysa)
                if (firstDot != null && firstDot.candyType != CandyType.Normal)
                {
                    // Tahtayı bekleme moduna al ki oyuncu o sırada başka taşa dokunamasın
                    board.currentState = GameState.wait;

                    // Taşı patlayacak olarak işaretle
                    firstDot.isMatched = true;

                    // Doğrudan patlatma zincirini başlat
                    destroyMatches.DestroyMatchedTiles();

                    // Seçimleri sıfırla ve Update'in geri kalanını çalıştırma (Sürüklemeye gerek kalmadı)
                    firstTouchObject = null;
                    return;
                }
            }
        }

        // OYUNCU PARMAĞINI KALDIRDIĞINDA (Normal taşlar için sürükleme/yer değiştirme mantığı aynen devam ediyor)
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider != null && firstTouchObject != null && hit.collider.gameObject != firstTouchObject)
            {
                secondTouchObject = hit.collider.gameObject;
                secondDot = secondTouchObject.GetComponent<Dots>();
                Swap();
            }
        }
    }

    void Swap()
    {
        if (firstDot && secondDot)
        {
            float distance = Mathf.Abs(firstDot.column - secondDot.column) + Mathf.Abs(firstDot.row - secondDot.row);

            if (distance == 1)
            {
                StartCoroutine(board.DoSwap(firstTouchObject, secondTouchObject, false));

                manager.move--;
                manager.textmove.text = manager.move.ToString();
                if (manager.move == 0)
                {
                    manager.move = 0;
                    Debug.Log("Düşman saldırmalı");
                    enemy.EnemyAttack();
                }
            }
        }
    }
}