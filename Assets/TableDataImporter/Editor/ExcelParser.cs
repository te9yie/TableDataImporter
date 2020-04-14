using System.IO;
using System.Linq;
using NPOI.SS.UserModel;

namespace TableDataImporter.Editor {
    internal class ExcelParser : ITableDataParser {
        private string path;

        public ExcelParser(string path) {
            this.path = path;
        }

        public TableDataAst Parse() {
            
            using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var book = WorkbookFactory.Create(input);
                if (book == null) return null;
                var repo = new TableDataAst();
                for (int i = 0, i_n = book.NumberOfSheets; i < i_n; ++i) {
                    var sheet = book.GetSheetAt(i);
                    if (sheet == null) continue;
                    TableAst table = null;
                    int tableIndent = -1;
                    for (int j = sheet.FirstRowNum, j_n = sheet.LastRowNum; j <= j_n; ++j) {
                        var row = sheet.GetRow(j);
                        if (row == null) continue;
                        RowType rowType = RowType.None;
                        EntryAst entry = null;
                        for (int k = row.FirstCellNum, k_n = row.LastCellNum; k < k_n; ++k) {
                            var cell = row.GetCell(k);
                            if (cell == null) continue;
                            var str = GetCellString(cell, cell.CellType);
                            if (!string.IsNullOrEmpty(str)) {
                                if (str.StartsWith("[") && str.EndsWith("]")) {
                                    tableIndent = k;
                                    table = repo.AddTable(str.Trim('[', ']'));
                                    continue;
                                }
                            }
                            if (table == null) continue;
                            if (k < tableIndent) continue;
                            var index = k - tableIndent;
                            if (index == 0) {
                                if (!string.IsNullOrEmpty(str)) {
                                    if (str.StartsWith("#")) continue;
                                    switch (str.ToLower()) {
                                    case "<tag>":
                                        rowType = RowType.Tag;
                                        break;
                                    case "<type>":
                                        rowType = RowType.Type;
                                        break;
                                    default:
                                        entry = table.AddEntry(str);
                                        rowType = RowType.Entry;
                                        break;
                                    }
                                }
                                else {
                                    entry = table.DupLastEntry();
                                    rowType = RowType.Entry;
                                }
                                continue;
                            }
                            index -= 1;
                            switch (rowType) {
                            case RowType.Tag:
                                table.AddTagName(index, str);
                                break;
                            case RowType.Type:
                                table.AddTagType(index, str);
                                break;
                            case RowType.Entry:
                                if (entry == null) break;
                                entry.AddValue(index, str);
                                break;
                            }
                        }
                    }
                }
                return repo;
            }
        }
        
        enum RowType {
            None,
            Tag,
            Type,
            Entry,
        };

        private string GetCellString(ICell cell, CellType cellType) {
            switch (cellType) {
            case CellType.Boolean: return cell.BooleanCellValue.ToString();
            case CellType.Formula: return GetCellString(cell, cell.CachedFormulaResultType);
            case CellType.Numeric: return cell.NumericCellValue.ToString();
            case CellType.String: return cell.StringCellValue;
            default: break;
            }
            return null;
        }

        internal static bool CanParse(string path) {
            string[] EXTENSIONS = { ".xlsx", ".xls" };
            var ext = Path.GetExtension(path);
            return EXTENSIONS.Contains(ext);
        }
    }
}
