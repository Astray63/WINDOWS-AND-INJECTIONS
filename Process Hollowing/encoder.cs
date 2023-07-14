using System;
using System.Text;

namespace XorCoder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Shellcode encodé avec XOR (clé fixe)
            byte[] shellcode = new byte[511] {
                // Shellcode ici...
            };

            // Encode le shellcode avec XOR (clé fixe)
            byte[] encodedShellcode = new byte[shellcode.Length];
            byte xorKey = 0xFA; // Clé XOR fixe
            for (int i = 0; i < shellcode.Length; i++)
            {
                encodedShellcode[i] = (byte)(shellcode[i] ^ xorKey);
            }

            // Conversion du shellcode encodé en chaîne hexadécimale
            StringBuilder hexBuilder = new StringBuilder(encodedShellcode.Length * 2);
            int totalCount = encodedShellcode.Length;
            for (int count = 0; count < totalCount; count++)
            {
                byte b = encodedShellcode[count];

                if ((count + 1) == totalCount) // Ne pas ajouter de virgule pour le dernier élément
                {
                    hexBuilder.AppendFormat("0x{0:x2}", b);
                }
                else
                {
                    hexBuilder.AppendFormat("0x{0:x2}, ", b);
                }

                if ((count + 1) % 15 == 0)
                {
                    hexBuilder.Append("\n");
                }
            }

            Console.WriteLine($"Payload XOR (clé: 0x{xorKey:x2}):");
            Console.WriteLine($"byte[] encodedShellcode = new byte[{encodedShellcode.Length}] {{\n{hexBuilder}\n}};");
        }
    }
}
