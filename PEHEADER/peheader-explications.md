# PE HEADER pour l'analyse de malwares

## Introduction

Dans le domaine de l'analyse de malwares, comprendre la structure interne des fichiers exécutables est essentiel. Le format Portable Executable (PE) est le format de fichier binaire utilisé par les systèmes d'exploitation Windows pour les applications exécutables. Le PE HEADER (en-tête PE) est une partie clé de ce format, car il contient des informations importantes sur le fichier exécutable lui-même.

## Structure du PE HEADER

Le PE HEADER est situé au début du fichier exécutable et contient plusieurs en-têtes différents qui fournissent des informations détaillées sur le fichier. Voici une description de certains des en-têtes les plus importants :

### DOS HEADER

Le DOS HEADER est le premier en-tête dans le fichier exécutable et contient une petite structure appelée IMAGE_DOS_HEADER. Cet en-tête fournit des informations spécifiques au système d'exploitation MS-DOS, ainsi que l'adresse de départ du PE HEADER.

### Signature

L'octet de signature indique que le fichier est un exécutable PE. La signature est généralement représentée par les caractères "PE\0\0" (50 45 00 00 en hexadécimal).

### IMAGE_FILE_HEADER

Cet en-tête fournit des informations générales sur le fichier, telles que l'architecture cible, le nombre de sections, la date et l'heure de création, etc. Il contient une structure appelée IMAGE_FILE_HEADER qui est composée des champs suivants :

- Machine : Indique l'architecture cible pour laquelle le fichier a été compilé (par exemple, 0x014C pour x86, 0x8664 pour x64, etc.).
- NumberOfSections : Indique le nombre de sections dans le fichier.
- TimeDateStamp : Indique la date et l'heure de création du fichier.
- Characteristics : Fournit des informations sur les attributs du fichier, tels que s'il est exécutable, s'il est lié statiquement ou dynamiquement, etc.

### IMAGE_OPTIONAL_HEADER


Cet en-tête contient des informations détaillées sur les propriétés spécifiques du fichier. Il contient une structure appelée IMAGE_OPTIONAL_HEADER, qui peut varier en fonction de l'architecture cible. Certains des champs couramment utilisés comprennent :

- AddressOfEntryPoint : Indique l'adresse de départ de l'exécution du fichier.
- ImageBase : Indique l'adresse de base à laquelle le fichier est chargé en mémoire.
- BaseOfCode : Indique l'adresse de base du code exécutable.
- BaseOfData : Indique l'adresse de base des données initialisées.
- SizeOfImage : Indique la taille totale de l'image en mémoire.
- CheckSum : Indique la somme de contrôle du fichier exécutable.

### IMAGE_SECTION_HEADER


Cet en-tête contient des informations sur chaque section du fichier, telles que les adresses de début et de fin, les attributs, les noms, etc. Il peut y avoir plusieurs sections dans un fichier exécutable, chacune ayant un objectif spécifique, comme le code exécutable, les données, les ressources, etc.

## Analyse des PE HEADER

L'analyse du PE HEADER permet aux analystes de malwares de recueillir des informations précieuses sur le fichier exécutable. Voici quelques éléments clés à examiner lors de l'analyse :

1. Architecture cible : Le champ "Machine" dans l'IMAGE_FILE_HEADER indique l'architecture cible pour laquelle le fichier a été compilé. Cela peut aider à déterminer la plate-forme pour laquelle le malware est destiné.

2. Nombre de sections : Le champ "NumberOfSections" dans l'IMAGE_FILE_HEADER indique le nombre de sections dans le fichier. Chaque section peut contenir du code, des données, des ressources, etc. L'analyse de ces sections peut aider à comprendre la structure interne du fichier.

3. Entrée point (Entry Point) : Le champ "AddressOfEntryPoint" dans l'IMAGE_OPTIONAL_HEADER indique l'adresse de départ de l'exécution du fichier. Il peut être utile pour comprendre le point d'entrée du malware et les premières instructions exécutées.

4. Adresses de base : Les champs "ImageBase", "BaseOfCode" et "BaseOfData" dans l'IMAGE_OPTIONAL_HEADER indiquent les adresses de base de l'image, du code et des données respectivement. Ces informations sont importantes pour la résolution des adresses lors de l'analyse dynamique du malware.

5. Informations sur les sections : Les champs de l'IMAGE_SECTION_HEADER fournissent des informations sur chaque section, telles que les adresses de début et de fin, les attributs, les noms, etc. L'analyse de ces informations peut aider à identifier les parties du fichier utilisées par le malware, telles que les sections de code malveillant ou les sections de données chiffrées.

## Conclusion

La compréhension du PE HEADER est cruciale dans l'analyse de malwares, car il fournit des informations clés sur le fichier exécutable. En examinant les différents en-têtes et champs du PE HEADER, les analystes peuvent obtenir des indices sur la nature du fichier, son architecture, ses fonctionnalités et bien plus encore. Cette connaissance est essentielle pour comprendre le comportement et les capacités du malware.

N'hésitez pas à explorer davantage les différentes structures et champs du PE HEADER pour approfondir votre analyse de malwares.
