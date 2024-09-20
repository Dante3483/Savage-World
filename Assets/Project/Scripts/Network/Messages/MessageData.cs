using System.Text;

namespace SavageWorld.Runtime.Network.Messages
{
    public struct MessageData
    {
        #region Fields
        public int IntNumber1;
        public int IntNumber2;
        public int IntNumber3;
        public int IntNumber4;
        public long LongNumber1;
        public long LongNumber2;
        public float FloatNumber1;
        public float FloatNumber2;
        public float FloatNumber3;
        public float FloatNumber4;
        public bool Bool1;
        public bool Bool2;
        public bool Bool3;
        public bool Bool4;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override string ToString()
        {
            StringBuilder builder = new();
            builder.AppendLine($"Int 1: {IntNumber1}");
            builder.AppendLine($"Int 2: {IntNumber2}");
            builder.AppendLine($"Int 3: {IntNumber3}");
            builder.AppendLine($"Int 4: {IntNumber4}");
            builder.AppendLine($"Long 1: {LongNumber1}");
            builder.AppendLine($"Long 2: {LongNumber2}");
            builder.AppendLine($"Float 1: {FloatNumber1}");
            builder.AppendLine($"Float 2: {FloatNumber2}");
            builder.AppendLine($"Float 3: {FloatNumber3}");
            builder.AppendLine($"Float 4: {FloatNumber4}");
            builder.AppendLine($"Bool 1: {Bool1}");
            builder.Append($"Bool 2: {Bool2}");
            builder.Append($"Bool 3: {Bool3}");
            builder.Append($"Bool 4: {Bool4}");
            return builder.ToString();
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
