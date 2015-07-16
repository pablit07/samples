using System;
using System.Numerics;
using Cache.ByteArrayConverters;

namespace Cache
{
	using CacheMemoryIndex = Int32;

	/// <summary>
	///     KeyHelper contains methods for mapping a key to an index by looking at n number of
	///     the least significant bits.
	/// </summary>
	public static class KeyHelper
	{
		/// <summary>
		///     Gets some number of least significant bits as the index, using the default method of converting an object to binary
		///     form
		/// </summary>
		/// <param name="key">string key, converted to a byte array</param>
		/// <param name="n">number of sig bits to get</param>
		/// <returns>Specified number of sig bits as an int</returns>
		public static CacheMemoryIndex GetBlockIdFromBits<TKey>(TKey key, int n)
		{
			return GetBlockIdFromBits(key, n, new BinaryFormatterByteArrayConverter<TKey>());
		}

		/// <summary>
		///     Gets some number of least significant bits as the index, using a client-provided method of converting an object to
		///     binary form
		/// </summary>
		/// <param name="key">string key, converted to a byte array</param>
		/// <param name="n">number of sig bits to get</param>
		/// <param name="byteArrayConverter">A converter to get the key object as a byte array</param>
		/// <returns>Specified number of sig bits as an int</returns>
		public static CacheMemoryIndex GetBlockIdFromBits<TKey>(TKey key, int n, IByteArrayConverter<TKey> byteArrayConverter)
		{
			// IsClass checks if it is not a value type, thus null check will be legal
			// ReSharper disable once CompareNonConstrainedGenericWithNull
			if (key.GetType().IsClass && key == null) throw new ArgumentNullException();

			var bytes = byteArrayConverter.ToByteArray(key);

			if (bytes.Length == 0) return 0;

			return _GetBlockIdFromBits(bytes, n);
		}

		/// <summary>
		///     Gets some number of least significant bits as the index
		/// </summary>
		/// <param name="key"></param>
		/// <param name="n">number of sig bits to get, 0 should always return 0</param>
		/// <returns>Specified number of sig bits as an int</returns>
		private static CacheMemoryIndex _GetBlockIdFromBits(byte[] key, int n)
		{
			if (key == null) throw new ArgumentNullException();
			// n must be 0 or greater to have meaning
			// n must not be greater than or equal to the total number of bits provided
			if (n < 0 || n >= (key.Length*8)) throw new ArgumentOutOfRangeException();

			// use a BigInteger because BigInteger supports the bitwise and operator
			var bigIntegerKey = new BigInteger(key);
			// use left shift + bitwise and to mask out all but n-most significant bits
			// inspired by http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte
			var bigIntegerResult = (bigIntegerKey & ((1 << n) - 1));
			// cast to mem index type and return
			return (CacheMemoryIndex) bigIntegerResult;
		}
	}
}