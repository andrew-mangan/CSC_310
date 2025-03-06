using System;
using System.Collections.Generic;

public class HashTable<TKey, TValue>
{
    private class HashTableEntry
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public HashTableEntry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    private HashTableEntry[] table;
    private int size;
    private int count;
    private double loadFactor;

    public HashTable(int initialSize = 10, double loadFactor = 0.75)
    {
        this.size = initialSize;
        this.loadFactor = loadFactor;
        this.table = new HashTableEntry[size];
        this.count = 0;
    }

    private int Hash1(TKey key)
    {
        return key.GetHashCode() % size;
    }

    private int Hash2(TKey key)
    {
        return 1 + (key.GetHashCode() % (size - 1)); // Must not return 0
    }

    private int FindIndexForInsert(TKey key)
    {
        int index = Hash1(key);
        int step = Hash2(key);

        // Resolve collisions using double hashing
        while (table[index] != null && !EqualityComparer<TKey>.Default.Equals(table[index].Key, key))
        {
            index = (index + step) % size;
        }
        return index;
    }

    private void Resize()
    {
        size *= 2;
        HashTableEntry[] newTable = new HashTableEntry[size];

        // Rehash all the entries into the new table
        foreach (var entry in table)
        {
            if (entry != null)
            {
                int index = FindIndexForInsert(entry.Key);
                newTable[index] = entry;
            }
        }

        table = newTable;
    }

    public void Insert(TKey key, TValue value)
    {
        // Resize if necessary
        if ((double)count / size >= loadFactor)
        {
            Resize();
        }

        int index = FindIndexForInsert(key);

        if (table[index] == null)  // Empty slot
        {
            table[index] = new HashTableEntry(key, value);
            count++;
        }
        else
        {
            table[index].Value = value;  // Update value if the key already exists
        }
    }

    public bool Get(TKey key, out TValue value)
    {
        int index = FindIndexForInsert(key);

        if (table[index] != null && EqualityComparer<TKey>.Default.Equals(table[index].Key, key))
        {
            value = table[index].Value;
            return true;
        }

        value = default(TValue);
        return false;
    }

    public bool Remove(TKey key)
    {
        int index = FindIndexForInsert(key);

        if (table[index] != null && EqualityComparer<TKey>.Default.Equals(table[index].Key, key))
        {
            table[index] = null;
            count--;
            return true;
        }

        return false;
    }

    public void Print()
    {
        for (int i = 0; i < table.Length; i++)
        {
            if (table[i] != null)
            {
                Console.WriteLine($"{table[i].Key}: {table[i].Value}");
            }
        }
    }
}

// Usage
public class Program
{
    public static void Main()
    {
        HashTable<string, int> ht = new HashTable<string, int>();

        // Insert some key-value pairs
        Console.WriteLine("Insert apple.\n");

        ht.Insert("Andrew", 1);
        Console.WriteLine("Insert banana.\n");
        ht.Insert("Salad", 2);
        Console.WriteLine("Insert orange.\n");
        ht.Insert("snop", 3);

        ht.Print();

        /* Get a value by key
        if (ht.Get("Salad", out int value))
        {
            Console.WriteLine("Salad: " + value);
        }*/

        if (ht.Get("apple", out int value))
        {
            Console.WriteLine("apple: " + value);
        }
        else Console.WriteLine("apple doesn't exist.");
             


        // Remove a key-value pair
        Console.WriteLine("Andrew Removed.");
        ht.Remove("Andrew");

        if (!ht.Get("Andrew", out value))
        {
            Console.WriteLine("Andrew not found");
        }

        // Print the hash table
        ht.Print();
    }
}

