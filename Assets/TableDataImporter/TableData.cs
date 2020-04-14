using System;
using System.Collections.Generic;
using UnityEngine;

namespace TableDataImporter {
    [Serializable]
    public class TableData : ScriptableObject {
        [Serializable]
        public class Label : IComparable<Label> {
            public string Name;
            public int Offset;

            public int CompareTo(Label other) => Name.CompareTo(other.Name);
        }

        [SerializeField] private List<Label> labels;
        [SerializeField] private List<Table> tables;

        public void Initialize(List<Label> labels, List<Table> tables) {
            this.labels = labels;
            this.tables = tables;
        }
        
        public Table GetTable(string name) {
            Label key = new Label {
                Name = name
            };
            var labelIndex = labels.BinarySearch(key);
            if (labelIndex < 0) return null;
            var label = labels[labelIndex];
            return tables[label.Offset];
        }
    }

    [Serializable]
    public class Table {
        [Serializable]
        public class Label : IComparable<Label> {
            public string Name;
            public int Offset;
            public int ArrayCount;

            public int CompareTo(Label other) => Name.CompareTo(other.Name);
        }

        [SerializeField] private List<Label> labels;
        [SerializeField] private List<Entry> entries;

        public Table(List<Label> labels, List<Entry> entries) {
            this.labels = labels;
            this.entries = entries;
        }

        public int? GetCount(string name) {
            var label = GetLabel(name);
            if (label == null) return null;
            return label.ArrayCount;
        }

        public Entry GetEntry(string name) => GetEntry(name, 0);

        public Entry GetEntry(string name, int i) {
            var label = GetLabel(name);
            if (label == null) return null;
            if (i >= label.ArrayCount) return null;
            return entries[label.Offset + i];
        }

        private Label GetLabel(string name) {
            Label key = new Label {
                Name = name
            };
            var labelIndex = labels.BinarySearch(key);
            if (labelIndex < 0) return null;
            return labels[labelIndex];
        }
    }

    [Serializable]
    public class Entry {
        [Serializable]
        public class Tag : IComparable<Tag> {
            public string Name;
            public int Offset;
            public int ArrayCount;

            public int CompareTo(Tag other) => Name.CompareTo(other.Name);
        }

        [Serializable]
        public struct Value {
            public byte[] Bytes;
        }

        [SerializeField] private List<Tag> tags;
        [SerializeField] private List<Value> values;
        [SerializeField] private List<string> strings;

        public Entry(List<Tag> tags, List<Value> values, List<string> strings) {
            this.tags = tags;
            this.values = values;
            this.strings = strings;
        }

        public int? GetCount(string name) {
            var tag = GetTag(name);
            if (tag == null) return null;
            return tag.ArrayCount;
        }

        public int? GetInt(string name) => GetInt(name, 0);
        public float? GetFloat(string name) => GetFloat(name, 0);
        public bool? GetBool(string name) => GetBool(name, 0);
        public string GetString(string name) => GetString(name, 0);

        public int? GetInt(string name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return BitConverter.ToInt32(value.Value.Bytes, 0);
        }

        public float? GetFloat(string name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return BitConverter.ToSingle(value.Value.Bytes, 0);
        }

        public bool? GetBool(string name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return BitConverter.ToBoolean(value.Value.Bytes, 0);
        }

        public string GetString(string name, int i) {
            var stringIndex = GetInt(name, i);
            if (!stringIndex.HasValue) return null;
            return strings[stringIndex.Value];
        }

        private Tag GetTag(string name) {
            Tag key = new Tag {
                Name = name
            };
            var tagIndex = tags.BinarySearch(key);
            if (tagIndex < 0) return null;
            return tags[tagIndex];
        }

        private Value? GetValue(string name, int i) {
            var tag = GetTag(name);
            if (tag == null) return null;
            if (i >= tag.ArrayCount) return null;
            return values[tag.Offset + i];
        }
    }
}