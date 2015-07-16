namespace Cache
{
	/// <summary>
	///     Options is a container for passing required options to a new Cache instance.
	/// </summary>
	public class Options
	{
		/// <summary>
		///     Creates an Options instance.
		/// </summary>
		/// <param name="numberOfSets">numberOfSets tells the cache how many sets of blocks to keep</param>
		/// <param name="wayness">
		///     Wayness (actually log2(n) of the true wayness) tells the cache how many blocks to allocate in each set. Because of the
		///     associativity of each block's index, this option actually tells the cache to create 2^k number of blocks.
		///     Pass 0 to create a directly-mapped cache.
		/// </param>
		public Options(int numberOfSets, int wayness)
		{
			NumberOfSets = numberOfSets;
			Wayness = wayness;
		}

		public int NumberOfSets { get; private set; }
		public int Wayness { get; private set; }
	}
}