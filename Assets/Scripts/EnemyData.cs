using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data", order = 51)]
public class EnemyData : ScriptableObject
{
    public int maxHealth ;
    public int damage  ;
    public Sprite idleSprite;
    public Sprite attackSprite;
    public Sprite impactSprite;
    public Sprite deadSprite;
    public GameObject AttackObjectPrefab;
}
        
    

