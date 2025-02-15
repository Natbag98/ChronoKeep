using UnityEngine;
using System;
using System.Collections.Generic;

public class Utils : MonoBehaviour {
    public static T Choice<T>(T[] array) { return array[GameManager.Random.Next(array.Length)]; }
    public static T Choice<T>(List<T> list) { return list[GameManager.Random.Next(list.Count)]; }

    [System.Serializable]
    public class SerializeableDict<TKey, TValue> {
        [SerializeField] private SerializableDictPair[] dictPairs;

        [System.Serializable]
        private class SerializableDictPair {
            public TKey Key;
            public TValue Value;
        }

        public Dictionary<TKey, TValue> GetDict() {
            Dictionary<TKey, TValue> dict = new();
            foreach (SerializableDictPair pair in dictPairs) dict.Add(pair.Key, pair.Value);
            return dict;
        }
    }

    public static T CreateJaggedArray<T>(params int[] lengths) { return (T)InitializeJaggedArray(typeof(T).GetElementType(), 0, lengths); }
    public static bool SetFirstNull<T>(T element, T[] array) {
        for (int i = 0; i < array.Length; i++) if (array[i] == null) { array[i] = element; return true; }
        return false;
    }

    private static object InitializeJaggedArray(Type type, int index, int[] lengths) {
        Array array = Array.CreateInstance(type, lengths[index]);
        Type elementType = type.GetElementType();

        if (elementType != null) {
            for (int i = 0; i < lengths[index]; i++) {
                array.SetValue(InitializeJaggedArray(elementType, index + 1, lengths), i);
            }
        }

        return array;
    }
}
