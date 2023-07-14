# Introduction

Le format PE est un format de fichier spécifique aux binaires Windows tels que les exécutables, les DLL et les pilotes. Il permet à Windows de charger et d'exécuter ces fichiers. Les structures PE contiennent des informations importantes telles que la table des sections, l'adresse du point d'entrée de l'exécutable, et d'autres détails.

Pour profiter pleinement de cette visite guidée, il est recommandé d'avoir des notions en C (notamment l'utilisation de pointeurs et de structures) et d'être à l'aise avec la notation hexadécimale. Vous aurez également besoin d'un éditeur hexadécimal et d'une calculatrice (celle de Windows fera l'affaire) pour effectuer quelques manipulations.

## Préparatifs : les offsets, RVA et autres (D)WORDs

Il est important de distinguer deux mondes distincts : celui des fichiers et celui de la mémoire vive. Un fichier exécutable est un fichier ordinaire, similaire à un fichier texte ou une image jpg, mais son objectif ultime est d'être chargé en mémoire par le système d'exploitation (Windows, dans notre cas). Lors du chargement en mémoire, l'exécutable subit des transformations qui le rendent complètement différent du fichier d'origine, notamment en modifiant les adresses mémoires.

C'est pourquoi les ingénieurs de Microsoft ont introduit un système ingénieux basé sur les sections et les RVA (Relative Virtual Address), qui servent de base pour la plupart des adresses que nous rencontrerons. Ainsi, au lieu de se baser sur les offsets du fichier, les adresses sont déterminées par rapport à l'adresse de base à laquelle l'exécutable est chargé en mémoire (généralement 0x00400000).

Un offset représente la position d'un élément dans le fichier. Par exemple, si nous voulons accéder au caractère à l'offset 4, nous lirons en réalité le cinquième caractère du fichier (comptant à partir de 0). Il est préférable d'utiliser la notation hexadécimale pour les offsets, les adresses mémoires et les RVA, bien que la notation décimale soit également possible.

Les RVA sont plus subtiles, car elles dépendent de la "table des sections" pour exister. Un exécutable divise son code en sections qui sont chargées en mémoire en fonction des informations contenues dans la table des sections (nous y reviendrons plus en détail dans la deuxième partie du tutoriel). La RVA représente l'adresse en mémoire d'une entité par rapport à l'adresse de base à laquelle l'exécutable est chargé.

Enfin, nous rencontrerons souvent des termes tels que "WORD", "DWORD" ou "BYTE" dans nos structures. Ce sont simplement des variables ayant une certaine longueur en octets. Par exemple, un DWORD (Double Word) occupe 4 octets consécutifs en mémoire (en little-endian), ce qui signifie que les octets successifs en mémoire 0x01 0x02 0x03 0x04 donneront 0x04030201 en représentation DWORD. Un WORD correspond à 2 octets consécutifs en mémoire (toujours en little-endian), et un BYTE correspond à un octet.

## Début de la randonnée : le MZ Header

Ouvrons notre exécutable avec un éditeur hexadécimal et admirons les octets qui composent son en-tête MZ (MS-DOS Header). Vous reconnaîtrez probablement la célèbre phrase "This program can't run in DOS mode" si vous avez déjà essayé d'ouvrir un exécutable dans le Bloc-notes lorsque vous étiez enfant. Les premiers octets correspondent à l'en-tête MZ, qui est en réalité un exécutable MS-DOS avec des fonctionnalités supplémentaires pour les exécutables Windows.

Voici la structure représentant l'en-tête MZ :

```c
typedef struct _IMAGE_DOS_HEADER {
    WORD e_magic; /* 00: Signature de l'en-tête MZ */
    WORD e_cblp; /* 02: Nombre d'octets dans la dernière page du fichier */
    WORD e_cp; /* 04: Nombre de pages du fichier */
    WORD e_crlc; /* 06: Relocalisations */
    WORD e_cparhdr; /* 08: Taille de l'en-tête en paragraphes */
    WORD e_minalloc; /* 0a: Nombre minimum de paragraphes supplémentaires nécessaires */
    WORD e_maxalloc; /* 0c: Nombre maximum de paragraphes supplémentaires nécessaires */
    WORD e_ss; /* 0e: Valeur SS initiale (relative) */
    WORD e_sp; /* 10: Valeur SP initiale */
    WORD e_csum; /* 12: Somme de contrôle */
    WORD e_ip; /* 14: Valeur IP initiale */
    WORD e_cs; /* 16: Valeur CS initiale (relative) */
    WORD e_lfarlc; /* 18: Adresse du tableau de relocalisation */
    WORD e_ovno; /* 1a: Numéro de superposition */
    WORD e_res[4]; /* 1c: Mots réservés */
    WORD e_oemid; /* 24: Identifiant OEM (pour e_oeminfo) */
    WORD e_oeminfo; /* 26: Informations OEM ; spécifiques à e_oemid */
    WORD e_res2[10]; /* 28: Mots réservés */
    DWORD e_lfanew; /* 3c: Offset du PE header étendu */
} IMAGE_DOS_HEADER, *PIMAGE_DOS_HEADER;
