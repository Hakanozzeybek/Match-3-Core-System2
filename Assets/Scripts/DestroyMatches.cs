using UnityEngine;
using System.Collections;

public class DestroyMatches : MonoBehaviour
{
    Board board;
    void Start() { board = FindAnyObjectByType<Board>(); }

    public void DestroyMatchedTiles()
    {
        // 1. ADIM: Özel taşların patlama alanlarını isMatched = true olarak işaretle
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allTiles[i, j] != null)
                {
                    Dots dot = board.allTiles[i, j].GetComponent<Dots>();
                    if (dot.isMatched)
                    {
                        if (dot.candyType == CandyType.RowClearer)
                        {
                            MatchRow(j); // Bulunduğu satırı işaretle
                        }
                        else if (dot.candyType == CandyType.ColumnClearer)
                        {
                            MatchColumn(i); // Bulunduğu sütunu işaretle
                        }
                    }
                }
            }
        }

        // 2. ADIM: İşaretlenen tüm taşları yok et ve listeye ekle
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allTiles[i, j] != null && board.allTiles[i, j].GetComponent<Dots>().isMatched)
                {
                    board.allTiles[i, j].GetComponent<Dots>().Explasion();
                    board.currentMatches.Add(board.allTiles[i, j]);
                    Destroy(board.allTiles[i, j]);
                    board.allTiles[i, j] = null;
                }
            }
        }

        // Mana kazanım kontrolleri
        if (board.currentMatches.Count == 3) ManaManager.instance.AddMana(10);
        else if (board.currentMatches.Count == 4) ManaManager.instance.AddMana(20);
        else if (board.currentMatches.Count >= 5) ManaManager.instance.AddMana(30);

        board.currentMatches.Clear();
        StartCoroutine(board.RefillBoard());
    }

    // Satırdaki tüm taşları eşleşti olarak işaretleyen yardımcı fonksiyon
    private void MatchRow(int row)
    {
        for (int i = 0; i < board.width; i++)
        {
            if (board.allTiles[i, row] != null)
            {
                Dots dot = board.allTiles[i, row].GetComponent<Dots>();
                if (!dot.isMatched)
                {
                    dot.isMatched = true;
                    // Eğer o satırda başka bir özel taş varsa o da tetiklensin (Zincirleme reaksiyon)
                    if (dot.candyType == CandyType.ColumnClearer) MatchColumn(i);
                }
            }
        }
    }

    // Sütundaki tüm taşları eşleşti olarak işaretleyen yardımcı fonksiyon
    private void MatchColumn(int col)
    {
        for (int j = 0; j < board.height; j++)
        {
            if (board.allTiles[col, j] != null)
            {
                Dots dot = board.allTiles[col, j].GetComponent<Dots>();
                if (!dot.isMatched)
                {
                    dot.isMatched = true;
                    if (dot.candyType == CandyType.RowClearer) MatchRow(j);
                }
            }
        }
    }
}