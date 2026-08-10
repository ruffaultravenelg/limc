---
slug: first_script
sidebar_position: 2
---

# Premier script

Contrairement à d'autres langages, Lim ne nécessite pas de "créer un projet". Un simple fichier de code dans un dossier créé manuellement suffit.

## Création du dossier de travail

Commençons par créer un dossier dans lequel nous allons travailler :
```sh
mkdir tuto
cd tuto
```

## Écriture du premier script

Créons un premier script qui affichera le message "Hello world" dans la console. Pour cela, créez un fichier nommé `hello_world.lim` contenant le code suivant :
```go title="hello_world.lim"
func main
    puts('Hello world')
```

### Explications du code

1. **Déclaration de la fonction `main`** :  
   La fonction `main` est spéciale, car elle est automatiquement appelée lorsque le programme est exécuté.

2. **Appel de la fonction `puts`** :  
   À l'intérieur de `main`, nous utilisons la fonction `puts` pour afficher une chaîne de caractères dans la console.  
   - La chaîne de caractères `'Hello world'` est passée en argument à `puts`, c'est elle qui sera affichée.
   - La fonction `puts` fait partie de la bibliothèque standard `std.lim`, qui est importée par défaut dans chaque fichier Lim.

### Compilation du script

Pour transformer notre fichier en un exécutable, utilisez la commande suivante :
```sh
limc hello_world.lim hello_world
```

Cela génère un fichier exécutable nommé `hello_world`. Vous pouvez ensuite l'exécuter avec :
```sh
./hello_world
```

Le message `Hello world` devrait s'afficher dans la console.

## Résumé

Voici un exemple complet de compilation :
```sh
C:\Users\exemple>limc hello_world.lim hello_world
C:\Users\exemple>./hello_world
Hello world

C:\Users\exemple>
```