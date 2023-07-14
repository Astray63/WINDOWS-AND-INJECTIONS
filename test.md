# Introduction

Le format PE, est un format de fichier propre aux binaires Windows (c'est-à-dire exécutables, DLL, drivers) permettant à notre cher Windows de charger et lancer ces derniers. Ces structures contiennent des informations plus ou moins utiles, comme la table des sections, l'adresse du point d'entrée de l'exécutable ou encore l'adresse du dealer de chocapicz le plus proche de chez vous.

Pour bien profiter de cette visite guidée, il est recommandé d'avoir des notions en C (utilisation massive de pointeurs/structures au programme), et être un minimum à l'aise avec la notation hexadécimale. Il est aussi recommandé d'avoir un éditeur hexadécimal et une calculatrice (celle de Windows fera l'affaire) pour réaliser quelques bidouillages.

## Préparatifs : les offsets, RVA et autres (D)WORDs

Il va nous falloir ici différentier deux mondes totalement différents : le monde des fichiers, et celui de la mémoire vive. Un exécutable est un fichier de ce qu'il y a de plus banal (au même titre qu'un fichier texte ou une image jpg), mais qui a pour but ultime d'être chargé en mémoire par le système d'exploitation (en l'occurrence Windows, ou ReactOS si vous êtes un libriste convaincu aimant le goût du risque ;) ). Cet exécutable chargé en mémoire subira différentes transformations, le rendant ainsi complètement différent du fichier dont il est issu (modifiant les adresses mémoires par le fait).

C'est pour ça que les ingénieurs de Microsoft ont introduit un système intelligent de découpage en sections et de RVA (Relative Virtual Address) qui sert de base à la plupart des adresses qu'on rencontrera, plutôt que de baser les adresses sur les offsets du fichier.

Un offset est en quelque sorte la position d'un élément dans le fichier. Par exemple, si je veux lire le caractère à l'offset 4, je lirai le 5ème caractère du fichier (étant donné que la numérotation des offsets commence à partir de 0 au lieu de 1). À noter ici qu'on préférera utiliser une notation hexadécimale pour les offsets (ainsi que pour les adresses mémoire et les RVA) par convention, même s'il est possible d'utiliser une notation décimale (ainsi le caractère à l'offset 10 ou 0x0A est le 11ème caractère du fichier, car 0x0A en hexadécimal correspond à 10 en décimal).

Les RVA, elles sont plus subtiles, puisqu'elles dépendent d'une entité appelée "table de sections" pour exister. En effet, un exécutable range son code dans des "sections" qui seront chargées en mémoire selon les informations contenues dans la table des sections (sur laquelle je reviendrai plus en détail sur la 2ème partie du tuto). La RVA représente l'adresse en mémoire d'une entité, relativement à l'adresse de base, c'est-à-dire par rapport à l'adresse où est chargé l'exécutable par le système d'exploitation (souvent 0x00400000).

