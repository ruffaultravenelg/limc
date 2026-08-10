# Scope
Le **scope** (ou contexte) désigne la portée d’accessibilité des variables et fonctions dans un programme Lim. Chaque bloc de code (fonction, boucle, condition, etc.) crée un nouveau scope. Une variable déclarée dans un scope est accessible uniquement dans ce scope et dans ses scopes enfants, mais pas dans les scopes parents ou frères.

### Exemple simple

```js
func maFonction
    let x = 10; // x est accessible dans toute la fonction

    while x > 0
        let y = x - 1; // y est accessible uniquement dans la boucle
        puts(y);

    // puts(y); // Erreur : y n’est pas accessible ici
```

**Schéma des scopes :**

```
maFonction (scope de la fonction)
 └── while (scope de la boucle)
```

Dans cet exemple, `x` est accessible dans toute la fonction et dans la boucle, tandis que `y` n’est accessible que dans la boucle. Cela permet d’organiser et de protéger les données selon leur contexte d’utilisation.

### Exemple de scopes imbriqués et parallèles

```js
func test
    let b = 2; // Scope de la fonction

    if b > 0
        let c = 3; // Scope du if
        puts(b, c);

        for i to 2
            let e = i; // Scope de la boucle for dans le if
            puts(b, c, e);

        // puts(e); // Erreur : e n’est pas accessible ici

    else
        let d = 4; // Scope du else
        puts(b, d);

    // puts(c); // Erreur : c n’est pas accessible ici
    // puts(d); // Erreur : d n’est pas accessible ici
```

**Schéma des scopes complexes :**

```
test (fonction)
 ├── if (scope du if)
 │    └── for (scope de la boucle for)
 └── else (scope du else)
```

Dans cet exemple :
- `b` est accessible dans toute la fonction `test`.
- `c` n’est accessible que dans le bloc `if`.
- `d` n’est accessible que dans le bloc `else`.
- `e` n’est accessible que dans la boucle `for` à l’intérieur du `if`.
- Les scopes `if` et `else` sont au même niveau, enfants du scope de la fonction.
- Les variables déclarées dans un bloc ne sont pas accessibles en dehors de ce bloc, même dans la même fonction.
