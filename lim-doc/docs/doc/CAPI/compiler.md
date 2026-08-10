# Architecture et Utilisation

Le compilateur **limc** transforme les fichiers source Lim (`.lim`) en code source C. Ce code est ensuite compilé en exécutable binaire via un compilateur C externe (comme `gcc` ou `clang`).

## Compilation vers C vs Exécutable

Par défaut, `limc` génère un exécutable. Si vous souhaitez uniquement obtenir le code source C intermédiaire, utilisez le drapeau `-s` ou `--source`.

```bash
limc main.lim program.c --source
```

## Configuration du compilateur C

Le compilateur utilise `gcc` par défaut pour lier le code C généré. Vous pouvez spécifier un autre compilateur (par exemple `clang`) en utilisant l'option `-cc` suivie du chemin vers l'exécutable.

```bash
limc main.lim out -cc clang
```

:::info
limc est installé avec `gcc` d'installé par défault.
::::

## Options de ligne de commande

Voici les principales options de developpement du CLI :

| Drapeau | Alias | Description |
| --- | --- | --- |
| `--source` | `-s` | Compile vers un fichier source C au lieu d'un exécutable. |
| `-cc` |  | Spécifie le chemin vers le compilateur C (Défaut: `gcc`). |
| `--verbose` | `-vb` | Ajoute des commentaires détaillés dans le fichier C généré. |

## Compilation Conditionnelle

Lim permet d'inclure ou d'exclure des lignes de code selon la plateforme ou l'architecture cible. Pour ce faire, vous pouvez préfixer n'importe quelle ligne avec une directive de condition `@CONDITION`.

Lors de la phase de prétraitement, `limc` analyse ces drapeaux et ne conserve que les lignes correspondant à l'environnement de compilation détecté.

### Syntaxe

Placez le drapeau au tout début de la ligne concernée :

```go title="platform_specific.lim"
@win $include "<io.h>"          // Inclus uniquement sur Windows
@unix $include "<unistd.h>"     // Inclus uniquement sur les systèmes Unix
```

### Plateformes et Architectures supportées

Le compilateur reconnaît actuellement les identifiants suivants :

| Flag | Description |
| --- | --- |
| **`@win`** | Systèmes d'exploitation Windows |
| **`@unix`** | Systèmes de type Unix (Linux, macOS, FreeBSD) |
| **`@arm`** | Architectures de processeurs ARM |
| **`@x86-64`** | Architectures de processeurs x86 64-bits |

:::tip Conseil
Cette fonctionnalité est particulièrement utile pour gérer les dépendances système divergentes via la directive `$include` ou pour adapter des [blocs de code source](interoperability.md) spécifiques à une architecture.
:::