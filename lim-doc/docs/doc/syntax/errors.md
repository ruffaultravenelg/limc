# Gestion des erreurs
Lim ne possède pas de système permettant de faire remonter une erreur a travers le stack d'appel contrairement a de nombreux langages de programation. En effet, en combinaison avec le ramasse miette, c'est un système qui peut se revelé lourd sur le temps d'execution. De plus, la plus par des exceptions que nous levons, ne sont en fait jamais récupéré, si ce n'ai au début de notre programme pour ne pas les afficher a l'utilisateur final.
Il convient ainsi d'essayer un système différent. Lim possède bien un système de gestion d'erreur, mais il se base sur un concept simple : celui qui appel gère l'erreur, ou permet le crash.

## Définir un problème
```lim
problem userDoesNotExist "L'utilisateur n'existe pas"
```

## Lever un problème
N'importe quel fonction peut lever un problème, tant que celui-ci est accessible.
```
func getUser(userID:int):User
    let res = db.select("SELECT * FROM USERS WHERE id = ? LIMIT 1;", userID);
    
    if res.len = 0
        raise userDoesNotExist
    return res[0]
```

## Gérer une erreur
Comme énoncé précédement, c'est la fonction parente qui a la responsabilité de gérer les problèmes. Si rien n'est fait le système crashera, indiquant le message d'erreur.


### else
```
call_problem() else default_value
```

La syntaxe `else` est une expression qui permet de définir une valeur par défault qui sera utiliser si l'appel vient a lever un problème.

## Crasher proprement
Si une erreur nécéssitant l'arrêt complet du programme advient, la commande `panic` permet de stopper proprement l'execution.

```lim
func main
    puts("you see me")
    panic "Erreur !"
    puts("now you don't")
```

sortie :
```
you see me

=== FATAL RUNTIME ERROR ===
Erreur !

---- Stacktrace ----
main <- here 'example.lim (l.1)'
```