﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Tsdb.Ats
{
   public class YearlyPartitioningProvider<TKey> : IPartitionProvider<TKey>
   {
      private static readonly string MinPartitionKeyRange = "9999";
      private static readonly string MaxPartitionKeyRange = "0000";

      public string GetMaxPartitioning( TKey id )
      {
         return MaxPartitionKeyRange;
      }

      public string GetMinPartitioning( TKey id )
      {
         return MinPartitionKeyRange;
      }

      public string GetPartitioning( TKey id, DateTime timestamp )
      {
         return CalculatePartitionKeyRange( timestamp );
      }

      private static string CalculatePartitionKeyRange( DateTime timestamp )
      {
         return CalculatePartitionKeyRange( timestamp.Year );
      }

      private static string CalculatePartitionKeyRange( int year )
      {
         var inverseYear = 9999 - year;
         if( inverseYear < 1000 )
         {
            return inverseYear.ToString( "0000" );
         }
         return inverseYear.ToString();
      }
   }
}
