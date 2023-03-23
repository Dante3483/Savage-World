using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="newStructure", menuName ="Structure")]
public class Structure : ScriptableObject
{
    [Serializable]
    public class ColorAndBlock
    {
        public Color ColorOnTemplate;
        public BlockSO BlockOnTemplate;
        public int CountOfBlocks;
    }

    public Sprite StructureTemplate;
    public bool ClearPrevData = false;
    public List<ColorAndBlock> colorsAndBlocks;

    private void OnValidate()
    {
        if (ClearPrevData)
        {
            colorsAndBlocks = new List<ColorAndBlock>();
            Dictionary<Color, int> countOfColors = new Dictionary<Color, int>();
            Color color;
            for (int x = 0; x < StructureTemplate.texture.width; x++)
            {
                for (int y = 0; y < StructureTemplate.texture.height; y++)
                {
                    color = StructureTemplate.texture.GetPixel(x, y);
                    countOfColors.TryGetValue(color, out var currentCount);
                    countOfColors[color] = currentCount + 1;
                }
            }
            foreach (var countOfColor in countOfColors)
            {
                colorsAndBlocks.Add(new ColorAndBlock
                {
                    ColorOnTemplate = countOfColor.Key,
                    CountOfBlocks = countOfColor.Value
                });
            }
            ClearPrevData = false;
        }
    }
}
