using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Cache.ByteArrayConverters;
using Cache.ReplacementPolicies;
using NUnit.Framework;

namespace Cache.Tests
{
	[TestFixture]
	public class CacheTests4By2
	{
		[SetUp]
		public void BeforeEach()
		{
			_cache = new Cache<string, string>(new Options(4, 2))
			{
				ByteArrayConverter = new StringByteArrayConverter(),
				ReplacementPolicy = new LruPositionReplacementPolicy<string, string>()
			};
		}

		private Cache<string, string> _cache;

		[Test]
		public void TestMany()
		{
			var loops = 100;
			var sizeRange = 10;
			var queue = new List<string>();
			var queueSize = 4;

			var random = new Random(DateTime.Now.Second);

			var str = string.Empty;
			string outValue;

			for (var i = 0; i < loops; i++)
			{
				var chars = new List<char>();
				var length = random.Next(1, sizeRange);
				for (var j = 0; j < length; j++)
				{
					chars.Add((char)random.Next(0, 255));
				}

				str = new String(chars.ToArray());
				_cache.Write(str, str);

				queue.Insert(0, str);
				if (queue.Count > queueSize)
				{
					queue.RemoveAt(queueSize);
					Assert.IsTrue(_cache.Read(queue[0], out outValue));
					Assert.AreEqual(queue[0], outValue);
					Assert.IsTrue(_cache.Read(queue[1], out outValue));
					Assert.AreEqual(queue[1], outValue);
					Assert.IsTrue(_cache.Read(queue[2], out outValue));
					Assert.AreEqual(queue[2], outValue);
					Assert.IsTrue(_cache.Read(queue[3], out outValue));
					Assert.AreEqual(queue[3], outValue);
				}

				Assert.True(_cache.Read(str, out outValue));
			}

			Assert.AreEqual(4, queue.Count);
			Assert.IsTrue(_cache.Read(queue[0], out outValue));
			Assert.AreEqual(queue[0], outValue);
			Assert.IsTrue(_cache.Read(queue[1], out outValue));
			Assert.AreEqual(queue[1], outValue);
			Assert.IsTrue(_cache.Read(queue[2], out outValue));
			Assert.AreEqual(queue[2], outValue);
			Assert.IsTrue(_cache.Read(queue[3], out outValue));
			Assert.AreEqual(queue[3], outValue);
		}
	}


	[TestFixture]
	public class CacheTests2By4
	{
		[SetUp]
		public void BeforeEach()
		{
			_cache = new Cache<string, MockCacheable>(new Options(2, 4));
		}

		private Cache<string, MockCacheable> _cache;


		[Test]
		public void TestReadFromEmpty()
		{
			var dummy = new MockCacheable {Key = "1"};
			MockCacheable actualValue;
			bool actual = _cache.Read(dummy.GetCacheKey(), out actualValue);

			Assert.IsFalse(actual);
			Assert.IsNull(actualValue);
		}


		[Test]
		public void TestWriteOne()
		{
			var dummy = new MockCacheable {Key = "1"};
			MockCacheable actualValue;

			_cache.Write(dummy.GetCacheKey(), dummy);

			bool actual = _cache.Read(dummy.GetCacheKey(), out actualValue);

			Assert.IsTrue(actual);
			Assert.AreEqual(dummy.GetCacheKey(), actualValue.GetCacheKey());
		}

		[Test]
		public void TestWriteSeveralIn1Way()
		{
			_cache = new Cache<string, MockCacheable>(new Options(4, 0));
			MockCacheable dummy1 = new MockCacheable().SetKeyInBytes(13);
			MockCacheable dummy2 = new MockCacheable().SetKeyInBytes(14);
			MockCacheable dummy3 = new MockCacheable().SetKeyInBytes(15);
			MockCacheable dummy4 = new MockCacheable().SetKeyInBytes(16);

			MockCacheable actualValue1;
			MockCacheable actualValue2;
			MockCacheable actualValue3;
			MockCacheable actualValue4;

			_cache.Write(dummy1.GetCacheKey(), dummy1);
			_cache.Write(dummy2.GetCacheKey(), dummy2);
			_cache.Write(dummy3.GetCacheKey(), dummy3);
			_cache.Write(dummy4.GetCacheKey(), dummy4);

			bool actual1 = _cache.Read(dummy1.GetCacheKey(), out actualValue1);
			bool actual2 = _cache.Read(dummy2.GetCacheKey(), out actualValue2);
			bool actual3 = _cache.Read(dummy3.GetCacheKey(), out actualValue3);
			bool actual4 = _cache.Read(dummy4.GetCacheKey(), out actualValue4);

			Assert.IsTrue(actual1);
			Assert.AreEqual(dummy1.Key, actualValue1.Key);
			Assert.IsTrue(actual2);
			Assert.AreEqual(dummy2.Key, actualValue2.Key);
			Assert.IsTrue(actual3);
			Assert.AreEqual(dummy3.Key, actualValue3.Key);
			Assert.IsTrue(actual4);
			Assert.AreEqual(dummy4.Key, actualValue4.Key);
		}

