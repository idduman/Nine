using System;

namespace Garawell.Data
{
    [System.Serializable]
    public class DictionaryData<T> : IDisposable
    {
        public string key;
        public T value;

        public void Dispose()
        {

        }
    } 
}
