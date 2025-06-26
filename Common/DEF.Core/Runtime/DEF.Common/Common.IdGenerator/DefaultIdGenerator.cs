

using System;
using System.Threading;

namespace DEF.IdGenerator
{
    // 默认实现
    public class DefaultIdGenerator : IIdGenerator
    {
        ISnowWorker SnowWorker { get; set; }

        public DefaultIdGenerator(IdGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException("options error.");
            }

            // 1.BaseTime
            if (options.BaseTime < DateTime.Now.AddYears(-50) || options.BaseTime > DateTime.Now)
            {
                throw new ArgumentException("BaseTime error.");
            }

            // 2.WorkerIdBitLength
            int maxLength = options.TimestampType == 0 ? 22 : 31; // （秒级时间戳时放大到31位）
            if (options.WorkerIdBitLength <= 0)
            {
                throw new ArgumentException("WorkerIdBitLength error.(range:[1, 21])");
            }
            if (options.DataCenterIdBitLength + options.WorkerIdBitLength + options.SeqBitLength > maxLength)
            {
                throw new ArgumentException("error：DataCenterIdBitLength + WorkerIdBitLength + SeqBitLength <= " + maxLength);
            }

            // 3.WorkerId & DataCenterId
            var maxWorkerIdNumber = (1 << options.WorkerIdBitLength) - 1;
            if (maxWorkerIdNumber == 0)
            {
                maxWorkerIdNumber = 63;
            }
            if (options.WorkerId < 0 || options.WorkerId > maxWorkerIdNumber)
            {
                throw new ArgumentException("WorkerId error. (range:[0, " + maxWorkerIdNumber + "]");
            }

            var maxDataCenterIdNumber = (1 << options.DataCenterIdBitLength) - 1;
            if (options.DataCenterId < 0 || options.DataCenterId > maxDataCenterIdNumber)
            {
                throw new ArgumentException("DataCenterId error. (range:[0, " + maxDataCenterIdNumber + "]");
            }

            // 4.SeqBitLength
            if (options.SeqBitLength < 2 || options.SeqBitLength > 21)
            {
                throw new ArgumentException("SeqBitLength error. (range:[2, 21])");
            }

            // 5.MaxSeqNumber
            var maxSeqNumber = (1 << options.SeqBitLength) - 1;
            if (maxSeqNumber == 0)
            {
                maxSeqNumber = 63;
            }
            if (options.MaxSeqNumber < 0 || options.MaxSeqNumber > maxSeqNumber)
            {
                throw new ArgumentException("MaxSeqNumber error. (range:[1, " + maxSeqNumber + "]");
            }

            // 6.MinSeqNumber
            if (options.MinSeqNumber < 5 || options.MinSeqNumber > maxSeqNumber)
            {
                throw new ArgumentException("MinSeqNumber error. (range:[5, " + maxSeqNumber + "]");
            }

            // 7.TopOverCostCount
            if (options.TopOverCostCount < 0 || options.TopOverCostCount > 10000)
            {
                throw new ArgumentException("TopOverCostCount error. (range:[0, 10000]");
            }

            switch (options.Method)
            {
                case 2:
                    SnowWorker = new SnowWorkerM2(options);
                    break;
                default:
                    if (options.DataCenterIdBitLength == 0 && options.TimestampType == 0)
                    {
                        SnowWorker = new SnowWorkerM1(options);
                    }
                    else
                    {
                        SnowWorker = new SnowWorkerM3(options);
                    }
                    break;
            }

            if (options.Method != 2)
            {
                //Thread.Sleep(500);
            }
        }

        public long NewLong()
        {
            return SnowWorker.NextId();
        }
    }
}