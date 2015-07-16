using System.Collections.Generic;
using System.Linq;

namespace Cache
{
	/// <summary>
	/// LRU replacement policy
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class LruReplacementPolicy<TKey, TValue> : IReplacementPolicy<TKey, TValue>
	{
		public int ChooseSet(Dictionary<int, Block<TKey, TValue>> blocks)
		{
			return blocks.OrderBy(pair => pair.Value.LastRead).First().Key;
		}
	}
}