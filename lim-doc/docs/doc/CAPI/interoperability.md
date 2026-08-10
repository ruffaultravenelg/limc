# Interopérabilité avec le C

Lim offre une interaction directe et puissante avec l'écosystème C. Grâce à des directives de préprocesseur et une syntaxe dédiée, vous pouvez intégrer du code C natif, inclure des bibliothèques externes et manipuler les mécanismes internes du compilateur.

:::info
De manière générale, le symbole `$` signale une interaction directe avec le code C généré.
:::

## Directives de Préprocesseur

### Inclusion de headers (`$include`)

La directive `$include` permet d'insérer une clause `#include` directement au sommet du fichier C généré. C'est l'outil indispensable pour accéder à la bibliothèque standard (`stdio.h`, `math.h`, etc.) ou à des bibliothèques tierces.

```go
$include "<math.h>"
$include "my_custom_lib.h"
```

### Options de compilation (`$comopt`)

La directive `$comopt` (*Compiler Option*) permet de transmettre des arguments spécifiques au compilateur final (`gcc` ou `clang`). Cela est particulièrement utile pour l'édition de liens (linking).

```go
// Exemple : Liaison avec la bibliothèque mathématique
$comopt "-lm"
```

## Insertion de code C natif

Vous pouvez injecter du code C directement à l'intérieur de vos fonctions. Toute ligne commençant par le symbole `$` est traitée comme du code source C brut et insérée telle quelle à l'emplacement correspondant dans le fichier généré.

```go
func main
    $ for (int i = 0; i < 5; i++) {
    $     printf("[%d] Hello from C!\n", i);
    $ }
```

:::warning Attention
Le compilateur `limc` ne vérifie pas la syntaxe C à l'intérieur des lignes `$`. Assurez-vous de ne pas oublier les points-virgules (`;`) et de respecter la syntaxe du langage C.
:::

## Accès aux éléments Lim depuis le C

L'une des forces de `limc` est la capacité de référencer des variables Lim directement dans vos blocs de code C.

### Variables (`$variableName`)

Le compilateur `limc` renomme souvent les variables pour éviter les conflits ou gérer les portées. Pour accéder au nom réel d'une variable dans le code C généré, utilisez la syntaxe `$nomDeLaVariable`.

**Exemple en Lim :**

```go
func main
    let maVariable = 42
    $ printf("Valeur : %d\n", $maVariable);
```

**Résultat compilé (conceptuel) :**

```c
void main_f(Ctx_t _c) {
    Ctx_t c = { _c, "main", _c.gc, NULL };
    int v_0 = 42; // 'maVariable' a été renommée en 'v_0'
    printf("Valeur : %d\n", v_0); // $maVariable est correctement remplacé
}
```

### Constantes internes (`@CONSTANT_NAME`)

`limc` utilise un ensemble de constantes pour structurer le code généré (gestion du contexte, allocation, etc.). Vous pouvez y accéder via la syntaxe `@NOM_CONSTANTE`.

| Nom | Description | Valeur |
| --- | --- | --- |
| `@RUNTIME_CONTEXT_STRUCT_NAME` | Nom de la structure contenant le contexte d'appel d'une fonction | `CT_t` |
| `@RUNTIME_CONTEXT_VARIABLE_NAME` | Nom de la variable contenant le contexte dans une fonction | `c` |
| `@PANIC_FUNCTION_NAME` | Nom de la fonction de panique (crash) | `lim_panic` |
| `@PRINT_STACK_TRACE_FUNCTION_NAME` | Nom de la fonction permettant de print la stack trace | `lim_printStackTrace` |
| `@LIM_ALLOC` | Nom de la fonction d'allocation dynamique de mémoire (garbage collected) | `LIM_ALLOC` |
| `@LIM_FREE` | Nom de la fonction de libération de mémoire (le block libéré avec cette fonction a du être alloué avec `LIM_ALLOC`) | `LIM_FREE` |
| `@LIM_REALLOC` | Realloc dynamique toujours avec garbage collector | `LIM_REALLOC` |
| `@INT_TO_STR_BUFFERSIZE` | Taille du bufffer de la convertion d'un entier a une chaine de caractère | `12` |
| `@FLOAT_TO_STR_BUFFERSIZE` | Taille du buffer de la convertion d'un float vers une chaine de caractère | `12` |
| `@GETS_BUFFER_SIZE` | Taille du buffer de l'input `gets()` | `100` |
| `@INSTANCE_ARGUMENT_NAME` | Nom du paramètre d'instance pour les méthodes | `instance` |
| `@SELF` | Alias d'`INSTANCE_ARGUMENT_NAME` | `instance` |

## Attributs natifs (C-Field)

Dans une classe ou une structure, vous pouvez définir des champs qui n'existent qu'en C. Ces champs seront ajoutés à la structure C résultante, mais seront ignorés par le système de type de Lim (ils ne sont accessibles que via des lignes de code `$`).

```go
class FileReader
    $ FILE *handle; // Attribut purement C

    new(filepath: str)
        $ $handle = fopen($filepath, "r");
```