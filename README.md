# limc - Lim Compiler

## Introduction
`limc` est un compilateur pour le langage [Lim](#lim). Il prend des fichiers source `.lim` et les convertit exécutables.

## Utilisation
```bash
limc <source> <destination> [options...]
```

- `source` : Chemin vers le fichier `.lim` à compiler. Ce fichier doit contenir une fonction `main`.
- `destination` : Chemin vers l'exécutable à créer. Le fichier sera écrasé s'il existe déjà.

## Intentions
Lim a été avant tout conçu comme un projet ludique, sans la prétention de rivaliser avec quelconque autre langages, ou de réinventer la roue. Cependant, il essaie d'explorer une approche différente de la programmation orientée objet, en privilégiant la composition plutôt que l'héritage, notamment dans sa syntaxe.

## Caractéristiques techniques
- Lim compile d'abord tout le code source en un fichier C, puis utilise `gcc` pour le transformer en exécutable. Cela permet d'accéder à l'ensemble des bibliothèques C. De plus, il est possible d'injecter directement du code C dans le code Lim.
- Le type d'allocation mémoire est déterminé par la classe : une classe déclarée avec `class` crée des objets sur le tas (heap), tandis qu'une classe `primitive class` alloue ses objets sur la pile (stack).
- Là où Java prône "Write once, run anywhere", Lim prône le "Write once, compile anywhere".

## À faire
- [X] Classes primitives
- [X] Fonctions
- [X] Système d'import/export
- [X] Types génériques
- [X] Classes heap
- [X] Ajout direct de sources
- [X] Collecteur de déchets (Garbage Collector)
- [ ] Relations
- [ ] Accesseurs (getters & setters)
- [ ] Chaînes de caractères
- [ ] Énumérations
- [ ] Contrats
- [ ] Extensions
- [ ] Multithreading

## Lim
Lim est un langage compilé, orienté objet et fortement typé, inspiré de la syntaxe de Python.

### Exemple : Hello World
```go
func main
    puts("Hello World")
```

### Fonctions
```go
func main(args:array<str>)
    for arg in args
        if not isFlag(arg)
            puts(arg)

func isFlag(arg:str)
    return arg[0] == '-'
```

### Types génériques et accesseurs
```go
class stack<T>
    let content:list<T>

    func new
        content = new list<T>
    
    func push(elm:T)
        content.add(elm)

    func pop:T
        let elm = content[-1]
        content.remove(-1)
        return elm
    
    get len
        return content.len
```

### Contrats
```go
import image

class rectangle implements drawable
    let x:int
    let y:int
    let w:int
    let h:int

    func draw(canvas:image)
        canvas.drawRect(x, y, w, h, "#00FF00".hex())

class circle implements drawable
    let x:int
    let y:int
    let r:int

    func draw(canvas:image)
        canvas.drawEllipse(x, y, r, r, "#0000FF".hex())

contract drawable
    func draw(img:image)
```

### Extensions
Lim ne permet pas l'héritage, mais il est possible d'étendre une classe avec `extend` :
```go
extend int
    let counter

    func increment
        counter += 1
```

Il est également possible d'étendre toutes les classes qui implémentent un contrat :
```go
extend drawable
    let c:color
```
Ici, la propriété `c:color` sera ajoutée aux classes `rectangle` et `circle`.

## Liens
- [TGC](https://github.com/orangeduck/tgc) est un collecteur de déchets (garbage collector) de type mark-and-sweep qui m'a grandement aidé lors du développement des prototypes.