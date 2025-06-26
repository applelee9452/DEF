

using System;

namespace DEF.IdGenerator
{
    // 这是一个调用的例子，默认情况下，单机集成者可以直接使用 NextId()。
    public class IdHelper
    {
        static IIdGenerator _IdGenInstance = null;
        static IIdGenerator IdGenInstance => _IdGenInstance;

        // 设置参数，建议程序初始化时执行一次
        public static void SetIdGenerator(IdGeneratorOptions options)
        {
            _IdGenInstance = new DefaultIdGenerator(options);
        }

        // 生成新的Id
        // 调用本方法前，请确保调用了 SetIdGenerator 方法做初始化。
        public static long NextId()
        {
            //if (_IdGenInstance == null)
            //{
            //    lock (_IdGenInstance)
            //    {
            //        if (_IdGenInstance == null)
            //        {
            //            _IdGenInstance = new DefaultIdGenerator(
            //                new IdGeneratorOptions() { WorkerId = 0 }
            //                );
            //        }
            //    }
            //}

            if (_IdGenInstance == null) throw new ArgumentException("Please initialize Yitter.IdGeneratorOptions first.");

            return _IdGenInstance.NewLong();
        }
    }
}