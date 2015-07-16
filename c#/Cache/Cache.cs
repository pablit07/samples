using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cache.ByteArrayConverters;
using Cache.ReplacementPolicies;

namespace Cache
{
	using CacheMemoryIndex = Int32;

	/// <summary>
	///     Cache is an n-way set associative in-memory cache with strongly-typed keys and values.
	///		It does not write through; that is, the client is responsible for keeping the cache up-to-date.
	///     Its available storage memory is divided into sets to enable more storage.
	///     Each set is indexed using an algorithm to provide quicker reading and writing.
	///     The algorithm used for mapping a key to an index is to take the n least significant bits of the key as the index.
	///     http://www.pitt.edu/~juy9/142/slides/L11-Cache2.pdf
	/// </summary>
	/// <typeparam name="TKey">The type of the keys per instance</typeparam>
	/// <typeparam name="TValue">The type of the values per instance</typeparam>
	public class Cache<TKey, TValue>
	{
		/// <summary>
		///     memory contains all items written to cache
		/// </summary>
		private readonly Memory memory;

		/// <summary>
		///     options holds the client-defined options for a particular cache instance
		/// </summary>
		private readonly Options options;


		/// <summary>
		///     ByteArrayConverter tells the cache how to convert a key to its binary form in order to read its least-significant
		///     bits. These bits then become the indexes that are mapped to cache memory.
		/// </summary>
		public IByteArrayConverter<TKey> ByteArrayConverter = new BinaryFormatterByteArrayConverter<TKey>();

		/// <summary>
		///     ReplacementPolicy tells the cache what strategy to use when choosing a block for eviction, when there are no
		///     available cache blocks for
		///     a write.
		/// </summary>
		public IReplacementPolicy<TKey, TValue> ReplacementPolicy = new MruReplacementPolicy<TKey, TValue>();


		public Cache(Options options)
		{
			this.options = options;
			memory = new Memory(options);
		}


		/// <summary>
		///     Read attempts to find the cache block with the given key and make its value available to the caller.
		/// </summary>
		/// <param name="key">the key to look for</param>
		/// <param name="value">out param that will contain the value if found, or null or default(value types)</param>
		/// <returns>true if key match was found, false otherwise</returns>
		public bool Read(TKey key, out TValue value)
		{
			return memory.TryGetValue(key, KeyHelper.GetBlockIdFromBits(key, options.Wayness, ByteArrayConverter), out value);
		}

		/// <summary>
		///     Write will allocate a cache block for the provided key and value.
		///     If the key is found to already live inside the cache, the value is updated.
		///     If no suitable cache blocks are free, the ReplacementPolicy is utilized to free a cache block, and that block is
		///     then
		///     written to.
		/// </summary>
		/// <param name="key">key of the value to write</param>
		/// <param name="value">the value to write into the cache</param>
		public void Write(TKey key, TValue value)
		{
			var cacheKey = key;
			int? collisionSet;
			var index = KeyHelper.GetBlockIdFromBits(cacheKey, options.Wayness, ByteArrayConverter);
			var success = memory.TryPutValue(key, value, index, out collisionSet);

			// collision replacement
			if (!success && collisionSet.HasValue)
			{
				var existing = memory.GetFromSet(collisionSet.Value, index);
				existing.Update(value);
			}
			// eviction replacement
			else if (!success)
			{
				var evictedSet = memory.Evict(index, ReplacementPolicy);
				memory.PutInSet(key, value, evictedSet, index);
			}
		}


		/// <summary>
		///     Memory is responsible for directly reading and writing the cache's memory
		/// </summary>
		private class Memory
		{
			/// <summary>
			///     blocks is the underlying physical implementation of the cache's memory
			/// </summary>
			private readonly Block<TKey, TValue>[,] blocks;

			/// <summary>
			///     options is a copy of the cache's client-defined options
			/// </summary>
			private readonly Options options;

			private int _lastPosition = 0;


			public Memory(Options options)
			{
				this.options = options;
				blocks = new Block<TKey, TValue>[options.NumberOfSets, (int) Math.Pow(2, options.Wayness)];
			}


