using System.Collections.Concurrent;
using System.Numerics;

namespace IB3Api.Api
{
	public static class DataHolder
	{
		public static ConcurrentDictionary<
			Tuple<BigInteger, BigInteger, BigInteger, BigInteger>,
			Tuple<BigInteger, BigInteger>> SecurityKeysHolder { get; set; } = new();
	}
}
