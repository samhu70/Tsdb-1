﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Tsdb
{
   public interface IVolumeStorageSelector<TKey, TEntry> where TEntry : IEntry
   {
      IEnumerable<StorageSelection<TKey, TEntry, IVolumeStorage<TKey, TEntry>>> GetStorage( TKey id, DateTime? from, DateTime? to );

      IVolumeStorage<TKey, TEntry> GetStorage( TKey key, TEntry entry );
   }
}
