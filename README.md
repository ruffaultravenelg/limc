# limc - Lim Compiler

## Introduction
limc est un compilateur pour le langage [Lim](#lim). Il prend des fichiers source `.lim` et les convertit en exécutables.

## Utilisation
```sh
limc <source> [destination] [flags...]
```

- `<source>` : Obligatoire, Chemin du fichier `.lim` à compiler. Ce fichier doit contenir une fonction `main`.
- `[destination]` : Optionnel, chemin de l'exécutable à créer. Le fichier sera écrasé s'il existe déjà.

## Lim
Lim est un langage compilé, orienté objet et fortement typé, inspiré de la syntaxe de Python.

### Intentions
Lim a été avant tout conçu comme un projet ludique, sans la prétention de rivaliser avec quelque autre langage ou de réinventer la roue. J'ai pour but de créer un langage qui soit, pour moi, agréable à utiliser. Dans son intention, Lim veut prendre la simplicité et la lisibilité de Python, avec le côté compilé du C.

### Caractéristiques techniques
- Lim compile d'abord tout le code source en un fichier C, puis utilise `gcc` pour le transformer en exécutable. Cela permet d'accéder à l'ensemble des bibliothèques C. De plus, il est possible d'injecter directement du code C dans le code Lim.
- Le type d'allocation mémoire est déterminé par le type : les `enum` ou les `struct` seront sur la pile (stack) tandis que les `class` seront instanciées dans le tas (heap).
- Là où Java prône "Write once, run anywhere", Lim prône le "Write once, compile anywhere".

### TODO list
- [X] Fonctions
- [X] Structures
- [X] Ajout direct de sources
- [X] Classes
- [ ] Collecteur de déchets (Garbage Collector, pour l'instant assuré par [tgc](https://github.com/orangeduck/tgc) mais sera à terme remplacé par une implémentation propre)
- [X] Système d'import/export
- [X] Types génériques
- [ ] Relations
- [ ] Accesseurs (getters & setters)
- [ ] Enums
- [ ] Enumerateurs
- [ ] Contrats
- [ ] Extensions
- [ ] Multithreading

### Exemples

#### Exemple : Hello World
```go
func main
    puts("Hello World")
```

#### Fonctions
```go
func main(args:array<str>)
    for arg in args
        if not isFlag(arg)
            puts(arg)

func isFlag(arg:str)
    return arg[0] == '-'
```

#### Types génériques et accesseurs
```go
class stack<T>
    let content:list<T>

    func new
        content = new list<T>
    
    func push(elm:T)
        content.add(elm)

    func pop:T
        let elm = content[-1] // index -1 means last element
        content.pop(-1)
        return elm
    
    get len // Create a .len acessor on a stack<T> object
        return content.len
```

#### Contrats
Or interfaces as we call them in other languages, but I don't really care, I like this name.
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

#### Extensions
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
- [TGC](https://github.com/orangeduck/tgc) est un ramasse-miettes (garbage collector) de type mark-and-sweep qui me sert temporairement pendant le développement pour réduire la complexité de la tâche...