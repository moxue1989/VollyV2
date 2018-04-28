using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace VollyV2.Services
{
    public class CsvImporter<T>
    {
        public IEnumerable<T> Import(StreamReader reader)
        {
            CsvReader csvReader = GetCsvReader(reader);
            return csvReader.GetRecords<T>();
        }

        private static CsvReader GetCsvReader(StreamReader reader)
        {
            CsvReader csvReader = new CsvReader(reader);
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.HeaderValidated = null;
            csvReader.Configuration.BadDataFound = null;
            return csvReader;
        }
    }
}
