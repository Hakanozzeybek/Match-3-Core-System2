using UnityEngine;
using System.Collections;

public enum CandyType { Normal, RowClearer, ColumnClearer }

public class Dots : MonoBehaviour
{
    public int column;
    public int row;
    public bool isMatched = false;
    public GameObject effect;
    private Coroutine moveCoroutine;

    // YENİ: Taşın türünü belirler
    public CandyType candyType = CandyType.Normal;

    public void CheckPosition()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        Vector3 target = new Vector3(column, row, 0);
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 15f);
            yield return null;
        }
        transform.position = target;
        moveCoroutine = null;
    }

    // YENİ: Taşı özel taşa dönüştüren fonksiyon
    public void MakeRowClearer()
    {
        candyType = CandyType.RowClearer;
        // Görsel geri bildirim: Özel taşı ayırt etmek için rengini koyulaştırıyoruz/değiştiriyoruz
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (render != null) render.color = Color.gray;
    }

    public void MakeColumnClearer()
    {
        candyType = CandyType.ColumnClearer;
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (render != null) render.color = Color.black;
    }

    public void Explasion()
    {
        if (effect != null)
        {
            GameObject exp = Instantiate(effect, transform.position, Quaternion.identity);
            Destroy(exp, 1f);
        }
    }
}