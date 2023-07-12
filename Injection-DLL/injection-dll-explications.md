# Injection de DLL

L'injection de DLL (Dynamic Link Library) est une technique utilisée pour injecter une bibliothèque dynamique dans un processus en cours d'exécution. Cette technique permet d'étendre les fonctionnalités d'un programme en injectant du code externe dans son espace d'adressage.

## Processus d'injection de DLL

Pour effectuer une injection de DLL, voici une explication générale des étapes impliquées :

1. **Chargement de la DLL** : Le chargement de la DLL à injecter est effectué en utilisant la fonction `LoadLibrary` de la bibliothèque Windows.

2. **Allocation de mémoire dans le processus cible** : L'allocation d'un espace mémoire dans le processus cible est réalisée en utilisant la fonction `VirtualAllocEx`.

3. **Copie du code de la DLL dans le processus cible** : Le code de la DLL est copié dans l'espace mémoire alloué du processus cible à l'aide de la fonction `WriteProcessMemory`.

4. **Résolution des dépendances de la DLL** : Si la DLL injectée dépend d'autres bibliothèques, il peut être nécessaire de résoudre les adresses de ces dépendances dans l'espace mémoire du processus cible.

5. **Injection du point d'entrée** : L'appel au point d'entrée de la DLL injectée est effectué en créant un thread distant à l'aide de la fonction `CreateRemoteThread`.

## Avertissement

Il est important de noter que l'injection de DLL est une technique avancée, qui peut avoir des implications légales et de sécurité. L'utilisation de cette technique à des fins malveillantes est illégale.

