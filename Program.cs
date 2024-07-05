using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class CsvComparator
{
    public class CsvRecord
    {
        public List<string> Data { get; set; }

        public CsvRecord()
        {
            Data = new List<string>();
        }
    }

    public static List<string> CompareCsv(string file1, string file2)
    {
        var differences = new List<string>();

        var records1 = ReadCsv(file1);
        var records2 = ReadCsv(file2);

        int size = Math.Min(records1.Count, records2.Count);

        for (int i = 0; i < size; i++)
        {
            var record1 = records1[i];
            var record2 = records2[i];

            int columnCount = Math.Min(record1.Data.Count, record2.Data.Count);

            for (int j = 0; j < columnCount; j++)
            {
                var value1 = record1.Data[j];
                var value2 = record2.Data[j];

                if (!value1.Equals(value2))
                {
                    differences.Add($"Difference in row {i + 1}, column {j + 1}: file1='{value1}', file2='{value2}'");
                }
            }

            if (record1.Data.Count != record2.Data.Count)
            {
                differences.Add($"Row {i + 1} has different number of columns: file1={record1.Data.Count}, file2={record2.Data.Count}");
            }
        }

        if (records1.Count != records2.Count)
        {
            differences.Add($"Files have different number of rows: file1={records1.Count}, file2={records2.Count}");
        }

        return differences;
    }

    private static List<CsvRecord> ReadCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = new List<CsvRecord>();

        while (csv.Read())
        {
            var record = new CsvRecord();
            for (int i = 0; csv.TryGetField(i, out string value); i++)
            {
                record.Data.Add(value);
            }
            records.Add(record);
        }

        return records;
    }

    public static void Main(string[] args)
    {
        string file1 = @"C:\Users\Baku\Downloads\TESLA INC (07-10-2023 _ 07-03-2024).csv";
        string file2 = @"C:\Users\Baku\Downloads\AnotherFile.csv";

        Console.WriteLine($"File1 path: {file1}");
        Console.WriteLine($"File2 path: {file2}");

        try
        {
            var differences = CompareCsv(file1, file2);

            if (differences.Count == 0)
            {
                Console.WriteLine("No differences found.");
            }
            else
            {
                Console.WriteLine("Differences found:");
                foreach (var difference in differences)
                {
                    Console.WriteLine(difference);
                }

                string outputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Differences.txt");
                File.WriteAllLines(outputFile, differences);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
