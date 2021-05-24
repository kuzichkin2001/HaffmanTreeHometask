using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace HaffmanProject
{
    class Program
    {
        static void Main(string[] args)
        {
            HaffmanTree tree = new HaffmanTree();
            string inputMessage = "";
            using (StreamReader inFile = new StreamReader(@"C:\Users\kuzic\source\repos\HaffmanProject\HaffmanProject\input.txt"))
            {
                inputMessage = inFile.ReadToEnd();
            }
            if (inputMessage.Length > 1)
            {
                tree.BuildTree(inputMessage);
                BitArray encodedMessage = tree.EncodeMessage(inputMessage);
                using (BinaryWriter binFileWriter = new BinaryWriter(File.Open(@"C:\Users\kuzic\source\repos\HaffmanProject\HaffmanProject\task.bin", FileMode.Create)))
                {
                    byte[] bytes = new byte[encodedMessage.Length / 8 + (encodedMessage.Length % 8 == 0 ? 0 : 1)];
                    encodedMessage.CopyTo(bytes, 0);
                    binFileWriter.Write(bytes);
                }
            }

            BitArray encodedBinaryMessage;
            using (BinaryReader binFileReader = new BinaryReader(File.Open(@"C:\Users\kuzic\source\repos\HaffmanProject\HaffmanProject\task.bin", FileMode.Open)))
            {
                byte[] bytes = new byte[(int)binFileReader.BaseStream.Length];
                int k = 0;
                while (binFileReader.BaseStream.Position != binFileReader.BaseStream.Length)
                {
                    bytes[k] = binFileReader.ReadByte();
                    k++;
                }
                encodedBinaryMessage = new BitArray(bytes);
            }
            string decodedMessage = tree.DecodeBinaryMessage(encodedBinaryMessage);

            using (StreamWriter outFile = new StreamWriter(@"C:\Users\kuzic\source\repos\HaffmanProject\HaffmanProject\output.txt", false))
            {
                outFile.WriteLine(decodedMessage);
            }
            Console.WriteLine("Decoded message: " + decodedMessage);
        }
    }
}
