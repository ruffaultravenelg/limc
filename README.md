# Lim Compiler

## Introduction
Lim est un langage de programmation compilé orienté objet.

## Fonctionnalités
- Programmation orientée objet.
- Programmation événementielle.
- Support du multithreading.
- Compilation vers le langage C.
- Ramasse miette intégrée.

## Prérequis (pour le code source)
- .NET Framework 8.0 ou version ultérieure pour compiler le projet.

## Utilisation (du compilateur)
1. Téléchargez-le depuis les releases.
2. Ouvrez un terminal.
3. Utilisez la commande `limc <source.lim> [destination] [flags...]`.

## Exemples
### Hello World
```limc
func main
    puts("Hello world")
```

### Définition d'une classe
```limc
class user
    let username:str
    let age:int

    func new(_username:str, _age:int)
        username = _username
        age = _age
```

### Programmation synchronisée
```limc
func foo
    ...

func main
    foo() // La fonction foo sera exécutée, une fois finie, on passe à la ligne suivante
    cook foo() // La fonction foo est appelée sur un nouveau thread, la fonction continue sans attendre que foo finisse.
```

### Utilisation des relations
```limc
class vector
    let x:int
    let y:int

    func new(x:int, y:int)
        me.x = x
        me.y = y
    
    relation +(a:vector, b:vector)
        return new vector(a.x + b.x, a.y + b.y)

func main
    let vec1 = new vector(1, 2)
    let vec2 = new vector(3, 4)
    let vec3 = vec1 + vec2
```

### Utilisation des contrats (interfaces)
```limc
class vehicule
    func avancer()

class bus signs vehicule
    func avancer()

class voiture signs vehicule
    func avancer()

func main
    let v:vehicule = new bus()
    v.avancer() // Appelle le avancer() de bus
```

## Projets utilisés
- [Boehm GC](https://github.com/ivmai/bdwgc)
- [SDL2](https://www.libsdl.org/)
- [MinGW](https://www.mingw-w64.org/)
- [Olive C](https://github.com/tsoding/olive.c/)
- [STB](https://github.com/nothings/stb/)