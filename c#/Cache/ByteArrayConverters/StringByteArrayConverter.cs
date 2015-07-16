using System;

namespace Cache.ByteArrayConverters
{
	public class StringByteArrayConverter : IByteArrayConverter<string>
	{
		public byte[] ToByteArray(string str)
		{
			var bytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}
}