Enfin, on trouvera souvent des noms du type "WORD", "DWORD" ou "BYTE" utilisés dans nos structures. Il s'agit juste de variables ayant une certaine longueur d'octets. Un DWORD pour Double Word est une variable qui prend 4 octets consécutifs en mémoire (en little-endian, c'est-à-dire que les octets successifs dans la RAM 0x01 0x02 0x03 0x04 donneront 0x04030201 représentés par le DWORD). Le WORD lui, est une variable qui prend 2 octets consécutifs en mémoire (toujours en little-endian), et le BYTE, un seul octet.

## Début de la randonnée : le MZ Header

Ouvrons notre exécutable avec notre éditeur hexadécimal, et admirons la jolie bouille d'octets servie pour notre déjeuner ! Vous reconnaissez sûrement le célèbre "This program can't run in DOS mode" si vous vous êtes déjà amusés à ouvrir un exécutable dans le bloc-notes lorsque vous étiez petits. Les premiers octets correspondent ici au MZ Header, ou encore l'en-tête MS-DOS (et oui, un exécutable Windows n'est rien d'autre qu'un exécutable MS-DOS avec des fonctionnalités supplémentaires).

Voici donc la structure représentant le MZ header :

```c
typedef struct _IMAGE_DOS_HEADER {
    WORD e_magic; /* 00: MZ Header signature */
    WORD e_cblp; /* 02: Bytes on last page of file */
    WORD e_cp; /* 04: Pages in file */
    WORD e_crlc; /* 06: Relocations */
    WORD e_cparhdr; /* 08: Size of header in paragraphs */
    WORD e_minalloc; /* 0a: Minimum extra paragraphs needed */
    WORD e_maxalloc; /* 0c: Maximum extra paragraphs needed */
    WORD e_ss; /* 0e: Initial (relative) SS value */
    WORD e_sp; /* 10: Initial SP value */
    WORD e_csum; /* 12: Checksum */
    WORD e_ip; /* 14: Initial IP value */
    WORD e_cs; /* 16: Initial (relative) CS value */
    WORD e_lfarlc; /* 18: File address of relocation table */
    WORD e_ovno; /* 1a: Overlay number */
    WORD e_res[4]; /* 1c: Reserved words */
    WORD e_oemid; /* 24: OEM identifier (for e_oeminfo) */
    WORD e_oeminfo; /* 26: OEM information; e_oemid specific */
    WORD e_res2[10]; /* 28: Reserved words */
    DWORD e_lfanew; /* 3c: Offset to extended header */
} IMAGE_DOS_HEADER, *PIMAGE_DOS_HEADER;


## Le PE Header (suite)

L'Optional Header contient de nombreuses informations, telles que l'emplacement de l'image de l'exécutable en mémoire, la taille du code, la taille des données, la RVA du point d'entrée, et bien d'autres informations intéressantes. L'Optional Header est représenté par la structure suivante :

```c
typedef struct _IMAGE_OPTIONAL_HEADER {
    /* Champs standard */
    WORD Magic; /* pe_base + 0x18 */
    BYTE MajorLinkerVersion; /* pe_base + 0x1A */
    BYTE MinorLinkerVersion; /* pe_base + 0x1B */
    DWORD SizeOfCode; /* pe_base + 0x1C */
    DWORD SizeOfInitializedData; /* pe_base + 0x20 */
    DWORD SizeOfUninitializedData; /* pe_base + 0x24 */
    DWORD AddressOfEntryPoint; /* pe_base + 0x28 */
    DWORD BaseOfCode; /* pe_base + 0x2C */
    DWORD BaseOfData; /* pe_base + 0x30 */

    /* Champs supplémentaires NT */
    DWORD ImageBase; /* pe_base + 0x34 */
    DWORD SectionAlignment;
    DWORD FileAlignment; /* pe_base + 0x3C */
    WORD MajorOperatingSystemVersion; /* pe_base + 0x40 */
    WORD MinorOperatingSystemVersion; /* pe_base + 0x42 */
    WORD MajorImageVersion; /* pe_base + 0x44 */
    WORD MinorImageVersion; /* pe_base + 0x46 */
    WORD MajorSubsystemVersion; /* pe_base + 0x48 */
    WORD MinorSubsystemVersion; /* pe_base + 0x4A */
    DWORD Win32VersionValue; /* pe_base + 0x4C */
    DWORD SizeOfImage; /* pe_base + 0x50 */
    DWORD SizeOfHeaders; /* pe_base + 0x54 */
    DWORD CheckSum; /* pe_base + 0x58 */
    WORD Subsystem; /* pe_base + 0x5C */
    WORD DllCharacteristics; /* pe_base + 0x5E */
    DWORD SizeOfStackReserve; /* pe_base + 0x60 */
    DWORD SizeOfStackCommit; /* pe_base + 0x64 */
    DWORD SizeOfHeapReserve; /* pe_base + 0x68 */
    DWORD SizeOfHeapCommit; /* pe_base + 0x6C */
    DWORD LoaderFlags; /* pe_base + 0x70 */
    DWORD NumberOfRvaAndSizes; /* pe_base + 0x74 */
    IMAGE_DATA_DIRECTORY DataDirectory[16]; /* pe_base + 0x78 */
} IMAGE_OPTIONAL_HEADER32, *PIMAGE_OPTIONAL_HEADER32;
