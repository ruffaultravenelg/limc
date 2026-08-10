---
slug: std
sidebar_position: 1
description: std
---

# Standard library
La bibliothèque `std` contiens tous les types et fonctions de base du language Lim. Elle est inclue automatiquement dans tous les fichiers sources. Ainsi il n'est pas utile de l'importer manuellement.
| Symbole | Type d'élément |
|---|---|
| 𝒇 | Fonction |
| 𝒮 | Structure |
| 𝒞 | Classe |
| 𝓂 | Méthode |
| 𝓟 | Propriété |


## 𝒇 : puts
La fonction `puts`, pour "put string", permet d'écrire une nouvelle ligne dans le standard output. Elle est utile pour afficher du texte dans la console.

#### Syntaxe
```c
puts(message:str)
```

- `message`
    - **Type** : [`str`](#str)
    - Description : Le message à écrire dans le standard output.

*Ne retourne pas de valeur.*

#### Exemples
```c
// Exemple simple
puts('Hello world')

// Exemple avec une variable
let name = 'Alice'
puts('Bonjour ' + name)

// Exemple avec une chaîne multi-lignes
puts('Ligne 1\nLigne 2\nLigne 3')
```

---

## 𝒇 : puti
La fonction `puti`, pour "put   eger", permet d'écrire un entier dans une nouvelle ligne dans le standard output. Elle est utile pour afficher un nombre dans la console.

#### Syntaxe
```c
puti(value:int)
```

- `value`
    - **Type** : [`int`](#int)
    - Description : Le nombre à écrire dans le standard output.

*Ne retourne pas de valeur.*

#### Exemples
```c
puti(3)
puti(6 + 53)
```

:::note
Lim ne convertie pas automatiquement les différents types de nombres entre eux. Ainsi cette fonction ne peut pas prendre de nomber a virgule.
:::

---

## 𝒇 : gets
La fonction `gets`, pour "get string", permet de lire une ligne de texte depuis le standard input. Elle est utile pour récupérer une entrée utilisateur sous forme de chaîne de caractères.

#### Syntaxe
```c
gets():str
gets(message:str):str
```

- `message`
    - **Type** : [`str`](#str)
    - Description : Message a écrire dans la console avant de demander une valeur.

*Retourne une valeur de type [`str`](#str).*

#### Exemples
```c
// Exemple simple avec un message
let input = gets('Veuillez entrée un chiffre :')
puts('Vous avez entré : ' + input)

// Sans message
puts('Vous avez entré : ' + gets())
```

---

## 𝒇 : geti
La fonction `geti`, pour "get integer", permet de lire un entier depuis le standard input. Elle est utile pour récupérer une entrée utilisateur sous forme de nombre entier.

#### Syntaxe
```c
geti():int
geti(message:str):int
```

- `message`
    - **Type** : [`str`](#str)
    - Description : Message a écrire dans la console avant de demander une valeur.

*Retourne une valeur de type [`int`](#int).*

#### Exemples
```c
// Exemple simple
let number = geti()
puti(number * 2)
```

:::caution
Si l'entrée utilisateur n'est pas un entier valide, une erreur sera levée.
:::

---

## 𝒮 : int
La structure `int` représente un entier non signé 32 bits en mémoire.

:::caution
Cette structure n'a pas de constructeur, chaque nombre dans le code est une instance de cette structure. Ainsi `5` est de type `int`.
:::

### 𝓂 : str
Renvoie la chaine de caractère correspondant au nombre.

```js
let age = 19
let message = 'Tu a ' + age.str() + ' ans'
```

### 𝓂 : float
Renvoie le nombre flottant correspondant a l'entier. `5` deviens `5.0`

