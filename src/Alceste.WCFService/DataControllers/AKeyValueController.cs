using System;
using System.Collections.Generic;
using System.Linq;

namespace Alceste.WCFService.DataControllers
{
    public abstract class AKeyValueController<K, V>
    {
        public AKeyValueController()
        {
            LoadValues();
        }

        private readonly Dictionary<K, V> DataItems = new Dictionary<K, V>();

        public abstract void LoadValues();

        protected void AddItem(K key, V value)
        {
            DataItems.Add(key, value);
        }

        protected virtual string NoKeyExceptionText
        {
            get { return "Key \"{0}\" is not found"; }
        }

        protected virtual string NoValueExceptionText
        {
            get { return "Value \"{0}\" is not found"; }
        }

        public V GetValueByKey(K key)
        {
            if (!DataItems.ContainsKey(key))
                throw new Exception(string.Format(NoKeyExceptionText, key));
            return DataItems[key];
        }

        public K GetKeyByValue(V value)
        {
            if (!DataItems.ContainsValue(value))
                throw new Exception(string.Format(NoValueExceptionText, value));
            return DataItems.FirstOrDefault(item => Equals(item.Value, value)).Key;
        }
    }
}
