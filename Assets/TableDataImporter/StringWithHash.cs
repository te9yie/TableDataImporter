using System;
using UnityEngine;

namespace TableDataImporter {
    [Serializable]
    public class StringWithHash : IComparable<StringWithHash> {
        [SerializeField] private string string_;
        [SerializeField] private uint hash;

        public static implicit operator StringWithHash(string s) {
            return new StringWithHash(s);
        }

        public StringWithHash(string s) {
            string_ = s;
            hash = CalcHash();
        }

        public int CompareTo(StringWithHash other) {
            var r = hash.CompareTo(other.hash);
            if (r != 0) return r;
            return string_.CompareTo(other.string_);
        }

        // 32bits FNV-1a hash.
        private uint CalcHash() {
            uint x = 2166136261;
            foreach (var c in string_) {
                x = (x ^ c) * 16777619U;
            }
            return x;
        }
    }
}
