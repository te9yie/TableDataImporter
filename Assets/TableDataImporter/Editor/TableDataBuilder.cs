using System;
using System.Collections.Generic;
using UnityEngine;

namespace TableDataImporter.Editor {
    internal class TableDataBuilder {
        private TableDataAst ast;

        public TableDataBuilder(TableDataAst ast) {
            this.ast = ast;
        }

        internal TableData Build() {
            var labels = new List<TableData.Label>();
            var tables = new List<Table>();
            foreach (var table in ast.Tables) {
                var label = new TableData.Label() {
                    Name = table.Name,
                    Offset = tables.Count,
                };
                var tableBuilder = new TableBuilder();
                foreach (var entry in table.Entries) {
                    var entryBuilder = new EntryBuilder();
                    for (int i = 0, n = table.Tags.Count; i < n; ++i) {
                        if (i >= entry.Values.Count) continue;
                        var tag = table.Tags[i];
                        var value = entry.Values[i];
                        switch (tag.Type.ToLower()) {
                        case "int":
                            entryBuilder.AddInt(tag.Name, int.Parse(value));
                            break;
                        case "float":
                            entryBuilder.AddFloat(tag.Name, float.Parse(value));
                            break;
                        case "string":
                            entryBuilder.AddString(tag.Name, value);
                            break;
                        case "bool":
                            entryBuilder.AddBool(tag.Name, bool.Parse(value));
                            break;
                        }
                    }
                    tableBuilder.AddEntry(entry.Name, entryBuilder.Build());
                }
                labels.Add(label);
                tables.Add(tableBuilder.Build());
            }
            labels.Sort();

            var asset = ScriptableObject.CreateInstance<TableData>();
            asset.Initialize(labels, tables);
            return asset;
        }
    }

    internal class TableBuilder {
        private Dictionary<string, List<Entry>> entries = new Dictionary<string, List<Entry>>();

        internal TableBuilder AddEntry(string name, Entry entry) {
            var entries = GetEntry(name);
            entries.Add(entry);
            return this;
        }

        internal Table Build() {
            List<Table.Label> ls = new List<Table.Label>();
            List<Entry> es = new List<Entry>();
            foreach (var item in entries) {
                ls.Add(new Table.Label {
                    Name = item.Key,
                    Offset = es.Count,
                    ArrayCount = item.Value.Count,
                });
                es.AddRange(item.Value);
            }
            ls.Sort();
            return new Table(ls, es);
        }

        private List<Entry> GetEntry(string name) {
            List<Entry> values;
            if (!entries.TryGetValue(name, out values)) {
                values = new List<Entry>();
                entries.Add(name, values);
            }
            return values;
        }
    }

    internal class EntryBuilder {
        private Dictionary<string, List<Entry.Value>> tags = new Dictionary<string, List<Entry.Value>>();
        private List<string> strings = new List<string>();

        internal EntryBuilder AddInt(string name, int i) {
            var values = GetValues(name);
            values.Add(new Entry.Value { Bytes = BitConverter.GetBytes(i) });
            return this;
        }

        internal EntryBuilder AddFloat(string name, float f) {
            var values = GetValues(name);
            values.Add(new Entry.Value { Bytes = BitConverter.GetBytes(f) });
            return this;
        }

        internal EntryBuilder AddBool(string name, bool b) {
            var values = GetValues(name);
            values.Add(new Entry.Value { Bytes = BitConverter.GetBytes(b) });
            return this;
        }

        internal EntryBuilder AddString(string name, string s) {
            var i = strings.Count;
            strings.Add(s);
            return AddInt(name, i);
        }

        internal Entry Build() {
            List<Entry.Tag> ts = new List<Entry.Tag>();
            List<Entry.Value> vs = new List<Entry.Value>();
            foreach (var item in tags) {
                ts.Add(new Entry.Tag {
                    Name = item.Key,
                    Offset = vs.Count,
                    ArrayCount = item.Value.Count,
                });
                vs.AddRange(item.Value);
            }
            ts.Sort();
            return new Entry(ts, vs, strings);
        }

        private List<Entry.Value> GetValues(string name) {
            List<Entry.Value> values;
            if (!tags.TryGetValue(name, out values)) {
                values = new List<Entry.Value>();
                tags.Add(name, values);
            }
            return values;
        }
    }
}