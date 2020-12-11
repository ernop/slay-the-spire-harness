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
 - minimal example of C/RC situation and verify that all paths are considered
 - absolute/montecarlo - which way to go? for very small fights absolute is okay.
 - I need helper methods to visualize the Choice/RandomChoice method
 - better "show full turn" methods
 - why isn't it finding all good lines?
 - tests for exhaustion of all good lines. make sure it finds all.
 - picking the shorter line to a win 
 - better to emit events when something happens and platedarmor just listens.
 - fix output so I can see what's going on - bad experience when running through nunit.
 - Are enemy attacks/actions best characterized as cards?  Would make it easier in some ways since enemy "turns" could be the same as players.
 - deck concepts - exhaust, draw, etc.  New test classes just for this
 - ability to characterize certain cards like the ones that add wounds, the one that draws & puts a card on top.
 - shouldn't card be a subclass of cardinstance?
 - fix up text notation so it's playable by hand.
 - AI to handle simple fights
 - monsters are going to be a lot more painful than I thought.
 - cards that have multiple "versions" of sims, i.e. headbutt or any card that has targets, needs to specify targets somehow? so the AI can monte carlo over them.
 - fix tests to have standard format and test autodetection.
 - console application for testing
 

Todos Done
==

 - unify status parameter - it means either "intensity" for things with fixed lengths like strength (permanent) or flame barrier status(1 round), or length, for things with fixed effects (vulnerable/weaken).
 - There is no reason for this to have two parameters; one which gets mapped is enough.
 - base tests for finding best lines in simple fights
 - how to generally get a narrative account of a fight?  textmode StS
 - I should try out events to handle some of these really rare connections. i.e. no other status except PlatedArmor cares when a player takes attack damage; why should I call ApplyStatus on every status whenever that happens?  And i don't just want to have cutouts directly in the damage code.