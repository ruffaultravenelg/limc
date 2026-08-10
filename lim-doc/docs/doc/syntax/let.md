# Déclaration de variables

## Qu'est-ce qu'une variable ?

Une variable est un espace nommé en mémoire permettant de stocker une valeur qui peut être utilisée et modifiée au cours de l'exécution d'un programme.

## Syntaxe

- `let|const <nom_variable> = <valeur>`  
    Déclare une variable/constante avec une valeur initiale. Le type est alors déduit automatiquement de la valeur.

- `let|const <nom_variable>:<type> = <valeur>`  
    Déclare une variable/constante en spécifiant explicitement son type et sa valeur initiale.

`let` déclare une variable mutable.

`const` déclare une constante.

```js
let name = "Bob"
name = "John" // Fonctionne

const age = 18
age = 17 // Provoque une erreur
```

## Utilisations

### Dans un bloc de code

L'utilisation de `let` dans un bloc de code permet de définir une nouvelle variable accessible dans ce scope et dans ses sous-scopes. Il est possible de redéclarer une variable avec le même nom dans un scope enfant : dans ce cas, la variable la plus proche du scope courant sera utilisée.

### Dans une classe ou une structure

`let` permet de définir des propriétés d'une classe ou d'une structure.

Il est possible d'ajouter le mot-clé `loc` (local) pour restreindre l'accès à une propriété, la rendant privée et accessible uniquement à l'intérieur de la classe ou de la structure.

```lim
class User
        let username:str = ""
        let age = 18
        let loc password = ""
```

Dans l'exemple ci-dessus :
- `username` est une propriété publique de type `str` avec une chaine vide comme valeur par défault.
- `age` est une propriété publique initialisée à 18, son type est inféré (`int`).
- `password` est une propriété privée grâce au mot-clé `loc`, ça valeur par défault est aussi une chaine vide.