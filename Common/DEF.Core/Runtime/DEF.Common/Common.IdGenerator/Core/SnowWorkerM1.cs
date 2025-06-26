

using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.IdGenerator
{
    // 雪花漂移算法
    internal class SnowWorkerM1 : ISnowWorker
    {
        // 基础时间
        protected readonly DateTime BaseTime;

        // 机器码
        protected readonly ushort WorkerId = 0;

        // 机器码位长
        protected readonly byte WorkerIdBitLength = 0;

        // 自增序列数位长
        protected readonly byte SeqBitLength = 0;

        // 最大序列数（含）
        protected readonly int MaxSeqNumber = 0;

        // 最小序列数（含）
        protected readonly ushort MinSeqNumber = 0;

        // 最大漂移次数（含）
        protected int TopOverCostCount = 0;

        protected byte _TimestampShift = 0;
        protected static object _SyncLock = new object();

        protected ushort _CurrentSeqNumber = 0;
        protected long _LastTimeTick = 0; // -1L
        protected long _TurnBackTimeTick = 0; // -1L;
        protected byte _TurnBackIndex = 0;
        protected bool _IsOverCost = false;
        protected int _OverCostCountInOneTerm = 0;

#if DEBUG
        protected int _GenCountInOneTerm = 0;
        protected int _TermIndex = 0;
#endif

        public Action<OverCostActionArg> GenAction { get; set; }

        //private static long _StartTimeTick = 0;
        //private static long _BaseTimeTick = 0;

        public SnowWorkerM1(IdGeneratorOptions options)
        {
            // 1.BaseTime
            if (options.BaseTime != DateTime.MinValue)
            {
                BaseTime = options.BaseTime;
            }

            // 2.WorkerIdBitLength
            if (options.WorkerIdBitLength == 0)
            {
                WorkerIdBitLength = 6;
            }
            else
            {
                WorkerIdBitLength = options.WorkerIdBitLength;
            }

            // 3.WorkerId
            WorkerId = options.WorkerId;

            // 4.SeqBitLength
            if (options.SeqBitLength == 0)
            {
                SeqBitLength = 6;
            }
            else
            {
                SeqBitLength = options.SeqBitLength;
            }

            // 5.MaxSeqNumber
            if (options.MaxSeqNumber <= 0)
            {
                MaxSeqNumber = (1 << SeqBitLength) - 1;
            }
            else
            {
                MaxSeqNumber = options.MaxSeqNumber;
            }

            // 6.MinSeqNumber
            MinSeqNumber = options.MinSeqNumber;

            // 7.Others
            TopOverCostCount = options.TopOverCostCount;
            //if (TopOverCostCount == 0)
            //{
            //    TopOverCostCount = 2000;
            //}

            _TimestampShift = (byte)(WorkerIdBitLength + SeqBitLength);
            _CurrentSeqNumber = options.MinSeqNumber;

            //_BaseTimeTick = BaseTime.Ticks;
            //_StartTimeTick = (long)(DateTime.UtcNow.Subtract(BaseTime).TotalMilliseconds) - Environment.TickCount;
        }

#if DEBUG
        private void DoGenIdAction(OverCostActionArg arg)
        {
            //return;
            Task.Run(() =>
            {
                GenAction(arg);
            });
        }

        private void BeginOverCostAction(in long useTimeTick)
        {
            if (GenAction == null)
            {
                return;
            }

            DoGenIdAction(new OverCostActionArg(
                WorkerId,
                useTimeTick,
                1,
                _OverCostCountInOneTerm,
                _GenCountInOneTerm,
                _TermIndex));
        }

        private void EndOverCostAction(in long useTimeTick)
        {
            //if (_TermIndex > 10000)
            //{
            //    _TermIndex = 0;
            //}

            if (GenAction == null)
            {
                return;
            }

            DoGenIdAction(new OverCostActionArg(
                WorkerId,
                useTimeTick,
                2,
                _OverCostCountInOneTerm,
                _GenCountInOneTerm,
                _TermIndex));
        }

        private void BeginTurnBackAction(in long useTimeTick)
        {
            if (GenAction == null)
            {
                return;
            }

            DoGenIdAction(new OverCostActionArg(
            WorkerId,
            useTimeTick,
            8,
            0,
            0,
            _TurnBackIndex));
        }

        private void EndTurnBackAction(in long useTimeTick)
        {
            if (GenAction == null)
            {
                return;
            }

            DoGenIdAction(new OverCostActionArg(
            WorkerId,
            useTimeTick,
            9,
            0,
            0,
            _TurnBackIndex));
        }
#endif

        protected virtual long NextOverCostId()
        {
            long currentTimeTick = GetCurrentTimeTick();

            if (currentTimeTick > _LastTimeTick)
            {
#if DEBUG
                EndOverCostAction(currentTimeTick);
                _GenCountInOneTerm = 0;
#endif
                _LastTimeTick = currentTimeTick;
                _CurrentSeqNumber = MinSeqNumber;
                _IsOverCost = false;
                _OverCostCountInOneTerm = 0;

                return CalcId(_LastTimeTick);
            }

            if (_OverCostCountInOneTerm >= TopOverCostCount)
            {
#if DEBUG
                EndOverCostAction(currentTimeTick);
                _GenCountInOneTerm = 0;
#endif
                // TODO: 在漂移终止，等待时间对齐时，如果发生时间回拨较长，则此处可能等待较长时间。可优化为：在漂移终止时增加时间回拨应对逻辑。（该情况发生概率低，暂不处理）

                _LastTimeTick = GetNextTimeTick();
                _CurrentSeqNumber = MinSeqNumber;
                _IsOverCost = false;
                _OverCostCountInOneTerm = 0;

                return CalcId(_LastTimeTick);
            }

            if (_CurrentSeqNumber > MaxSeqNumber)
            {
#if DEBUG
                _GenCountInOneTerm++;
#endif
                _LastTimeTick++;
                _CurrentSeqNumber = MinSeqNumber;
                _IsOverCost = true;
                _OverCostCountInOneTerm++;

                return CalcId(_LastTimeTick);
            }

#if DEBUG
            _GenCountInOneTerm++;
#endif
            return CalcId(_LastTimeTick);
        }

        protected virtual long NextNormalId()
        {
            long currentTimeTick = GetCurrentTimeTick();

            if (currentTimeTick < _LastTimeTick)
            {
                if (_TurnBackTimeTick < 1)
                {
                    _TurnBackTimeTick = _LastTimeTick - 1;

                    _TurnBackIndex++;
                    // 每毫秒序列数的前5位是预留位，0用于手工新值，1-4是时间回拨次序
                    // 支持4次回拨次序（避免回拨重叠导致ID重复），可无限次回拨（次序循环使用）。
                    if (_TurnBackIndex > 4)
                    {
                        _TurnBackIndex = 1;
                    }

#if DEBUG
                    BeginTurnBackAction(_TurnBackTimeTick);
#endif
                }

                //Thread.Sleep(1);
                return CalcTurnBackId(_TurnBackTimeTick);
            }

            // 时间追平时，_TurnBackTimeTick清零
            if (_TurnBackTimeTick > 0)
            {
#if DEBUG
                EndTurnBackAction(_TurnBackTimeTick);
#endif
                _TurnBackTimeTick = 0;
            }

            if (currentTimeTick > _LastTimeTick)
            {
                _LastTimeTick = currentTimeTick;
                _CurrentSeqNumber = MinSeqNumber;

                return CalcId(_LastTimeTick);
            }

            if (_CurrentSeqNumber > MaxSeqNumber)
            {
#if DEBUG
                BeginOverCostAction(currentTimeTick);
                _TermIndex++;
                _GenCountInOneTerm = 1;
#endif
                _OverCostCountInOneTerm = 1;
                _LastTimeTick++;
                _CurrentSeqNumber = MinSeqNumber;
                _IsOverCost = true;

                return CalcId(_LastTimeTick);
            }

            return CalcId(_LastTimeTick);
        }

        protected virtual long CalcId(long useTimeTick)
        {
            var result = ((useTimeTick << _TimestampShift) +
                ((long)WorkerId << SeqBitLength) +
                (uint)_CurrentSeqNumber);

            _CurrentSeqNumber++;
            return result;
        }

        protected virtual long CalcTurnBackId(long useTimeTick)
        {
            var result = ((useTimeTick << _TimestampShift) +
                ((long)WorkerId << SeqBitLength) + _TurnBackIndex);

            _TurnBackTimeTick--;
            return result;
        }

        protected virtual long GetCurrentTimeTick()
        {
            //return (long)(DateTime.UtcNow - BaseTime).Ticks;
            //return (long)(_StartTimeTick + Environment.TickCount);
            return (long)(DateTime.UtcNow - BaseTime).TotalMilliseconds;
        }

        protected virtual long GetNextTimeTick()
        {
            long tempTimeTicker = GetCurrentTimeTick();

            while (tempTimeTicker <= _LastTimeTick)
            {
                //Thread.Sleep(1);
                SpinWait.SpinUntil(() => false, 1);
                tempTimeTicker = GetCurrentTimeTick();
            }

            return tempTimeTicker;
        }

        public virtual long NextId()
        {
            lock (_SyncLock)
            {
                return _IsOverCost ? NextOverCostId() : NextNormalId();
            }
        }
    }
}