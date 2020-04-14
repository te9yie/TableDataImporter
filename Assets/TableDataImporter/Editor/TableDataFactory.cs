namespace TableDataImporter.Editor {
    internal class TableDataFactory {
        internal static ITableDataParser CreateParser(string path) {
            //if (CsvParser.CanParse(path)) return new CsvParser(path);
            if (ExcelParser.CanParse(path)) return new ExcelParser(path);
            return null;
        }

        internal static bool CanParse(string path) {
            //if (CsvParser.CanParse(path)) return true;
            if (ExcelParser.CanParse(path)) return true;
            return false;
        }
    }
}