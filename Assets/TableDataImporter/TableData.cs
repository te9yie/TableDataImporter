using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TableDataImporter {
    [Serializable]
    public class TableData : ScriptableObject {
        [Serializable]
        public class Label : IComparable<Label> {
            public StringWithHash Name;
            public int Offset;

            public int CompareTo(Label other) => Name.CompareTo(other.Name);
        }

        [SerializeField] private List<Label> labels;
        [SerializeField] private List<Table> tables;

        public void Initialize(List<Label> labels, List<Table> tables) {
            this.labels = labels;
            this.tables = tables;
        }

        public Table GetTable(StringWithHash name) {
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
            public StringWithHash Name;
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

        public int? GetCount(StringWithHash name) {
            var label = GetLabel(name);
            if (label == null) return null;
            return label.ArrayCount;
        }

        public Entry GetEntry(StringWithHash name) => GetEntry(name, 0);

        public Entry GetEntry(StringWithHash name, int i) {
            var label = GetLabel(name);
            if (label == null) return null;
            if (i >= label.ArrayCount) return null;
            return entries[label.Offset + i];
        }

        private Label GetLabel(StringWithHash name) {
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
            public StringWithHash Name;
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

        public Entry(List<Tag> tags, List<Value> values) {
            this.tags = tags;
            this.values = values;
        }

        public int? GetCount(StringWithHash name) {
            var tag = GetTag(name);
            if (tag == null) return null;
            return tag.ArrayCount;
        }

        public int? GetInt(StringWithHash name) => GetInt(name, 0);
        public float? GetFloat(StringWithHash name) => GetFloat(name, 0);
        public bool? GetBool(StringWithHash name) => GetBool(name, 0);
        public string GetString(StringWithHash name) => GetString(name, 0);

        public int? GetInt(StringWithHash name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return BitConverter.ToInt32(value.Value.Bytes, 0);
        }

        public float? GetFloat(StringWithHash name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return BitConverter.ToSingle(value.Value.Bytes, 0);
        }

        public bool? GetBool(StringWithHash name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return BitConverter.ToBoolean(value.Value.Bytes, 0);
        }

        public string GetString(StringWithHash name, int i) {
            var value = GetValue(name, i);
            if (!value.HasValue) return null;
            return Encoding.Unicode.GetString(value.Value.Bytes);
        }

        private Tag GetTag(StringWithHash name) {
            Tag key = new Tag {
                Name = name
            };
            var tagIndex = tags.BinarySearch(key);
            if (tagIndex < 0) return null;
            return tags[tagIndex];
        }

        private Value? GetValue(StringWithHash name, int i) {
            var tag = GetTag(name);
            if (tag == null) return null;
            if (i >= tag.ArrayCount) return null;
            return values[tag.Offset + i];
        }
    }
}