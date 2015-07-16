using System.Collections.Generic;
using System.Linq;

namespace Cache.ReplacementPolicies
{
	/// <summary>
	/// MRU replacement policy that relies on a global Position field, better for automated testing since the
	/// tests seem to run too quickly to rely on a timestamp
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class MruPositionReplacementPolicy <TKey, TValue> : IReplacementPolicy<TKey, TValue>
	{
		public int ChooseSet(Dictionary<int, Block<TKey, TValue>> blocks)
		{
			return blocks.OrderByDescending(pair => pair.Value.Position).First().Key;
		}
	}
}