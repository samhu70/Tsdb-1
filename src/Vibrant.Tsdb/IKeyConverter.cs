﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Tsdb
{
   public interface IKeyConverter<TKey>
   {
      TKey Convert( string key );

      string Convert( TKey key );
   }
}