		[Test]
		public void TestWriteSeveralIn2Way()
		{
			_cache = new Cache<string, MockCacheable>(new Options(4, 1));
			MockCacheable dummy1 = new MockCacheable().SetKeyInBytes(1);
			MockCacheable dummy2 = new MockCacheable().SetKeyInBytes(3);
			MockCacheable dummy3 = new MockCacheable().SetKeyInBytes(5);
			MockCacheable dummy4 = new MockCacheable().SetKeyInBytes(7);

			MockCacheable actualValue1;
			MockCacheable actualValue2;
			MockCacheable actualValue3;
			MockCacheable actualValue4;

			_cache.Write(dummy1.GetCacheKey(), dummy1);
			_cache.Write(dummy2.GetCacheKey(), dummy2);
			_cache.Write(dummy3.GetCacheKey(), dummy3);
			_cache.Write(dummy4.GetCacheKey(), dummy4);

			bool actual1 = _cache.Read(dummy1.GetCacheKey(), out actualValue1);
			bool actual2 = _cache.Read(dummy2.GetCacheKey(), out actualValue2);
			bool actual3 = _cache.Read(dummy3.GetCacheKey(), out actualValue3);
			bool actual4 = _cache.Read(dummy4.GetCacheKey(), out actualValue4);

			Assert.IsTrue(actual1);
			Assert.AreEqual(dummy1.Key, actualValue1.Key);
			Assert.IsTrue(actual2);
			Assert.AreEqual(dummy2.Key, actualValue2.Key);
			Assert.IsTrue(actual3);
			Assert.AreEqual(dummy3.Key, actualValue3.Key);
			Assert.IsTrue(actual4);
			Assert.AreEqual(dummy4.Key, actualValue4.Key);
		}

		[Test]
		public void TestWriteTwoOfSameReplacesFirst()
		{
			MockCacheable dummy1 = new MockCacheable {SomeOtherProp = 7}.SetKeyInBytes(100);
			MockCacheable dummy2 = new MockCacheable {SomeOtherProp = 9}.SetKeyInBytes(100);
			MockCacheable actualValue;

			_cache.Write(dummy1.GetCacheKey(), dummy1);
			_cache.Write(dummy2.GetCacheKey(), dummy2);

			bool actual = _cache.Read(dummy1.GetCacheKey(), out actualValue);

			Assert.IsTrue(actual);
			Assert.AreEqual(dummy2.SomeOtherProp, actualValue.SomeOtherProp);
		}


		[Test]
		public void TestMany()
		{
			var _cache = new Cache<string, string>(new Options(2, 4))
			{
				ByteArrayConverter = new StringByteArrayConverter(),
				ReplacementPolicy = new LruPositionReplacementPolicy<string, string>()
			};

			var loops = 100;
			var sizeRange = 10;
			var queue = new List<string>();
			var queueSize = 4;

			var random = new Random(DateTime.Now.Second);

			var str = string.Empty;
			string outValue;

			for (var i = 0; i < loops; i++)
			{
				var chars = new List<char>();
				var length = random.Next(1, sizeRange);
				for (var j = 0; j < length; j++)
				{
					chars.Add((char)random.Next(0, 255));
				}

				str = new String(chars.ToArray());
				_cache.Write(str, str);

				queue.Insert(0, str);
				if (queue.Count > queueSize)
				{
					queue.RemoveAt(queueSize);
				}

				Assert.True(_cache.Read(str, out outValue));
			}

			Assert.AreEqual(4, queue.Count);
			Assert.IsTrue(_cache.Read(queue[0], out outValue));
			Assert.AreEqual(queue[0], outValue);
			Assert.IsTrue(_cache.Read(queue[1], out outValue));
			Assert.AreEqual(queue[1], outValue);
		}
	}


	[TestFixture]
	public class CacheTests1By1
	{
		private Cache<string, string> _cache;

