using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlant", menuName = "Blocks/Plant")]
public class PlantSO : BlockSO
{
    #region Private fields
    [SerializeField] private PlantsID _id;
    [SerializeField] private BiomesID _biomeId;
    [SerializeField] private List<BlockSO> _allowedToSpawnOn;
    [SerializeField] private bool _canGrow = false;
    [SerializeField] private bool _isBottomSpawn = true;
    [SerializeField] private bool _isTopSpawn = false;
    [SerializeField] private int _chanceToSpawn;
    [SerializeField] private int _chanceToGrow;
    [SerializeField] private bool _isBiomeSpecified;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public PlantsID Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public List<BlockSO> AllowedToSpawnOn
    {
        get
        {
            return _allowedToSpawnOn;
        }

        set
        {
            _allowedToSpawnOn = value;
        }
    }

    public bool CanGrow
    {
        get
        {
            return _canGrow;
        }

        set
        {
            _canGrow = value;
        }
    }

    public bool IsBottomSpawn
    {
        get
        {
            return _isBottomSpawn;
        }

        set
        {
            _isBottomSpawn = value;
        }
    }

    public bool IsTopSpawn
    {
        get
        {
            return _isTopSpawn;
        }

        set
        {
            _isTopSpawn = value;
        }
    }

    public int ChanceToSpawn
    {
        get
        {
            return _chanceToSpawn;
        }

        set
        {
            _chanceToSpawn = value;
        }
    }

    public int ChanceToGrow
    {
        get
        {
            return _chanceToGrow;
        }

        set
        {
            _chanceToGrow = value;
        }
    }

    public BiomesID BiomeId
    {
        get
        {
            return _biomeId;
        }

        set
        {
            _biomeId = value;
        }
    }

    public bool IsBiomeSpecified
    {
        get
        {
            return _isBiomeSpecified;
        }

        set
        {
            _isBiomeSpecified = value;
        }
    }
    #endregion

    #region Methods
    public PlantSO()
    {
        Type = BlockTypes.Plant;
    }

    public override ushort GetId()
    {
        return (ushort)Id;
    }
    #endregion
}
