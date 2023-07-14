# Injection de Shellcode dans un Processus Cible

Ce programme démontre comment injecter un shellcode dans un processus cible en utilisant les fonctions de la bibliothèque Windows.

## Fonctionnement

1. Le shellcode est une séquence d'instructions machine représentée sous forme de tableau d'octets. Dans ce code, le shellcode est défini comme un tableau d'octets `shellcode[]`.

2. Le PID (ID de processus) du processus cible est passé en argument lors de l'exécution du programme.

3. Le programme effectue les étapes suivantes pour injecter le shellcode dans le processus cible :

   - **Ouverture du processus cible** : Le processus cible est ouvert à l'aide de la fonction `OpenProcess` en spécifiant les privilèges d'accès complets.

   - **Allocation d'un tampon mémoire** : Un tampon mémoire est alloué dans le processus cible à l'aide de la fonction `VirtualAllocEx`.

   - **Copie du shellcode** : Le shellcode est copié dans le tampon mémoire du processus cible à l'aide de la fonction `WriteProcessMemory`.

   - **Création d'un thread distant** : Un thread distant est créé à l'aide de la fonction `CreateRemoteThread`, qui exécute le code du shellcode dans le processus cible.

   - **Fermeture du gestionnaire de processus** : Le gestionnaire du processus cible est fermé à l'aide de la fonction `CloseHandle`.

## Utilisation

1. Compiler le programme en utilisant un compilateur C++ compatible.

2. Exécuter le programme en spécifiant le PID du processus cible comme argument en ligne de commande.