			/// <summary>
			///     _CheckInSet looks in a particular set and returns if the key is found in the specified index
			/// </summary>
			/// <param name="key">true if this key is found at given index</param>
			/// <param name="set">the set id to look in</param>
			/// <param name="index">index to check within set</param>
			/// <returns>true if key matches, false if no match or empty index</returns>
			private bool _CheckInSet(TKey key, int set, CacheMemoryIndex index)
			{
				return !_IsEmpty(set, index) && GetFromSet(set, index).Key.Equals(key);
			}

			/// <summary>
			///     _IsEmpty looks in a particular set and returns if the specified index is empty(null)
			/// </summary>
			/// <param name="set">the set id to look in</param>
			/// <param name="index">index to check within set</param>
			/// <returns>true if null found, false otherwise</returns>
			private bool _IsEmpty(int set, CacheMemoryIndex index)
			{
				return GetFromSet(set, index) == null;
			}


			/// <summary>
			///     PutInSet writes a new cache block into memory with specified key at the given set and index
			/// </summary>
			/// <param name="key">the key for the new block</param>
			/// <param name="value">the value of the new block</param>
			/// <param name="set">which set to write to</param>
			/// <param name="index">which index to write to</param>
			public void PutInSet(TKey key, TValue value, int set, CacheMemoryIndex index)
			{
				blocks[set, index] = new Block<TKey, TValue>(key, value, _lastPosition);
			}

			/// <summary>
			///     GetFromSet reads a cache block from a particular set in memory at the specified index
			/// </summary>
			/// <param name="set">the set to read from</param>
			/// <param name="index">index to read within set</param>
			/// <returns>the block at given set and index, or null or default(value types)</returns>
			public Block<TKey, TValue> GetFromSet(int set, CacheMemoryIndex index)
			{
				return blocks[set, index];
			}

			/// <summary>
			///     TryGetValue searches all sets, uses _CheckInSet to see if a key is found at given index within a specified set,
			///     and if found, returns its associated value.
			/// </summary>
			/// <param name="key">the key to look for</param>
			/// <param name="index">the index to look within all sets</param>
			/// <param name="value">out param containing the value if found, or null or default(value types)</param>
			/// <returns></returns>
			public bool TryGetValue(TKey key, CacheMemoryIndex index, out TValue value)
			{
				for (var set = 0; set < options.NumberOfSets; set++)
				{
					if (_CheckInSet(key, set, index))
					{
						value = GetFromSet(set, index).GetValue(_lastPosition++);	// NOTE - imperfect solution, will eventually run out of positions
						return true;
					}
				}
				value = default(TValue);
				return false;
			}

			/// <summary>
			///     TryPutValue searches all sets for an empty block at specified index, and either puts a new block with key and value
			///     and returns true, or returns false.
			///     Also searches for exact key collisions using collisionSet param.
			/// </summary>
			/// <param name="key">key of the new value to write</param>
			/// <param name="value">value to write</param>
			/// <param name="index">index to look within all sets</param>
			/// <param name="collisionSet">out param containing the set id of an exact key match collision, if one was found, or null</param>
			/// <returns></returns>
			public bool TryPutValue(TKey key, TValue value, CacheMemoryIndex index, out int? collisionSet)
			{
				collisionSet = null;
				for (var set = 0; set < options.NumberOfSets; set++)
				{
					if (_IsEmpty(set, index))
					{
						PutInSet(key, value, set, index);
						return true;
					}
					// collision detection
					if (_CheckInSet(key, set, index))
					{
						collisionSet = set;
						return false;
					}
				}
				return false;
			}

			/// <summary>
			///     Evict passes a map of the blocks at a given index to the replacement policy in order to find a block to remove.
			///     It then removes the block and returns the set so the caller can replace it.
			/// </summary>
			/// <param name="index">index across all sets to consider</param>
			/// <param name="replacementPolicy">instance of a policy that specifies the logic for choosing an evicted block</param>
			/// <returns>the set id which contains the block chosen for eviction</returns>
			public int Evict(CacheMemoryIndex index, IReplacementPolicy<TKey, TValue> replacementPolicy)
			{
				var _blocks = new Dictionary<int, Block<TKey, TValue>>();

				for (var i = 0; i < options.NumberOfSets; i++)
				{
					_blocks.Add(i, GetFromSet(i, index));
				}
				var setChoice = replacementPolicy.ChooseSet(_blocks);

				blocks[setChoice, index] = null;

				return setChoice;
			}
		}
	}
}