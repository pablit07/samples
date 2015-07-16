using Cache.ByteArrayConverters;
using NUnit.Framework;

namespace Cache.Tests
{
	[TestFixture]
	public class CacheKeyIntTests
	{
		private readonly IntByteArrayConverter _converter = new IntByteArrayConverter();


		[Test]
		public void TestKey1Set1()
		{
			int bit = 1;

			int expected = 1;
			int actual = KeyHelper.GetBlockIdFromBits(bit, 1, _converter);

			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void TestKey1Set2()
		{
			int key = 1;
			int expected = 1;
			int nSets = 2;

			int actual = KeyHelper.GetBlockIdFromBits(key, nSets, _converter);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TestKey2Set1()
		{
			int key = 2;
			int expected = 0;
			int nSets = 1;

			int actual = KeyHelper.GetBlockIdFromBits(key, nSets, _converter);

			Assert.AreEqual(expected, actual);
		}


		[Test]
		public void TestKey2Set2()
		{
			int key = 2;
			int expected = 2;
			int nSets = 2;

			int actual = KeyHelper.GetBlockIdFromBits(key, nSets, _converter);

			Assert.AreEqual(expected, actual);
		}


		[Test]
		[ExpectedException]
		public void TestOverflow()
		{
			int key = 1;
			int nSets = 32;

			KeyHelper.GetBlockIdFromBits(key, nSets, _converter);
		}
	}
}