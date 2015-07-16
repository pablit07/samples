using System;

namespace Cache.ByteArrayConverters
{
	public class IntByteArrayConverter : IByteArrayConverter<int>
	{
		public byte[] ToByteArray(int numeric)
		{
			return BitConverter.GetBytes(numeric);
		}
	}
}