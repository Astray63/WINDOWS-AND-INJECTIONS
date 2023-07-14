# Explication du code process.cs

Ce code utilise la technique de "Process Hollowing" pour injecter une charge utile dans un processus en cours d'exécution. Voici une explication détaillée du fonctionnement du code :

1. Les constantes `CREATION_SUSPENDUE` et `INFORMATIONS_PROCESSUS_DE_BASE` sont définies pour les drapeaux de création de processus.

2. La structure `InformationsProcessus` est définie avec des membres pour les handles du processus et du thread, ainsi que les IDs du processus et du thread.

3. La structure `InformationsDemarrage` est définie avec des membres pour les informations de démarrage du processus.

4. Plusieurs fonctions importées de différentes DLL sont utilisées, telles que `Sleep`, `CreateProcess`, `ZwQueryInformationProcess`, `ReadProcessMemory`, `WriteProcessMemory` et `ResumeThread`.

5. La méthode `Main` est le point d'entrée du programme. Elle effectue les étapes suivantes :

    - Mesure le temps écoulé pour l'évasion de l'antivirus en utilisant la fonction `Sleep`.
    - Définit une charge utile XORée.
    - Crée un nouveau processus `svchost.exe` en mode suspendu en utilisant la fonction `CreateProcess`.
    - Obtient des informations détaillées sur le processus en utilisant la fonction `ZwQueryInformationProcess`.
    - Calcule l'adresse de base de l'image exécutable du processus en ajoutant un décalage à l'adresse du Process Environment Block (PEB).
    - Calcule l'adresse du point d'entrée réel de l'exécutable en suivant une série d'étapes complexes.
    - Décode la charge utile XORée.
    - Écrase la mémoire à l'adresse du point d'entrée avec la charge utile décodée.
    - Reprend l'exécution du thread du processus pour déclencher la charge utile.

Ce code est utilisé pour des fins éducatives et de recherche. Il utilise des techniques avancées de manipulation de processus et de mémoire. Il est important de noter que l'utilisation de telles techniques pour des activités malveillantes est illégale et strictement interdite.

**Avertissement :** Utilisez ce code à vos propres risques et uniquement à des fins légales et éthiques.
