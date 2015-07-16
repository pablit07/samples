using System.Collections.Generic;
using System.Linq;

namespace Cache.ReplacementPolicies
{
	/// <summary>
	/// MRU replacement policy
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class MruReplacementPolicy <TKey, TValue> : IReplacementPolicy<TKey, TValue>
	{
		public int ChooseSet(Dictionary<int, Block<TKey, TValue>> blocks)
		{
			return blocks.OrderByDescending(pair => pair.Value.LastRead).First().Key;
		}
	}
}