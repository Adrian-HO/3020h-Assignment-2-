using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace partC
{

    public interface IContainer<T>
    {
        void MakeEmpty();         // Reset to empty
        bool Empty();             // Return true if empty; false otherwise 
        int Size();               // Return size 
    }

    //-------------------------------------------------------------------------

    public interface ISearchable<T> : IContainer<T>
    {
        void Add(T item);         // Add item to the treap (duplicates are not permitted)     
        void Remove(T item);      // Remove item from the treap
        bool Contains(T item);    // Return true if item found; false otherwise
    }

    //-------------------------------------------------------------------------

    // Generic node class for a Treap

    public class Node<T> where T : IComparable
    {
        private static Random R = new Random();

        // Read/write properties

        public T Item { get; set; }
        public int Priority { get; set; }   // Randomly generated
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }
        public T MinValue { get; set; }     // The minimum value in the subtree rooted at this node
        public T MaxValue { get; set; }     // The maximum value in the subtree rooted at this node
        public int MinGap { get; set; }     // for computing the minimum gap

        // Node constructor
        public Node(T item)
        {
            Item = item;
            Priority = R.Next(10, 100);
            Left = Right = null;
            MinValue = item;
            MaxValue = item;
            MinGap = int.MaxValue;      // Initialize MinGap to largest integer value
        }

        // UpdateMinMax
        // Updates the MinValue and MaxValue fields based on the left and right subtrees
        // Time complexity:  O(1)

        public void UpdateMinMax()
        {
            MinValue = Item;
            MaxValue = Item;

            if (Left != null)
            {
                MinValue = Left.MinValue;
                MaxValue = Left.MaxValue;
            }

            if (Right != null)
            {
                if (Right.MaxValue.CompareTo(MaxValue) > 0)
                    MaxValue = Right.MaxValue;
                if (Right.MinValue.CompareTo(MinValue) < 0)
                {
                    MinValue = Right.MinValue;
                    if (Math.Abs(MaxValue.CompareTo(MinValue)) < MinGap)
                        MinGap = Math.Abs(MaxValue.CompareTo(MinValue));
                }

                // Recursively compute the minimum gap in the left and right subtrees
                if (Left != null)
                    MinGap = Math.Min(MinGap, Left.MinGap);
                if (Right != null)
                    MinGap = Math.Min(MinGap, Right.MinGap);

            }
        }
    }

    //-------------------------------------------------------------------------

    // Implementation:  Treap

    class Treap<T> : ISearchable<T> where T : IComparable
    {
        private Node<T> Root;  // Reference to the root of the Treap
        private Node<T> MinValue;  // Reference to the node with minimum value
        private Node<T> MaxValue;  // Reference to the node with maximum value

        // Constructor Treap
        // Creates an empty Treap
        // Time complexity:  O(1)

        public Treap()
        {
            MakeEmpty();
        }

        // LeftRotate
        // Performs a left rotation around the given root
        // Time complexity:  O(1)

        private Node<T> LeftRotate(Node<T> root)
        {
            Node<T> temp = root.Right;
            root.Right = temp.Left;
            temp.Left = root;
            return temp;
        }

        // RightRotate
        // Performs a right rotation around the given root
        // Time complexity:  O(1)

        private Node<T> RightRotate(Node<T> root)
        {
            Node<T> temp = root.Left;
            root.Left = temp.Right;
            temp.Right = root;
            return temp;
        }

        // Public Add
        // Inserts the given item into the Treap
        // Calls Private Add to carry out the actual insertion
        // Expected time complexity:  O(log n)

        public void Add(T item)
        {
            Root = Add(item, Root);
            // Root.UpdateMinMax();
        }

        // Add 
        // Inserts item into the Treap and returns a reference to the root
        // Duplicate items are not inserted
        // Expected time complexity:  O(log n)

        private Node<T> Add(T item, Node<T> root)
        {
            int cmp;  // Result of a comparison

            if (root == null)
                return new Node<T>(item);
            else
            {
                cmp = item.CompareTo(root.Item);
                if (cmp > 0)
                {
                    //code to compare gap
                    int gap = Math.Abs((int)(object)root.Item - (int)(object)item);
                    if (gap < root.MinGap)
                    {
                        root.MinGap = gap; //update minimum gap

                    }

                    root.Right = Add(item, root.Right);       // Move right
                    if (root.Right.Priority > root.Priority)  // Rotate left
                        root = LeftRotate(root);              // (if necessary)

                }
                else if (cmp < 0)
                {
                    //code to compare gap
                    int gap = Math.Abs((int)(object)item - (int)(object)root.Item);
                    if (gap < root.MinGap)
                        root.MinGap = gap; //update minimum gap

                    root.Left = Add(item, root.Left);         // Move left
                    if (root.Left.Priority > root.Priority)   // Rotate right
                        root = RightRotate(root);             // (if necessary)

                }
                // else if (cmp == 0) ... do nothing
                return root;
            }
        }


        // Public Remove
        // Removes the given item from the Treap
        // Calls Private Remove to carry out the actual removal
        // Expected time complexity:  O(log n)

        public void Remove(T item)
        {
            Root = Remove(item, Root);
            Root.UpdateMinMax();
        }

        // Remove 
        // Removes the given item from the Treap
        // Nothing is performed if the item is not found
        // Time complexity:  O(log n)

        private Node<T> Remove(T item, Node<T> root)
        {
            int cmp;  // Result of a comparison

            if (root == null)   // Item not found
                return null;
            else
            {
                cmp = item.CompareTo(root.Item);
                if (cmp < 0)
                    root.Left = Remove(item, root.Left);      // Move left
                else if (cmp > 0)
                    root.Right = Remove(item, root.Right);    // Move right
                else if (cmp == 0)                            // Item found
                {
                    // Case: Two children
                    // Rotate the child with the higher priority to the given root
                    if (root.Left != null && root.Right != null)
                    {
                        if (root.Left.Priority > root.Right.Priority)
                            root = RightRotate(root);
                        else
                            root = LeftRotate(root);
                    }
                    // Case: One child
                    // Rotate the left child to the given root
                    else if (root.Left != null)
                        root = RightRotate(root);
                    // Rotate the right child to the given root
                    else if (root.Right != null)
                        root = LeftRotate(root);

                    // Case: No children (i.e. a leaf node)
                    // Snip off the leaf node containing item
                    else
                        return null;

                    // Recursively move item down the Treap
                    root = Remove(item, root);

                }
                return root;
            }
        }

        // Public MinGap
        // Returns the minimum difference between adjacent values in the Treap
        // Expected time complexity:  O(1)
        public int MinGap()
        {
            if (Root == null)
                return -1;

            Node<T> current = Root;
            Node<T> prev = null;
            int minGap = int.MaxValue;

            while (current != null)
            {
                if (current.Left == null)
                {
                    if (prev != null) //if left node is not null
                    {
                        int gap = Math.Abs((int)(object)current.Item - (int)(object)prev.Item);
                        if (gap < minGap)
                            minGap = gap; //update minimum gap
                    }

                    //if left null, shift set to right
                    prev = current;
                    current = current.Right;
                }
                else
                {
                    Node<T> temp = current.Left;

                    while (temp.Right != null && temp.Right != current)
                        temp = temp.Right;

                    if (temp.Right == null)
                    {
                        temp.Right = current;
                        current = current.Left;
                    }
                    else
                    {
                        temp.Right = null;

                        int gap = Math.Abs((int)(object)current.Item - (int)(object)prev.Item);
                        if (gap < minGap)
                            minGap = gap;

                        prev = current;
                        current = current.Right;
                    }
                }
            }

            return minGap;
        }

        public int GetMinGape()
        {
            return Root.MinGap;
        }

        // Contains
        // Returns true if the given item is found in the Treap; false otherwise
        // Expected Time complexity:  O(log n)

        public bool Contains(T item)
        {
            Node<T> curr = Root;

            while (curr != null)
            {
                if (item.CompareTo(curr.Item) == 0)     // Found
                    return true;
                else
                   if (item.CompareTo(curr.Item) < 0)
                    curr = curr.Left;               // Move left
                else
                    curr = curr.Right;              // Move right
            }
            return false;
        }

        // MakeEmpty
        // Creates an empty Treap

        public void MakeEmpty()
        {
            Root = null;
        }

        // Empty
        // Returns true if the Treap is empty; false otherwise

        public bool Empty()
        {
            return Root == null;
        }

        // Public Size
        // Returns the number of items in the Treap
        // Calls Private Size to carry out the actual calculation
        // Time complexity:  O(n)

        public int Size()
        {
            return Size(Root);
        }

        // Size
        // Returns the number of items in the given Treap
        // Time complexity:  O(n)

        private int Size(Node<T> root)
        {
            if (root == null)
                return 0;
            else
                return 1 + Size(root.Left) + Size(root.Right);
        }

        // Public Height
        // Returns the height of the Treap
        // Calls Private Height to carry out the actual calculation
        // Time complexity:  O(n)

        public int Height()
        {
            return Height(Root);
        }

        // Private Height
        // Returns the height of the given Treap
        // Time complexity:  O(n)

        private int Height(Node<T> root)
        {
            if (root == null)
                return -1;    // By default for an empty Treap
            else
                return 1 + Math.Max(Height(root.Left), Height(root.Right));
        }

        // Public Print
        // Prints out the items of the Treap inorder
        // Calls Private Print to 

        public void Print()
        {
            Print(Root, 0);
        }

        // Print
        // Inorder traversal of the BST
        // Time complexity:  O(n)

        private void Print(Node<T> root, int index)
        {
            if (root != null)
            {
                Print(root.Right, index + 5);
                Console.WriteLine(new String(' ', index) + root.Item.ToString() + " " + root.Priority.ToString());
                Print(root.Left, index + 5);
            }
        }
    }

    //-----------------------------------------------------------------------------

    internal class Program
    {
        static Random V = new Random();
        static void Main(string[] args)
        {
            Treap<int> B = new Treap<int>();

            for (int i = 0; i < 5; i++)
                B.Add(V.Next(10, 50));     // Add random integers from 10 to 99
                                           // B.Add(41);
                                           // B.Add(19);
                                           // B.Add(16);
                                           // B.Add(24);
                                           // B.Add(14);

            B.Print();

            Console.ReadLine();

            Console.WriteLine("Size of the Treap  : " + B.Size());
            Console.WriteLine("Height of the Treap: " + B.Height());
            Console.WriteLine("Contains 42        : " + B.Contains(42));
            Console.WriteLine("Contains 68        : " + B.Contains(68));
            Console.WriteLine("MinGap of the Treap: " + B.MinGap());
            Console.WriteLine("O(log(n)): " + B.GetMinGape());

            Console.ReadLine();

            for (int i = 0; i < 50; i++)
                B.Remove(V.Next(10, 100));  // Remove random integers

            B.Print();

            Console.WriteLine("Size of the Treap  : " + B.Size());
            Console.WriteLine("Height of the Treap: " + B.Height());
            Console.WriteLine("Contains 42        : " + B.Contains(42));
            Console.WriteLine("Contains 68        : " + B.Contains(68));
            Console.WriteLine("MinGap of the Treap: " + B.MinGap());

            Console.ReadLine();
        }
    }
}
