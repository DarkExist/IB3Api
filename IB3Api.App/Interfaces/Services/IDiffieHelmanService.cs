using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
    public interface IDiffieHelmanService
    {
        public BigInteger GetRandomBigInteger(int bits);
        public BigInteger GenerateRandomPrime(int bits);
        public BigInteger FindPrimitiveRoot(BigInteger p);

	}
}
