Goals
==

1. be able to simulate a battle after providing deck, [initial hand], enemy + behavior
2. generate optimal strategy w/monte carlo
3. can this be hooked up to alphazero
4. obviously the dream is a complete player of the entire game - simulating every choice fully
	annoying part of this is having to manually enter the behavior patterns of every enemy


Stages
==

0. just dealing with ironclad to start.
1. cards, communication, artifacts, enemy behavior
2. this could get complicated
3. simulation of a fight
4. trying various strategies (try as many draws as possible)
5. search optimization, pruning.  i.e. with no +heal or stateful artifacts, voluntarily not doing damage is probably prunable.
6. charts for likely outcomes of fights with perfect play

Todos
==
* Are enemy attacks/actions best characterized as cards?  Would make it easier in some ways since enemy "turns" could be the same as players.
* unify status parameter - it means either "intensity" for things with fixed lengths like strength (permanent) or flame barrier status(1 round), or length, for things with fixed effects (vulnerable/weaken).
* There is no reason for this to have two parameters; one which gets mapped is enough.
...deck concepts - exhaust, draw, etc.  New test classes just for this
- ability to characterize certain cards like the ones that add wounds, the one that draws & puts a card on top.
...shouldn't card be a subclass of cardinstance?
* fix up text notation so it's playable by hand.
* AI to handle simple fights
* monsters are going to be a lot more painful than I thought.
* cards that have multiple "versions" of sims, i.e. headbutt or any card that has targets, needs to specify targets somehow? so the AI can monte carlo over them.
* fix tests to have standard format and test autodetection.
* console application for testing