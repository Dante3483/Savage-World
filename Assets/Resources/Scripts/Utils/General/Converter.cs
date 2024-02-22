using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Converter
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public static void FromIntToCharArray(int value, char[] charArray)
    {
        int digit;
        int i = charArray.Length;
        while (i != 0)
        {
            charArray[--i] = StaticInfo.DigitsCharArray[10];
            if (value > 0)
            {
                digit = value % 10;
                value /= 10;
                charArray[i] = StaticInfo.DigitsCharArray[digit];
            }
        }
    }
    #endregion
}
