using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(OutlineController))]
public class PickableItem : MonoBehaviour, IInteractiveObject
{
    public SpriteRenderer SpriteRenderer;
    public BoxCollider2D BoxCollider;
    public PickableItemsID ID;
    public ObjectType Type;
    public TerrainLevelID LevelID;
    public List<Sprite> Sprites;
    public ItemSO Drop;
    public List<BiomesID> AllowedToSpawnBiomes;
    public int ChanceToSpawn;

    public PickableItem()
    {
        Type = ObjectType.PickableItem;
        AllowedToSpawnBiomes = new List<BiomesID>()
        {
            BiomesID.Ocean,
            BiomesID.Desert,
            BiomesID.Plains,
            BiomesID.Meadow,
            BiomesID.Forest,
            BiomesID.Swamp,
            BiomesID.ConiferousForest,
        };
    }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        CheckDownSide();
    }

    private void CheckDownSide()
    {
        if (!GameManager.Instance.World.IsAdjacentBlockSolid(transform.position, Vector2Int.down))
        {
            GameManager.Instance.Player.GetComponent<Interactions>().CreateDrop(transform.position - new Vector3(0.5f, 0.5f), Drop, 1, false);
            Destroy(gameObject);
        }
    }

    public Sprite GetRandomSprite()
    {
        return Sprites[GameManager.Instance.World.RandomVar.Next(0, Sprites.Count)];
    }

    public void HandleInteraction()
    {
        GameManager.Instance.Player.GetComponent<Interactions>().CreateDrop(transform.position - new Vector3(0.5f, 0.5f), Drop, 1, false);
        Destroy(gameObject);
    }
}
