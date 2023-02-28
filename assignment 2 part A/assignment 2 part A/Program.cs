using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_2_part_A
{
    public interface IContainer<T>
    {
        void MakeEmpty();
        bool Empty();
        int Size();
    }

    //-------------------------------------------------------------------------

    public interface ITrie<T> : IContainer<T>
    {
        bool Insert(string key, T value);
        bool Remove(string key);
        T Value(string key);
    }

    //-------------------------------------------------------------------------

    class Trie<T> : ITrie<T>
    {
        private Node root;          // Root node of the Trie

        private class Node
        {
            public T value;         // Value associated with a key; otherwise default
            public int numValues;   // Number of descendent values of a Node 
            public Node[] child;    // Branch for each letter 'a' .. 'z'

            // Node
            // Creates an empty Node
            // All children are set to null by default
            // Time complexity:  O(1)

            public Node()
            {
                value = default(T);
                numValues = 0;
                child = new Node[26];
            }
        }

        // Trie
        // Creates an empty Trie
        // Time complexity:  O(1)

        public Trie()
        {
            MakeEmpty();
        }

        // Public Insert
        // Calls the private Insert which carries out the actual insertion
        // Returns true if successful; false otherwise

        public bool Insert(string key, T value)
        {
            return Insert(root, key, 0, value);
        }

        // Private Insert
        // Inserts the key/value pair into the Trie
        // Returns true if the insertion was successful; false otherwise
        // Note: Duplicate keys are ignored
        // Time complexity:  O(L) where L is the length of the key

        private bool Insert(Node p, string key, int j, T value)
        {
            int i;

            if (j == key.Length)
            {
                if (p.value.Equals(default(T)))
                {
                    // Sets the value at the Node
                    p.value = value;
                    p.numValues++;
                    return true;
                }
                // Duplicate keys are ignored (unsuccessful insertion)
                else
                    return false;
            }
            else
            {
                // Maps a character to an index
                i = Char.ToLower(key[j]) - 'a';

                // Creates a new Node if the link is null
                // Note: Node is initialized to the default value
                if (p.child[i] == null)
                    p.child[i] = new Node();

                // If the inseration is successful
                if (Insert(p.child[i], key, j + 1, value))
                {
                    // Increase number of descendant values by one
                    p.numValues++;
                    return true;
                }
                else
                    return false;
            }
        }

        // Value
        // Returns the value associated with a key; otherwise default
        // Time complexity:  O(min(L,M)) where L is the length of the given key and
        //                                     M is the maximum length of a key in the trie

        public T Value(string key)
        {
            int i;
            Node p = root;

            // Traverses the links character by character
            foreach (char ch in key)
            {
                i = Char.ToLower(ch) - 'a';
                if (p.child[i] == null)
                    return default(T);    // Key is too long
                else
                    p = p.child[i];
            }
            return p.value;               // Returns the value or default
        }

        // Public Remove
        // Calls the private Remove that carries out the actual deletion
        // Returns true if successful; false otherwise

        public bool Remove(string key)
        {
            return Remove(root, key, 0);
        }

        // Private Remove
        // Removes the value associated with the given key
        // Time complexity:  O(min(L,M)) where L is the length of the key
        //                               where M is the maximum length of a key in the trie

        private bool Remove(Node p, string key, int j)
        {
            int i;

            // Key not found
            if (p == null)
                return false;

            else if (j == key.Length)
            {
                // Key/value pair found
                if (!p.value.Equals(default(T)))
                {
                    p.value = default(T);
                    p.numValues--;
                    return true;
                }
                // No value with associated key
                else
                    return false;
            }

            else
            {
                i = Char.ToLower(key[j]) - 'a';

                // If the deletion is successful
                if (Remove(p.child[i], key, j + 1))
                {
                    // Decrease number of descendent values by one and
                    // Remove Nodes with no remaining descendents
                    if (p.child[i].numValues == 0)
                        p.child[i] = null;
                    p.numValues--;
                    return true;
                }
                else
                    return false;
            }
        }

        // MakeEmpty
        // Creates an empty Trie
        // Time complexity:  O(1)

        public void MakeEmpty()
        {
            root = new Node();
        }

        // Empty
        // Returns true if the Trie is empty; false otherwise
        // Time complexity:  O(1)

        public bool Empty()
        {
            return root.numValues == 0;
        }

        // Size
        // Returns the number of Trie values
        // Time complexity:  O(1)

        public int Size()
        {
            return root.numValues;
        }

        // Public Print
        // Calls private Print to carry out the actual printing

        public void Print()
        {
            Print(root, "");
        }

        // Private Print
        // Outputs the key/value pairs ordered by keys
        // Time complexity:  O(n) where n is the number of nodes in the trie

        private void Print(Node p, string key)
        {
            int i;

            if (p != null)
            {
                if (!p.value.Equals(default(T)))
                    Console.WriteLine(key + " " + p.value + " " + p.numValues);
                for (i = 0; i < 26; i++)
                    Print(p.child[i], key + (char)(i + 'a'));
            }
        }





        //
        public List<string> PartialMatch(string pattern)
        {
            List<string> keys = new List<string>(); //create a list of strings

            return PartialMatch(pattern, "", keys, root); //call the recursive method to find any strings from the given pattern

        }

        //
        private List<string> PartialMatch(string pat, string path, List<string> k, Node cur)
        {
            if (cur != null) //if not off the end of the list
            {
                if (pat.Length != 0) //if there is more to the pattern
                {
                    char c = pat[0]; //set c to the first letter in the pattern

                    if (c == '*') //if c is a wild card
                    {
                        for (int i = 0; i < 26; i++) //check every letter for a node
                        {
                            PartialMatch(pat.Remove(0, 1), path + (char)(i + 'a'), k, cur.child[i]); //recursivly call method, removing a letter from the pattern and adding the letter being checked to the path
                        }
                    }
                    else
                        PartialMatch(pat.Remove(0, 1), path + c, k, cur.child[c - 'a']); //recursivly call method, removing a letter from the pattern and adding the current letter to the path
                }
                else //if the whole pattern has been searched
                {
                    if (!cur.value.Equals(default(T))) //add string to list if it is a finished string
                        k.Add(path);
                    return k;//return list
                }
            }
            
            return k; //return list
        }

        //
        public List<string> Autocomplete(string prefex)
        {
            //Return the list of keys that begin with the given prefex
            List<string> keys = new List<string>();

            return Autocomplete(prefex, "", keys, root);
        }

        //
        private List<string> Autocomplete(string pre, string path, List<string> k, Node cur)
        {
            if (cur != null) //if not off the end of the list
            {
                char c = '0'; //set c to a defult value

                if (pre.Length != 0)
                    c = pre[0]; //set c to the first letter in the prefex

                if (pre.Length == 0) //if all letters in the prefex hav been explored
                {
                    for (int i = 0; i < 26; i++) //check every letter for a node
                    {
                    Autocomplete(pre, path + (char)(i + 'a'), k, cur.child[i]); //recursivly call method, adding the letter being checked to the path
                    }
                    if (!cur.value.Equals(default(T))) //add string to list if it is a finished string
                        k.Add(path);
                }
                else
                    Autocomplete(pre.Remove(0, 1), path + c, k, cur.child[c - 'a']); //recursivly call method, removing a letter from the prefex and adding the current letter to the path
            }

            return k; //return list
        }

        //
        public List<string> Autocorrect(string key)
        {
            //Return the list of keys that differ from the given key by one letter. Try to be as effecient as possible.
            List<string> k = new List<string>();
            Node p = root;
            string path = "";
            foreach (char c in key)
            {
                if (p.child[c - 'a'] != null)
                {
                    p = p.child[c - 'a'];
                    path = path + c;
                    for (int i = 0; i < 26; i++)
                    {
                        if (p.child[i] != null)
                            if (!p.child[i].value.Equals(default(T)))
                                k.Add(path + (char)(i + 'a'));
                    }
                }
            }

            return k;
        }



    }

    class Program
    {
        static void Main(string[] args)
        {
            Random r = new Random();

            //code has been modifed
            //code taken from https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-a-text-file-one-line-at-a-time
            //will read text from a file one line at a time
            //will read from file path provided in the system.IO.File.ReadLines method

            Trie<int> T;
            T = new Trie<int>();
            
            // Read the file line by line and use each line of text as a key  
            foreach (string line in System.IO.File.ReadLines(@"C:\Users\kiera\Desktop\Assingment2_key_values.txt"))
            {
                foreach(char c in line)
                    char.ToLower(c);
                T.Insert(line, r.Next(1, 500));
            }

            T.Print();

            List<string> match = T.PartialMatch("y*u*s*l*"); //call the partal match method
            List<string> autocom = T.Autocomplete("you");
            List<string> autocor = T.Autocorrect("yoru");

            //print all partal matches
            Console.WriteLine("\n \n \n" + "strings that match the pattern: ");

            foreach (string s in match)
                Console.WriteLine("{0} ", s);

            //print all auto completes
            Console.WriteLine("\n \n \n" + "strings with prefex: ");

            foreach (string s in autocom)
                Console.WriteLine("{0} ", s);


            //print all auto corrects
            Console.WriteLine("\n \n \n" + "auto corrected strings: ");

            foreach (string s in autocor)
                Console.WriteLine("{0} ", s);


            Console.ReadKey();

        }
    }
}

