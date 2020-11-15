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
Are enemy attacks/actions best characterized as cards?  Would make it easier in some ways since enemy "turns" could be the same as players.