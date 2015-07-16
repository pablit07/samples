using System.Collections.Generic;

namespace Cache
{
	public interface IReplacementPolicy<TKey, TValue>
	{
		int ChooseSet(Dictionary<int, Block<TKey, TValue>> blocks);
	}
}