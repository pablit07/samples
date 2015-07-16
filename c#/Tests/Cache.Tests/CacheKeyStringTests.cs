using System;
using Cache.ByteArrayConverters;
using NUnit.Framework;

namespace Cache.Tests
{
	[TestFixture]
	public class CacheKeyStringTests
	{
		private readonly IByteArrayConverter<string> _converter = new StringByteArrayConverter();

		[Test]
		public void TestEmptyString()
		{
			object key = String.Empty;
			int nSets = 1;
			int expected = 0;

			int actual = KeyHelper.GetBlockIdFromBits(key, nSets);

			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void TestSimpleString1()
		{
			var key = new String(new[] { (char)1 });
			var nSets = 1;
			var expected = 1;

			var actual = KeyHelper.GetBlockIdFromBits(key, nSets, _converter);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestSimpleString2()
		{
			var key = "1";
			var nSets = 1;
			var expected = 1;

			var actual = KeyHelper.GetBlockIdFromBits(key, nSets, _converter);

			Assert.LessOrEqual(expected, actual);
		}

		[Test]
		public void TestComplexString1()
		{
			var key = new String(new[] {(char) 255});
			var nSets = 8;
			var expected = 255;

			var actual = KeyHelper.GetBlockIdFromBits(key, nSets, _converter);

			Assert.AreEqual(expected, actual);
		}
	}
}