using System;

namespace DEF
{
    [Serializable]
    public class BigNumberSerialized
    {
        public string fullBigNumberString;

        public BigNumber value
        {
            get => new BigNumber(this.fullBigNumberString);
            set => this.fullBigNumberString = value.ToString();
        }

        public BigNumberSerialized(BigNumber bn)
        {
            this.value = bn;
        }

        public BigNumberSerialized()
        {
            this.value = BigNumber.zero;
        }
    }
}