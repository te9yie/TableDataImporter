using System.Collections.Generic;
using System.Linq;

namespace TableDataImporter.Editor {
    internal class TableDataAst {
        internal List<TableAst> Tables = new List<TableAst>();

        internal TableAst AddTable(string name) {
            var t = new TableAst(name);
            Tables.Add(t);
            return t;
        }
    }

    internal class TableAst {
        internal class TagAst {
            internal string Name;
            internal string Type;
        }

        internal string Name;
        internal List<TagAst> Tags = new List<TagAst>();
        internal List<EntryAst> Entries = new List<EntryAst>();

        internal TableAst(string name) {
            Name = name;
        }

        internal void AddTagName(int index, string str) {
            var tag = GetOrCreateTag(index);
            tag.Name = str;
        }

        internal void AddTagType(int index, string str) {
            var tag = GetOrCreateTag(index);
            tag.Type = str;
        }

        internal EntryAst AddEntry(string name) {
            var e = new EntryAst(name);
            Entries.Add(e);
            return e;
        }

        internal EntryAst DupLastEntry() {
            if (Entries.Count == 0) return null;
            var e = new EntryAst(Entries.Last().Name);
            Entries.Add(e);
            return e;
        }

        private TagAst GetOrCreateTag(int index) {
            if (index >= Tags.Count) {
                Tags.AddRange(new TagAst[index - Tags.Count + 1]);
            }
            var tag = Tags[index];
            if (tag == null) {
                tag = Tags[index] = new TagAst();
            }
            return tag;
        }
    }

    internal class EntryAst {
        internal string Name;
        internal List<string> Values = new List<string>();

        internal EntryAst(string name) {
            Name = name;
        }

        internal void AddValue(int index, string str) {
            if (index >= Values.Count) {
                Values.AddRange(new string[index - Values.Count + 1]);
            }
            Values[index] = str;
        }
    }
}