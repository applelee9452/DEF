using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DEF
{
    [MemoryDiagnoser]
    public class HashFunctionsBenchmark
    {
        private readonly string _inputData;

        public HashFunctionsBenchmark()
        {
            _inputData = new string('y', 1000000);
        }

        [Benchmark]
        public byte[] MD5Hash()
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(_inputData));
            }
        }

        [Benchmark]
        public byte[] SHA1Hash()
        {
            using (var sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(_inputData));
            }
        }

        [Benchmark]
        public byte[] SHA256Hash()
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(_inputData));
            }
        }
    }
}
