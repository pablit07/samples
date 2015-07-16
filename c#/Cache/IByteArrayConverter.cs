namespace Cache
{
	public interface IByteArrayConverter<in T>
	{
		byte[] ToByteArray(T obj);
	}
}