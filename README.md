# Proiect ASC - C#
## Cum se foloseste
- Se adauga un fisier **exemplu.txt** ce contine cod Risc-V in folderul 'Codes/' din interiorul proiectului.

- Se modifica variabila **Code** din clasa **MainClass** cu numele fisierul ce va fi asamblat.
```Cs 
public static string Code = "exemplu";
```
- Se ruleaza proiectul in visual studio sau vscode cu extensia c# si se alege o optiune dintre *Assemble*, *Execute* sau *Both*.

- **In folderul 'Codes/' au fost deja adaugate exemplele de programe din cerinta proiectului.**

## Detalii de functionare
- In urma asamblarii, va fi creat un fisier binar in folderul 'Binaries/', avand acelasi nume ca fisierul text de la inceput.
- In urma executarii va fi creat un fisier 'status.txt' ce contine valorile finale ale fiecarui registru.
- Exista in interiorul proiectului un fisier 'Instructions' ce contine numele fiecarei instructiuni implementata si codificarea acesteia (al 3-lea parametrul este spatiul in biti alocat)
  - Se poate observa ca au fost definite instructiunile abstracte *label*, *var*, *eof*, pentru identificarea mai simpla a unui label, variabila sau a sfarsitului codului.

## Detalii despre binar
- In timpul asamblarii, se creeaza un o coada de booluri (```Queue<bool> bits```) ce va retine bitii care vor fi adaugati la final in binar.
  - La inceput, va fi adaugata adresa sectiunii .text (pentru parcurgerea mai eficienta a binarului de catre executor).
  - Dupa, se adauga metadata pentru fiecare variabila. Mai exact, id-ul variabilei urmat de adresa la care incepe aceasta.
  - Urmatoarea secventa de biti reprezinta valorile propriu-zise ale variabilelor, fara alte informatii intre ele, pentru simularea exacta a unei structuri de date risc-v
  - Urmeaza date din sectiunea .text, codurile instructiunilor urmate de parametrii acestora
  - La final sunt adaugati 5000 biti ce vor functiona pe post de stiva programului

## Particularitati tehnice: 
  - Functiile **printf** si **scanf** sunt importate din C (P/Invoke):
```Cs
[DllImport("msvcr120.dll")]
public static extern int printf(string format, __arglist);
[DllImport("msvcr120.dll")]
public static extern int scanf(string format, __arglist);
```
  - Implementarea functiei printf  foloseste *doar* registrii intregi *a1, ...*.
  - Pentru afisarea valorilor reale din *fa0, fa1, ...*, a fost creeata o functie fprintf (adresa stringului de inceput se memoreaza tot in a0!)
  - Similar, scanf poate citi doar valori intregi.
