---
slug: random
description: random.lim
---

# Random
La bibliothèque `random` contiens tous ce qui touche a la gestion de l'aléatoire.

| Symbole | Type d'élément |
|---|---|
| 𝒇 | Fonction |
| 𝒮 | Structure |
| 𝒞 | Classe |
| 𝓂 | Méthode |
| 𝓟 | Propriété |

## 𝒇 : randbool()
La fonction `randbool`, pour "random boolean", permet de retourner une valeur booléenne aléatoire (true ou false).

#### Syntaxe
```c
randbool:bool
```

*Retourne une valeur de type [`bool`](std.md#bool).*

#### Exemples
```js
let r = randbool()
if r
    puts("true !")
else
    puts("false !")
```

---
## 𝒇 : randint()
La fonction `randint`, pour "random integer", permet de retourner une valeur entière aléatoire inclue dans une intervalle donnée.

#### Syntaxe
```c
randint(min:int, max:int):int
```

- `min`
    - **Type** : [`int`](std.md#int)
    - Description : Valeur minimum incluse.

- `max`
    - **Type** : [`int`](std.md#int)
    - Description : Valeur maximum incluse.

*Retourne une valeur de type [`int`](std.md#int).*

#### Exemples
```js
let aleatoire = randint(5, 10) // "aleatoire" contiendera une valeur entre [5; 10]
```

---