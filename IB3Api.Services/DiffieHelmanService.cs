using System.Numerics;
using System;
using IB3Api.App.Interfaces.Services;

namespace IB3Api.Services
{
    public class DiffieHelmanService : IDiffieHelmanService
	{
		Random random = new Random();
		public BigInteger GetRandomBigInteger(int bits)
		{
			byte[] bytes = new byte[bits / 8];
			random.NextBytes(bytes);

			bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
			return new BigInteger(bytes);
		}

		public BigInteger GenerateRandomPrime(int bits)
		{
			Random rand = new Random();
			BigInteger prime;

			do
			{
				prime = GetRandomBigInteger(bits);
			} while (!IsProbablyPrime(prime, 10)); // Проверка на простоту

			return prime;
		}

		

		public BigInteger FindPrimitiveRoot(BigInteger p)
		{
			if (p <= 2)
			{
				return 1;
			}

			BigInteger phi = EilerCalculate(p);
			List<BigInteger> factors = Factorize(phi);

			for (BigInteger g = 2; g < p; g++)
			{
				if (IsPrimitiveRoot(g, p, factors))
				{
					return g;
				}
			}

			throw new Exception("Первообразный корень не найден.");
		}

		private List<BigInteger> Factorize(BigInteger n)
		{
			List<BigInteger> factors = new List<BigInteger>();
			BigInteger i = 2;

			while (i * i <= n)
			{
				if (n % i == 0)
				{
					factors.Add(i);
					n /= i;
				}
				else
				{
					i++;
				}
			}

			if (n > 1)
			{
				factors.Add(n);
			}

			return factors;
		}
		private bool IsPrimitiveRoot(BigInteger g, BigInteger p, List<BigInteger> factors)
		{
			BigInteger phi = EilerCalculate(p);

			foreach (BigInteger factor in factors)
			{
				// Проверяем g^((p-1)/factor) mod p != 1
				if (BigInteger.ModPow(g, phi / factor, p) == 1)
				{
					return false;
				}
			}

			return true;
		}

		private	BigInteger EilerCalculate(BigInteger n)
		{
			return n - 1;
		}

		// Тест простоты Миллера-Рабина
		private bool IsProbablyPrime(BigInteger number, int k)
		{
			if (number < 2) return false;
			if (number != 2 && number % 2 == 0) return false;

			BigInteger d = number - 1;
			int r = 0;

			while (d % 2 == 0)
			{
				d /= 2;
				r++;
			}

			Random rand = new Random();
			for (int i = 0; i < k; i++)
			{
				BigInteger a = 2 + (BigInteger)(rand.NextDouble() * (double)(number - 4));
				if (!MillerTest(a, d, number, r))
					return false;
			}

			return true;
		}

		private bool MillerTest(BigInteger a, BigInteger d, BigInteger n, int r)
		{
			BigInteger x = BigInteger.ModPow(a, d, n);
			if (x == 1 || x == n - 1)
				return true;

			for (int i = 1; i < r; i++)
			{
				x = BigInteger.ModPow(x, 2, n);
				if (x == 1) return false;
				if (x == n - 1) return true;
			}

			return false;
		}
	}
}
