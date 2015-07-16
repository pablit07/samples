using System;

namespace Cache.ByteArrayConverters
{
	public class DoubleByteArrayConverter : IByteArrayConverter<double>
	{
		public byte[] ToByteArray(double numeric)
		{
			return BitConverter.GetBytes(numeric);
		}
	}
}