using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
    public interface IDiffieHelman
    {
        public BigInteger GetRandomBigInteger(int bits);
        public BigInteger GenerateRandomPrime(int bits);
    }
}
