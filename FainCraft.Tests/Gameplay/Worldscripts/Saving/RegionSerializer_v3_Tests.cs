using static FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.RegionFileWriter.RegionSerializer_v3.ChunkTable;
using static FainCraft.Gameplay.WorldScripts.Systems.Loading.Files.RegionFileWriter.RegionSerializer_v3;
using FainCraft.Gameplay.WorldScripts.Systems.Loading.Files;

namespace FainCraft.Tests.Gameplay.Worldscripts.Saving
{
    public class RegionSerializer_v3_Tests
    {
        private ChunkTable _table;
        private long _minData => _table.Location + ChunkTable.Size;

        public RegionSerializer_v3_Tests()
        {
            // Arbitrary base location for the _table in the file
            _table = new ChunkTable { Location = 1000, Stream = null! };
        }

        [Fact]
        public void Throws_When_Row_Not_Found()
        {
            var rows = new List<TableRow>();
            var newRow = new TableRow { SaveCoord = new SaveCoordOffset(1, 1, 1) };

            Assert.Throws<InvalidDataException>(() =>
                _table.UpdateOrAddRow(rows, newRow)
            );
        }

        [Fact]
        public void Reuses_Row_When_Valid_Existing()
        {
            var coord = new SaveCoordOffset(2, 2, 2);
            var rows = new List<TableRow>
            {
                new TableRow { 
                    SaveCoord = coord, 
                    Encoding  = 0, 
                    Location  = _minData + 200, 
                    Length    = 100 
                }
            };

            var newRow = new TableRow { 
                SaveCoord = coord, 
                Encoding = 1, 
                Length = 50 
            };

            long result = _table.UpdateOrAddRow(rows, newRow);

            Assert.Equal(_minData + 200, result);
        }

        [Fact]
        public void Places_At_MinData_When_No_Existing_Valid()
        {
            var coord = new SaveCoordOffset(3, 3, 3);
            var rows = new List<TableRow>
            {
                new TableRow { SaveCoord = coord, Encoding = -1, Location = 0, Length = 0 }
            };
            var newRow = new TableRow { SaveCoord = coord, Encoding = 1, Length = 20 };

            long result = _table.UpdateOrAddRow(rows, newRow);

            Assert.Equal(_minData, result);
            Assert.Equal(_minData, rows[0].Location);
        }

        [Fact]
        public void Finds_Gap_Before_First_Valid_Row()
        {
            var coord      = new SaveCoordOffset(4, 4, 4);
            var validCoord = new SaveCoordOffset(5, 5, 5);

            var rows = new List<TableRow>
            {
                new TableRow { 
                    SaveCoord = coord, 
                    Encoding  = -1, 
                    Location  = 0, 
                    Length    = 0 
                },

                // First valid data row starts after minData + 100
                new TableRow { 
                    SaveCoord = validCoord, 
                    Encoding  = 1,
                    Location  = _minData + 100,
                    Length    = 50 
                }
            };

            var newRow = new TableRow { SaveCoord = coord, Encoding = 1, Length = 50 };

            long result = _table.UpdateOrAddRow(rows, newRow);

            // Should fill at minData, since gap before first valid
            Assert.Equal(_minData, result);
        }

        [Fact]
        public void Finds_Gap_Between_Valid_Rows()
        {
            var coord = new SaveCoordOffset(6, 6, 6);
            var valid1 = new SaveCoordOffset(7, 7, 7);
            var valid2 = new SaveCoordOffset(8, 8, 8);
            long start1 = _table.Location + ChunkTable.Size + 50;
            int len1 = 50;
            long start2 = _table.Location + ChunkTable.Size + 200;
            int len2 = 75;
            var rows = new List<TableRow>
            {
                new TableRow { SaveCoord = coord, Encoding = -1, Location = 0, Length = 0 },
                new TableRow { SaveCoord = valid1, Encoding = 1, Location = start1, Length = len1 },
                new TableRow { SaveCoord = valid2, Encoding = 1, Location = start2, Length = len2 }
            };
            var newRow = new TableRow { SaveCoord = coord, Encoding = 1, Length = 100 };

            long expectedGapStart = start1 + len1; // after first valid
            // gap between (start1+len1)=100 and start2=200 is 100, fits exactly
            long result = _table.UpdateOrAddRow(rows, newRow);

            Assert.Equal(expectedGapStart, result);
        }

        [Fact]
        public void Places_After_Last_When_No_Sufficient_Gap()
        {
            var coord = new SaveCoordOffset( 9,  9,  9);
            var valid = new SaveCoordOffset(10, 10, 10);
            long start = _table.Location + ChunkTable.Size;

            var rows = new List<TableRow>
            {
                new TableRow { SaveCoord = coord, Encoding = 1, Location = start,     Length = 100 },
                new TableRow { SaveCoord = valid, Encoding = 1, Location = start+100, Length = 200 }
            };
            var newRow = new TableRow { SaveCoord = coord, Encoding = 1, Length = 200 };

            long endOfLast = start + 300;
            long result = _table.UpdateOrAddRow(rows, newRow);

            Assert.Equal(start+300, result);
        }

        [Fact]
        public void NewRowLocation_Never_Before_MinData()
        {
            var coord = new SaveCoordOffset(11, 11, 11);
            var rows = new List<TableRow>
            {
                new TableRow { SaveCoord = coord, Encoding = -1, Location = 0, Length = 0 }
            };
            var newRow = new TableRow { SaveCoord = coord, Encoding = 1, Length = 10 };

            long minData = _table.Location + ChunkTable.Size;
            long result = _table.UpdateOrAddRow(rows, newRow);

            Assert.True(result >= minData, "New row location should never be before the data region.");
        }
    }
}