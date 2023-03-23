using UnityEngine;

[CreateAssetMenu(fileName = "newHole", menuName = "Hole")]
public class Hole : ScriptableObject
{
    public BlockSO fillBlock;
    public float size;
    public float freq;
}
