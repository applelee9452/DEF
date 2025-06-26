#if !DEF_CLIENT
using MongoDB.Bson.Serialization.Attributes;
#endif
using ProtoBuf;
using System;

namespace DEF
{
    [ProtoContract]
#if !DEF_CLIENT
    [BsonIgnoreExtraElements]
#endif
    public struct SyncBigNumber
    {
        [ProtoIgnore]
#if !DEF_CLIENT
        [BsonIgnore]
#endif
        public BigNumber Num
        {
            get
            {
                return _num;
            }
            set
            {
                _num = value;

                var num_str = _num.ToString(BigNumber.FORMAT_FULL);
                if (_numStr != num_str)
                {
                    _numStr = num_str;
                }
            }
        }

        [ProtoIgnore]
#if !DEF_CLIENT
        [BsonIgnore]
#endif
        BigNumber _num { get; set; }

        [ProtoMember(1)]
#if !DEF_CLIENT
        [BsonElement]
#endif
        public string NumStr
        {
            get
            {
                return _numStr;
            }
            set
            {
                _numStr = value;

                var num_str = _num.ToString(BigNumber.FORMAT_FULL);
                if (_numStr != num_str)
                {
                    if (string.IsNullOrEmpty(_numStr))
                    {
                        _num = BigNumber.zero;
                    }
                    else
                    {
                        _num = new BigNumber(_numStr);
                    }
                }
            }
        }

        [ProtoIgnore]
#if !DEF_CLIENT
        [BsonIgnore]
#endif
        string _numStr { get; set; }

        public static implicit operator SyncBigNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                var v = new SyncBigNumber
                {
                    Num = BigNumber.zero
                };

                return v;
            }
            else
            {
                var v = new SyncBigNumber
                {
                    Num = new BigNumber(str)
                };

                return v;
            }
        }

        public static implicit operator SyncBigNumber(BigNumber num)
        {
            var v = new SyncBigNumber
            {
                Num = num
            };

            return v;
        }
    }
}