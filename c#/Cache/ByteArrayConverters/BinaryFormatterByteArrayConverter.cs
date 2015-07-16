using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cache.ByteArrayConverters
{
	/// <summary>
	/// This was the widely-accepted way to serialize any object into binary form on the web but I found it wasn't exactly
	/// what I needed, so check out the other implementations that tend to produce more expected results.
	/// http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
	/// If this was C I would just copy the memory at the object's real address over into a new byte array but I've never found
	/// how to do that in C#.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class BinaryFormatterByteArrayConverter<TKey> : IByteArrayConverter<TKey>
	{
		public byte[] ToByteArray(TKey obj)
		{
			if (obj == null)
				return null;
			var bf = new BinaryFormatter();
			var ms = new MemoryStream();
			bf.Serialize(ms, obj);

			return ms.ToArray();
		}
	}
}