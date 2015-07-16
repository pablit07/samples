using System;

namespace Cache
{
	/// <summary>
	///     Block holds information about a single block of cache memory
	/// </summary>
	/// <typeparam name="TKey">The type of the block key</typeparam>
	/// <typeparam name="TValue">The type of the block value</typeparam>
	public class Block<TKey, TValue>
	{
		private TValue _value;

		public Block(TKey key, TValue value, int position)
		{
			Key = key;
			Value = value;
			LastRead = LastModified = Created = DateTime.Now;
			UseCount = 0;
			Position = position;
		}


		/// <summary>
		///     When the block was first written
		/// </summary>
		public DateTime Created { get; private set; }

		/// <summary>
		///     Block key
		/// </summary>
		public TKey Key { get; set; }

		/// <summary>
		///     The last time the value changed
		/// </summary>
		public DateTime LastModified { get; private set; }

		/// <summary>
		///     When last the block was read
		/// </summary>
		public DateTime LastRead { get; set; }

		/// <summary>
		///     The number of times this block has been read
		/// </summary>
		public int UseCount { get; set; }

		/// <summary>
		/// Tracks the last read position relative to other blocks
		/// </summary>
		public decimal Position { get; set; }

		/// <summary>
		///     Block value
		/// </summary>
		public TValue Value
		{
			set { _value = value; }
		}

		/// <summary>
		/// GetValue returns the block's Value field and lets the caller pass in a read position that is used to track
		/// its position across the entire set of reads per cache instance
		/// Note - the position will eventually run out of memory so this is an imperfect solution, but
		/// for automated testing the reads occur so fast that the LastRead timestamp was becoming unreliable
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public TValue GetValue(int position)
		{
			LastRead = DateTime.Now;
			UseCount++;
			Position = position;
			return _value;
		}

		/// <summary>
		///     Update sets the blocks value to the one passed in. The key remains the same.
		/// </summary>
		/// <param name="value">value to set</param>
		public void Update(TValue value)
		{
			Value = value;
			LastModified = DateTime.Now;
		}
	}
}