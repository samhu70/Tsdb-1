﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Tsdb
{
   public class MultiReadResult<TEntry> : IEnumerable<ReadResult<TEntry>>
      where TEntry : IEntry
   {
      private IDictionary<string, ReadResult<TEntry>> _results;

      public MultiReadResult( IDictionary<string, ReadResult<TEntry>> results )
      {
         _results = results;
      }

      public MultiReadResult( IEnumerable<ReadResult<TEntry>> results )
      {
         _results = results.ToDictionary( x => x.Id );
      }

      public MultiReadResult()
      {
         _results = new Dictionary<string, ReadResult<TEntry>>();
      }

      public ReadResult<TEntry> FindResult( string id )
      {
         return _results[ id ];
      }

      public bool TryFindResult( string id, out ReadResult<TEntry> readResult )
      {
         return _results.TryGetValue( id, out readResult );
      }

      public void AddOrMerge( MultiReadResult<TEntry> result )
      {
         foreach( var item in result )
         {
            AddOrMerge( item );
         }
      }

      public void AddOrMerge( ReadResult<TEntry> result )
      {
         ReadResult<TEntry> existing;
         if( _results.TryGetValue( result.Id, out existing ) )
         {
            existing.MergeWith( result );
         }
         else
         {
            _results.Add( result.Id, result );
         }
      }

      public IEnumerator<ReadResult<TEntry>> GetEnumerator()
      {
         return _results.Values.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return _results.Values.GetEnumerator();
      }
      public MultiReadResult<TOutputEntry> As<TOutputEntry>()
         where TOutputEntry : IEntry
      {
         return new MultiReadResult<TOutputEntry>( _results.ToDictionary( x => x.Key, x => x.Value.As<TOutputEntry>() ) );
      }

      public MultiReadResult<TEntry> MergeWith( MultiReadResult<TEntry> other )
      {
         foreach( var thisResult in this )
         {
            var otherResult = other.FindResult( thisResult.Id );
            thisResult.MergeWith( otherResult );
         }
         return this;
      }

      public MultiReadResult<TEntry> MergeInto( MultiReadResult<TEntry> other )
      {
         foreach( var otherResult in other )
         {
            var thisResult = FindResult( otherResult.Id );
            otherResult.MergeWith( thisResult );
         }
         return this;
      }
   }
}
