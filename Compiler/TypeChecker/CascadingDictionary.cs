using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker {
    public class CascadingDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> contents = new Dictionary<TKey, TValue>();
        private readonly CascadingDictionary<TKey, TValue> parent;

        private static readonly Random idRng = new Random();
        public readonly int ScopeId = idRng.Next();

        public IDictionary<TKey, TValue> TopLevel { get { return contents; } }

        public CascadingDictionary() {
            this.parent = null;
        }

        public CascadingDictionary<TKey, TValue> Parent {
            get { return parent; }
        }

        public CascadingDictionary(CascadingDictionary<TKey, TValue> parent) {
            this.parent = parent;
        }

        public bool ContainsKey(TKey key) {
            if (contents.ContainsKey(key)) {
                return true;
            }
            if (parent == null) {
                return false;
            }
            return parent.ContainsKey(key);
        }

        public void Add(TKey key, TValue value) {
            this[key] = value;
        }

        public bool Remove(TKey key) {
            if (contents.ContainsKey(key)) {
                contents.Remove(key);
                return true;
            }
            if (parent == null) {
                return false;
            }
            return parent.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            if (contents.ContainsKey(key)) {
                value = contents[key];
                return true;
            }
            return parent.TryGetValue(key, out value);
        }

        public TValue this[TKey key] {
            get {
                if (contents.ContainsKey(key)) {
                    return contents[key];
                }
                return parent[key];
            }

            set { contents[key] = value; }
        }

        public ICollection<TKey> Keys { get; private set; }
        public ICollection<TValue> Values { get; private set; }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return contents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            this[item.Key] = item.Value;
        }

        public void Clear() {
            contents.Clear();
            if (parent != null)
                parent.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            if (contents.ContainsKey(item.Key) && contents[item.Key].Equals(item.Value)) {
                return true;
            }
            if (parent == null) {
                return false;
            }
            return parent.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            if (contents.ContainsKey(item.Key) && contents[item.Key].Equals(item.Value)) {
                contents.Remove(item.Key);
                return true;
            }
            if (parent == null) {
                return false;
            }
            return parent.Remove(item);
        }

        public int Count { get { return contents.Count + (parent == null ? 0 : parent.Count); } }
        public bool IsReadOnly { get; private set; }
    }
}
