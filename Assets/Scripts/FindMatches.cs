using UnityEngine;
using System.Collections.Generic;

public class FindMatches : MonoBehaviour
{
    Board board;
    SwapController swapController;

    void Start()
    {
        board = FindAnyObjectByType<Board>();
        swapController = FindAnyObjectByType<SwapController>();
    }

    public bool Matches()
    {
        bool matchFound = false;

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject current = board.allTiles[i, j];
                if (current == null) continue;

                // --- YATAY KONTROL ---
                if (i > 0 && i < board.width - 1)
                {
                    GameObject left = board.allTiles[i - 1, j];
                    GameObject right = board.allTiles[i + 1, j];
                    if (left != null && right != null && left.tag == current.tag && right.tag == current.tag)
                    {
                        List<GameObject> horizontalMatch = new List<GameObject> { left, current, right };
                        matchFound = true;

                        if (i + 2 < board.width && board.allTiles[i + 2, j] != null && board.allTiles[i + 2, j].tag == current.tag)
                        {
                            horizontalMatch.Add(board.allTiles[i + 2, j]);
                        }
                        if (i - 2 >= 0 && board.allTiles[i - 2, j] != null && board.allTiles[i - 2, j].tag == current.tag)
                        {
                            horizontalMatch.Add(board.allTiles[i - 2, j]);
                        }

                        ProcessMatchList(horizontalMatch, false);
                    }
                }

                // --- DİKEY KONTROL ---
                if (j > 0 && j < board.height - 1)
                {
                    GameObject down = board.allTiles[i, j - 1];
                    GameObject up = board.allTiles[i, j + 1];
                    if (down != null && up != null && down.tag == current.tag && up.tag == current.tag)
                    {
                        List<GameObject> verticalMatch = new List<GameObject> { down, current, up };
                        matchFound = true;

                        if (j + 2 < board.height && board.allTiles[i, j + 2] != null && board.allTiles[i, j + 2].tag == current.tag)
                        {
                            verticalMatch.Add(board.allTiles[i, j + 2]);
                        }
                        if (j - 2 >= 0 && board.allTiles[i, j - 2] != null && board.allTiles[i, j - 2].tag == current.tag)
                        {
                            verticalMatch.Add(board.allTiles[i, j - 2]);
                        }

                        ProcessMatchList(verticalMatch, true);
                    }
                }
            }
        }
        return matchFound;
    }

    private void ProcessMatchList(List<GameObject> matchTiles, bool isVertical)
    {
        if (matchTiles.Count == 4)
        {
            GameObject specialTile = null;
            foreach (GameObject tile in matchTiles)
            {
                if (tile == swapController.firstTouchObject || tile == swapController.secondTouchObject)
                {
                    specialTile = tile;
                    break;
                }
            }
            if (specialTile == null) specialTile = matchTiles[0];

            Dots specialDot = specialTile.GetComponent<Dots>();
            if (isVertical) specialDot.MakeRowClearer();
            else specialDot.MakeColumnClearer();

            foreach (GameObject tile in matchTiles)
            {
                if (tile != specialTile) tile.GetComponent<Dots>().isMatched = true;
            }
        }
        else
        {
            foreach (GameObject tile in matchTiles) tile.GetComponent<Dots>().isMatched = true;
        }
    }

    // YENİ: Tahtada yapılabilecek hamle kaldı mı kontrol eder
    public bool IsMovePossible()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allTiles[i, j] == null) continue;

                // Sağa kaydırma simülasyonu
                if (i < board.width - 1 && board.allTiles[i + 1, j] != null)
                {
                    if (SimulateCheck(i, j, i + 1, j)) return true;
                }
                // Aşağı kaydırma simülasyonu
                if (j < board.height - 1 && board.allTiles[i, j + 1] != null)
                {
                    if (SimulateCheck(i, j, i, j + 1)) return true;
                }
            }
        }
        return false;
    }

    // İki taşın yerini hayali değiştirip eşleşme çıkıyor mu bakan yardımcı fonksiyon
    private bool SimulateCheck(int fX, int fY, int sX, int sY)
    {
        GameObject first = board.allTiles[fX, fY];
        GameObject second = board.allTiles[sX, sY];

        // Geçici yer değişimi
        board.allTiles[fX, fY] = second;
        board.allTiles[sX, sY] = first;

        bool hasMatch = SimulatedMatches();

        // Eski haline geri al
        board.allTiles[fX, fY] = first;
        board.allTiles[sX, sY] = second;

        return hasMatch;
    }

    // Taşları işaretlemeden sadece eşleşme var mı diye kontrol eden hafif (lightweight) fonksiyon
    private bool SimulatedMatches()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject cur = board.allTiles[i, j];
                if (cur == null) continue;

                if (i > 0 && i < board.width - 1)
                {
                    if (board.allTiles[i - 1, j] != null && board.allTiles[i + 1, j] != null &&
                        board.allTiles[i - 1, j].tag == cur.tag && board.allTiles[i + 1, j].tag == cur.tag) return true;
                }
                if (j > 0 && j < board.height - 1)
                {
                    if (board.allTiles[i, j - 1] != null && board.allTiles[i, j + 1] != null &&
                        board.allTiles[i, j - 1].tag == cur.tag && board.allTiles[i, j + 1].tag == cur.tag) return true;
                }
            }
        }
        return false;
    }
}