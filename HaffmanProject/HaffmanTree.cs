using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace HaffmanProject
{
    public class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public char Character { get; set; }
        public int CharacterFrequency { get; set; }

        public List<bool> ToBinarySuccessions(char ch, List<bool> items)
        {
            if (Right == null && Left == null)
            {
                if (ch == Character)
                {
                    return items;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    List<bool> leftSuccession = new List<bool>();
                    leftSuccession.AddRange(items);
                    leftSuccession.Add(false);

                    left = Left.ToBinarySuccessions(ch, leftSuccession);
                }
                if (Right != null)
                {
                    List<bool> rightSuccession = new List<bool>();
                    rightSuccession.AddRange(items);
                    rightSuccession.Add(true);
                    right = Right.ToBinarySuccessions(ch, rightSuccession);
                }

                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
    }

    public class HaffmanTree
    {
        private List<Node> _nodes = new List<Node>();
        public Node Root { get; set; }
        public Dictionary<char, int> messageLettersDictionary = new Dictionary<char, int>();

        public void BuildTree(string message)
        {
            for (int i = 0; i < message.Length; ++i)
            {
                if (!messageLettersDictionary.ContainsKey(message[i]))
                {
                    messageLettersDictionary.Add(message[i], 1);
                }
                else
                {
                    messageLettersDictionary[message[i]]++;
                }
            }
            foreach (var pair in messageLettersDictionary)
            {
                _nodes.Add(new Node() { Character = pair.Key, CharacterFrequency = pair.Value });
            }
            while (_nodes.Count > 1)
            {
                List<Node> nodesOrderedByFrequency = _nodes.OrderBy(node => node.CharacterFrequency).ToList<Node>();

                if (nodesOrderedByFrequency.Count >= 2)
                {
                    List<Node> taken = nodesOrderedByFrequency.Take(2).ToList<Node>();
                    Node parent = new Node()
                    {
                        Character = '&',
                        CharacterFrequency = taken[0].CharacterFrequency + taken[1].CharacterFrequency,
                        Left = taken[0],
                        Right = taken[1]
                    };

                    _nodes.Remove(taken[0]);
                    _nodes.Remove(taken[1]);
                    _nodes.Add(parent);
                }
                Root = _nodes.FirstOrDefault();
            }
        }

        public BitArray EncodeMessage(string message)
        {
            List<bool> encodedMessage = new List<bool>();
            string characters = "";
            for (int i = 0; i < message.Length; ++i)
            {
                if (characters.IndexOf(message[i]) < 0)
                {
                    characters += message[i];
                }
            }

            List<bool>[] information = new List<bool>[characters.Length];
            for (int i = 0; i < characters.Length; ++i)
            {
                List<bool> encodedCharacter = this.Root.ToBinarySuccessions(characters[i], new List<bool>());
                information[i] = encodedCharacter;
            }

            List<string> s = new List<string>();
            for (int i = 0; i < characters.Length; ++i)
            {
                s.Add(Convert.ToString(characters[i], 2));
            }

            string length = Convert.ToString((int)characters.Length, 2);
            List<bool> currentBit = new List<bool>();
            string messageLength = Convert.ToString((int)message.Length, 2);

            if (messageLength.Length < 16)
            {
                for (int i = 0; i < 16 - messageLength.Length; ++i)
                {
                    currentBit.Add(false);
                }
            }
            for (int i = 0; i < messageLength.Length; ++i)
            {
                if (messageLength[i] == '0')
                {
                    currentBit.Add(false);
                }
                else
                {
                    currentBit.Add(true);
                }
            }

            if (length.Length < 16)
            {
                for (int i = 0; i < 16 - length.Length; ++i)
                {
                    currentBit.Add(false);
                }
            }
            for (int i = 0; i < length.Length; ++i)
            {
                if (length[i] == '0')
                {
                    currentBit.Add(false);
                }
                else
                {
                    currentBit.Add(true);
                }
            }

            for (int i = 0; i < characters.Length; ++i)
            {
                if (s[i].Length < 16)
                {
                    for (int j = 0; j < 16 - s[i].Length; ++j)
                    {
                        currentBit.Add(false);
                    }
                }
                for (int j = 0; j < s[i].Length; ++j)
                {
                    if (s[i][j] == '0')
                    {
                        currentBit.Add(false);
                    }
                    else
                    {
                        currentBit.Add(true);
                    }
                }
                int binaryMessageLength = information[i].Count;
                string binaryMessage = Convert.ToString(binaryMessageLength, 2);

                if (binaryMessage.Length < 16)
                {
                    for (int j = 0; j < 16 - binaryMessage.Length; ++j)
                    {
                        currentBit.Add(false);
                    }
                }
                for (int j = 0; j < binaryMessage.Length; ++j)
                {
                    if (binaryMessage[j] == '0')
                    {
                        currentBit.Add(false);
                    }
                    else
                    {
                        currentBit.Add(true);
                    }
                }

                foreach (var bit in information[i])
                    currentBit.Add(bit);
            }
            for (int i = 0; i < message.Length; ++i)
            {
                List<bool> encodedCharacter = Root.ToBinarySuccessions(message[i], new List<bool>());
                foreach (var bit in encodedCharacter)
                    currentBit.Add(bit);
            }

            BitArray bits = new BitArray(currentBit.ToArray());
            return bits;
        }

        public string DecodeBinaryMessage(BitArray bits)
        {
            string number = "";
            string bit = "";
            foreach (bool item in bits)
            {
                bit += ((item ? 1 : 0) + "");
            }
            for (int i = 0; i < 16; ++i)
            {
                number += bit[i];
            }
            int newNum = 0;
            int power = 1;
            for (int i = number.Length - 1; i >= 0; --i)
            {
                if (number[i] == '1')
                {
                    newNum += power;
                }
                power *= 2;
            }
            number = "";
            int l = newNum;
            newNum = 0;
            power = 1;
            for (int i = 16; i < 32; ++i)
            {
                number += bit[i];
            }
            int N = 32;
            newNum = 0;
            power = 1;
            for (int i = number.Length - 1; i >= 0; --i)
            {
                if (number[i] == '1')
                {
                    newNum += power;
                }
                power *= 2;
            }
            Dictionary<char, string> decodedMessageLetters = new Dictionary<char, string>();
            for (int i = 0; i < newNum; ++i)
            {
                char ch;
                int chNum = 0;
                string bitsStr = "";
                int newPower = 1;
                for (int j = N + 15; j >= N; --j)
                {
                    if (bit[j] == '1')
                    {
                        chNum += newPower;
                    }
                    newPower *= 2;
                }
                newPower = 1;
                N += 16;
                ch = (char)chNum;
                chNum = 0;
                for (int j = N + 15; j >= N; --j)
                {
                    if (bit[j] == '1')
                    {
                        chNum += newPower;
                    }
                    newPower *= 2;
                }
                N += 16;
                for (int j = N; j < N + chNum; ++j)
                {
                    bitsStr += bit[j];
                }
                N += chNum;
                decodedMessageLetters.Add(ch, bitsStr);
            }
            
            foreach (KeyValuePair<char, string> character in decodedMessageLetters)
            {
                Console.WriteLine($"{character.Key} - {character.Value}");
            }

            string result = "";
            string numChars = "";
            int count = 0;
            for (int i = N; i < bit.Length; ++i)
            {
                numChars += bit[i];
                foreach (KeyValuePair<char, string> character in decodedMessageLetters)
                {
                    int n1 = numChars.Length;
                    int n2 = character.Value.Length;
                    int n = 0;
                    if (n1 == n2 && count != l)
                    {
                        for (int j = 0; j < n1; ++j)
                        {
                            if (numChars[j] == character.Value[j]) ++n;
                        }
                        if (n == n1)
                        {
                            result += character.Key;
                            numChars = "";
                            ++count;
                        }
                    }
                }
            }
            return result;
        }

        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }
    }
}