		[SetUp]
		public void BeforeEach()
		{
			_cache = new Cache<string, string>(new Options(1, 1))
			{
				ByteArrayConverter = new StringByteArrayConverter(),
				ReplacementPolicy = new MruReplacementPolicy<string, string>()
			};
		}


		[Test]
		public void TestEvictOne()
		{
			var s1 = new string(new[] {(char) 255});
			var s2 = new string(new[] {(char) 7});

			var index1 = KeyHelper.GetBlockIdFromBits(s1, 1, _cache.ByteArrayConverter);
			var index2 = KeyHelper.GetBlockIdFromBits(s2, 1, _cache.ByteArrayConverter);

			Assert.AreEqual(index1, index2);

			_cache.Write(s1, s1);
			_cache.Write(s2, s2);

			string outValue;
			Assert.IsFalse(_cache.Read(s1, out outValue));
			Assert.IsTrue(_cache.Read(s2, out outValue));
			Assert.AreEqual(s2, outValue);
		}


		[Test]
		public void TestEvictOneLru()
		{
			_cache.ReplacementPolicy = new LruReplacementPolicy<string, string>();

			var s1 = new string(new[] { (char)255 });
			var s2 = new string(new[] { (char)7 });

			var index1 = KeyHelper.GetBlockIdFromBits(s1, 1, _cache.ByteArrayConverter);
			var index2 = KeyHelper.GetBlockIdFromBits(s2, 1, _cache.ByteArrayConverter);

			Assert.AreEqual(index1, index2);

			_cache.Write(s1, s1);
			_cache.Write(s2, s2);

			string outValue;
			Assert.IsFalse(_cache.Read(s1, out outValue));
			Assert.IsTrue(_cache.Read(s2, out outValue));
			Assert.AreEqual(s2, outValue);
		}


		[Test]
		public void TestMany()
		{
			var loops = 100;
			var sizeRange = 10;
			var queue = new List<string>();
			var queueSize = 1;

			var random = new Random(DateTime.Now.Second);

			var str = string.Empty;
			string outValue;

			for (var i = 0; i < loops; i++)
			{
				var chars = new List<char>();
				var length = random.Next(1, sizeRange);
				for (var j = 0; j < length; j++)
				{
					chars.Add((char) random.Next(0, 255));
				}

				str = new String(chars.ToArray());
				_cache.Write(str, str);

				queue.Insert(0, str);
				if (queue.Count > queueSize)
				{
					queue.RemoveAt(queueSize);
				}
				
				Assert.True(_cache.Read(str, out outValue));
			}

			Assert.AreEqual(1, queue.Count);
			Assert.IsTrue(_cache.Read(str, out outValue));
			Assert.AreEqual(queue[0], outValue);

		}
	}


	[TestFixture]
	public class IntBytesToStringTests
	{
		[Test]
		public void TestIt()
		{
			var n = new MockCacheable();
			n.SetKeyInBytes(48);

			string k = n.GetCacheKey();

			Assert.AreEqual("0", k);
		}
	}


	[TestFixture]
	public class ClassAsKeyTests
	{
		class MockCacheableByteArrayConverter : IByteArrayConverter<MockCacheable>
		{
			private byte[] _ToByteArray(string str)
			{
				var bytes = new byte[str.Length * sizeof(char)];
				Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
				return bytes;
			}

			public byte[] ToByteArray(MockCacheable obj)
			{
				return obj.GetCacheKey() == null ? _ToByteArray(String.Empty) : _ToByteArray(obj.GetCacheKey());
			}
		}

		[Test]
		public void TestComplexString1WithComplexConverter()
		{
			var key = new String(new[] { (char)255 });
			var nSets = 8;
			var expected = 255;

			var actual = KeyHelper.GetBlockIdFromBits(new MockCacheable{Key = key}, nSets, new MockCacheableByteArrayConverter());

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TryExtendingKeyClass()
		{
			var cache = new Cache<MockCacheable, MockCacheable>(new Options(4, 2))
			{
				ByteArrayConverter = new MockCacheableByteArrayConverter()
			};

			MockCacheable outValue;
			var result = cache.Read(new MockCacheable(), out outValue);

			Assert.False(result);
			Assert.IsNull(outValue);

		}

		[Test]
		public void TestDoubleAsKey()
		{
			double key = 2.99999999;
			int expected = 2;
			int nSets = 2;

			int actual = KeyHelper.GetBlockIdFromBits(key, nSets, new DoubleByteArrayConverter());

			Assert.AreEqual(expected, actual);
		}
	}
}