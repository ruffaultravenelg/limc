# Enums

## Enum classique
Dans un programme il arrive souvent qu'il soit nécessaire de stoquer des etats, des types, des informations qui corresponde a un élement dans une liste d'élément possible.

Un enum permet de définir explicitement cette liste d'élement possible, ce qui permet de leur donner un nom clair et compréhensible. De plus, cela permet de définir un type spécifique, permettant d'ajouter une couche de sécurité (le compiler saura vous avertir si une valeur est impossible).

Exemple :
```
enum paiementStatus
    en_attente
    en_cours
    valide
```

Celui ci peut donc être utilisé de la manière suivante :
```
func is_waiting(status:paiementStatus):bool
    return status = paiementStatus!en_attente or status = paiementStatus!en_cours
```

## Enum a valeur
On peut également associer une valeur a chaque option :
```lim
enum forme
    carre:carre
    rectangle:rect

let maForme = forme!carre(new Carre(10))
maForme = forme!rectangle(new Rect(10, 20)) // On modifie "maForme" pour qu'il contienne maintenant un rectangle

// Calculons l'air de "maForme"
let air = maForme where
    carre(c) -> c.width * c.width
    rectangle(r) -> r.width * r.height
```

L'interet est de pouvoir passer plusieurs object différent dans une même variable. Il est aussi possible d'utiliser les types génériques.

Enfin nous pouvons mixer une option simple et une option avec une valeur.

```lim
enum result<T>
    ok:T
    err

func myFunction:result<int>
    if randbool()
        return result!ok(53)
    else
        return result!err
```