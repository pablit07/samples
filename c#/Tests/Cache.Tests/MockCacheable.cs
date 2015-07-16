using System;

namespace Cache.Tests
{
	public class MockCacheable
	{
		private string _key;
		private byte[] _keyInBytes;

		public string GetCacheKey()
		{
			return Key;
		}

		public string Key
		{
			get
			{
				if (_keyInBytes != null)
				{
					return _ToStringKey(_keyInBytes);
				}
				return _key;
			}
			set { _key = value; }
		}

		public int SomeOtherProp { get; set; }

		private static string _ToStringKey(byte[] keyInBytes)
		{
			var i = keyInBytes.Length / sizeof(char);
			var charArray = new char[i];
			Buffer.BlockCopy(keyInBytes, 0, charArray, 0, keyInBytes.Length);

			return new string(charArray).Trim('\0');
		}

		public MockCacheable SetKeyInBytes(int bytes)
		{
			_keyInBytes = BitConverter.GetBytes(bytes);
			return this;
		}
	}
}