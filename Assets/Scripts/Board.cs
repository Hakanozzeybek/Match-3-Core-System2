using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { wait, move, selection }

public class Board : MonoBehaviour
{
    public int width, height;
    public GameObject[] tilePrefab;
    public Transform dotParent;
    public GameState currentState;
    public GameObject[,] allTiles;
    FindMatches findMatches;
    DestroyMatches destroyMatches;
    ManaManager manager;
    public ArrayList currentMatches = new ArrayList();

    float offset = 3f;

    void Start()
    {
        manager = FindAnyObjectByType<ManaManager>();
        Application.targetFrameRate = 90;
        destroyMatches = FindAnyObjectByType<DestroyMatches>();
        findMatches = FindAnyObjectByType<FindMatches>();
        allTiles = new GameObject[width, height];
        CreateBoard();
    }

    public void CreateBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int tile = Random.Range(0, tilePrefab.Length);
                int maxIterations = 0;
                while (maxIterations < 50 &&
                       ((i > 1 && allTiles[i - 1, j].tag == tilePrefab[tile].tag && allTiles[i - 2, j].tag == tilePrefab[tile].tag) ||
                        (j > 1 && allTiles[i, j - 1].tag == tilePrefab[tile].tag && allTiles[i, j - 2].tag == tilePrefab[tile].tag)))
                {
                    tile = Random.Range(0, tilePrefab.Length);
                    maxIterations++;
                }
                GameObject spawnedTile = Instantiate(tilePrefab[tile], new Vector2(i, j), Quaternion.identity);
                spawnedTile.transform.parent = dotParent;
                Dots dotScript = spawnedTile.GetComponent<Dots>();
                dotScript.column = i;
                dotScript.row = j;
                allTiles[i, j] = spawnedTile;
                dotScript.CheckPosition();
            }
        }
    }

    public IEnumerator DoSwap(GameObject first, GameObject second, bool isUndo)
    {
        currentState = GameState.wait;
        Dots fDot = first.GetComponent<Dots>();
        Dots sDot = second.GetComponent<Dots>();

        allTiles[fDot.column, fDot.row] = second;
        allTiles[sDot.column, sDot.row] = first;

        int tC = fDot.column; int tR = fDot.row;
        fDot.column = sDot.column; fDot.row = sDot.row;
        sDot.column = tC; sDot.row = tR;

        fDot.CheckPosition();
        sDot.CheckPosition();

        yield return new WaitForSeconds(0.25f);

        if (!isUndo)
        {
            if (!findMatches.Matches())
            {
                yield return new WaitForSeconds(0.2f);
                StartCoroutine(DoSwap(second, first, true));
            }
            else
            {
                destroyMatches.DestroyMatchedTiles();
            }
        }
        else { currentState = GameState.move; }
    }

    public IEnumerator RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allTiles[i, k] != null)
                        {
                            Dots dot = allTiles[i, k].GetComponent<Dots>();
                            dot.row = j;
                            allTiles[i, j] = allTiles[i, k];
                            allTiles[i, k] = null;
                            dot.CheckPosition();
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.3f);
        Spawning();
    }

    void Spawning()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] == null)
                {
                    int tileIndex = Random.Range(0, tilePrefab.Length);
                    GameObject spawnedTile = Instantiate(tilePrefab[tileIndex], new Vector2(i, height + offset), Quaternion.identity);
                    spawnedTile.transform.parent = dotParent;
                    Dots dot = spawnedTile.GetComponent<Dots>();
                    dot.column = i;
                    dot.row = j;
                    allTiles[i, j] = spawnedTile;
                    dot.CheckPosition();
                }
            }
        }
        StartCoroutine(CheckBoardRoutine());
    }

    IEnumerator CheckBoardRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (findMatches.Matches())
        {
            destroyMatches.DestroyMatchedTiles();
        }
        else
        {
            // YENİ: Patlamalar bitti, ekranda hiç hamle kaldı mı bakıyoruz
            if (!findMatches.IsMovePossible())
            {
                Debug.Log("Hamle kalmadı! Tahta karıştırılıyor...");
                ShuffleBoard();
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                currentState = GameState.move;
            }
        }
    }

    // YENİ: Tahtadaki taşları birbirleriyle karıştırır
    void ShuffleBoard()
    {
        currentState = GameState.wait;

        // Sahadaki tüm aktif taşları bir listeye topla
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] != null) pieces.Add(allTiles[i, j]);
            }
        }

        int shuffleIterations = 0;
        bool shuffleValid = false;

        // En az bir hamle yapılabilir hale gelene kadar karıştırmayı dene
        while (!shuffleValid && shuffleIterations < 20)
        {
            shuffleIterations++;

            // Listeyi karıştır (Fisher-Yates Algoritması)
            for (int i = 0; i < pieces.Count; i++)
            {
                GameObject temp = pieces[i];
                int randomIndex = Random.Range(i, pieces.Count);
                pieces[i] = pieces[randomIndex];
                pieces[randomIndex] = temp;
            }

            // Karıştırılan taşları matrise (Grid) yeniden dağıt
            int index = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (index < pieces.Count)
                    {
                        allTiles[i, j] = pieces[index];
                        Dots dot = allTiles[i, j].GetComponent<Dots>();
                        dot.column = i;
                        dot.row = j;
                        index++;
                    }
                }
            }

            // Karışım sonucunda "hazırda hemen patlayacak" 3'lü oluştuysa veya hala "hiç hamle yoksa" tekrar karıştır
            if (!findMatches.Matches() && findMatches.IsMovePossible())
            {
                shuffleValid = true;
            }
        }

        // Taşları yeni yerlerine pürüzsüzce kaydır
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allTiles[i, j] != null)
                {
                    allTiles[i, j].GetComponent<Dots>().CheckPosition();
                }
            }
        }

        // Oyuncuya kontrolü geri ver
        currentState = GameState.move;
    }
}