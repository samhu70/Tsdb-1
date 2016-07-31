﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Vibrant.Tsdb.Ats.Tests.Entries;
using Xunit;

namespace Vibrant.Tsdb.Ats.Tests
{
   public abstract class AbstractStorageTests<TStorage>
      where TStorage : IStorage
   {

      private static readonly string[] Ids = new[]
      {
         "row1",
         "row2",
         "row3",
         "row4",
         "row5",
         "row6",
         "row7",
      };

      private static readonly Random _rng = new Random();

      private static List<BasicEntry> CreateRows( DateTime startTime, int count )
      {
         var entries = new List<BasicEntry>();

         DateTime current = startTime;
         for( int i = 0 ; i < count ; i++ )
         {
            var entry = new BasicEntry();
            entry.Id = Ids[ _rng.Next( Ids.Length ) ];
            entry.Timestamp = current;
            entry.Value = _rng.NextDouble();

            current = current.AddSeconds( 1 );

            entries.Add( entry );
         }

         return entries;
      }

      public abstract IStorage GetStorage( string tableName );

      [Fact]
      public async Task Should_Write_And_Read_Basic_Rows()
      {
         TsdbTypeRegistry.Register<BasicEntry>();

         var store = GetStorage( "Table1");

         int count = 500000;

         var from = new DateTime( 2016, 12, 26, 0, 0, 0, DateTimeKind.Utc );
         var to = from.AddSeconds( count );

         var written = CreateRows( from, count );
         await store.Write( written );
         var read = await store.ReadAs<BasicEntry>( Ids, from, to );
         var latest = await store.ReadLatestAs<BasicEntry>( Ids );

         Dictionary<string, List<BasicEntry>> entries = new Dictionary<string, List<BasicEntry>>();
         foreach( var item in written )
         {
            List<BasicEntry> items;
            if( !entries.TryGetValue( item.GetId(), out items ) )
            {
               items = new List<BasicEntry>();
               entries[ item.GetId() ] = items;
            }
            items.Add( item );
         }

         var deletedCount = await store.Delete( Ids, from, to );

         foreach( var readResult in read )
         {
            var sourceList = entries[ readResult.Id ].Reverse<BasicEntry>().ToList();

            Assert.Equal( sourceList.Count, readResult.Entries.Count );
            for( int i = 0 ; i < sourceList.Count ; i++ )
            {
               var original = sourceList[ i ];
               var readen = readResult.Entries[ i ];

               Assert.Equal( original.Value, readen.Value );
               Assert.Equal( original.Timestamp, readen.Timestamp );

               if( i == 0 )
               {
                  var latestEntry = latest.First( x => x.Id == readResult.Id );
                  var entry = latestEntry.Entries[ 0 ];

                  Assert.Equal( original.Value, entry.Value );
                  Assert.Equal( original.Timestamp, entry.Timestamp );
               }
            }
         }
      }

      [Fact]
      public async Task Should_Write_And_Delete_Basic_Rows()
      {
         TsdbTypeRegistry.Register<BasicEntry>();

         var store = GetStorage( "Table2" );

         int count = 500000;

         var from = new DateTime( 2016, 12, 26, 0, 0, 0, DateTimeKind.Utc );
         var to = from.AddSeconds( count );

         var written = CreateRows( from, count );

         await store.Write( written );

         var deletedCount = await store.Delete( Ids, from, to );

         Assert.Equal( count, deletedCount );

         var read = await store.Read( Ids, from, to );

         Assert.Equal( 0, read.Sum( x => x.Entries.Count ) );
      }

      [Fact]
      public async Task Should_Write_Twice_Then_Delete_All_Basic_Rows()
      {
         TsdbTypeRegistry.Register<BasicEntry>();

         var store = GetStorage( "Table3" );

         int count = 1000;

         var from1 = new DateTime( 2016, 12, 26, 0, 0, 0, DateTimeKind.Utc );
         var written1 = CreateRows( from1, count );
         await store.Write( written1 );

         var from2 = new DateTime( 2017, 12, 26, 0, 0, 0, DateTimeKind.Utc );
         var written2 = CreateRows( from2, count );
         await store.Write( written2 );


         var rows = await store.Read( Ids );

         Assert.Equal( count * 2, rows.Sum( x => x.Entries.Count ) );

         var deletedCount = await store.Delete( Ids );

         Assert.Equal( count * 2, deletedCount );

         var read = await store.Read( Ids );

         Assert.Equal( 0, read.Sum( x => x.Entries.Count ) );
      }
   }
}