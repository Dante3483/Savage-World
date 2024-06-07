using Items;
using System.Text;

public class StackableItemSO : ItemSO
{
    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public StackableItemSO()
    {
        _isStackable = true;
        _stackSize = 9999;
    }

    public override StringBuilder GetFullDescription(int quantity)
    {
        _fullDescriptionStringBuilder.Clear();
        _fullDescriptionStringBuilder.Append(ColoredName).Append($" ({quantity})").AppendLine();
        _fullDescriptionStringBuilder.Append(_itemRarity.Name).AppendLine();
        _fullDescriptionStringBuilder.Append(_using).AppendLine();
        _fullDescriptionStringBuilder.Append(_description).AppendLine();
        return _fullDescriptionStringBuilder;
    }
    #endregion

    #region Private Methods

    #endregion
}