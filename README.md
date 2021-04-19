﻿Goals
==

0. make console basically work correctly adding ChooseCardFromCIs(filter, presetChoiceForTests = null) so that console can really work right.
1. be able to simulate a battle after providing deck, [initial hand], enemy + behavior
2. generate optimal strategy w/monte carlo
3. can this be hooked up to alphazero
4. obviously the dream is a complete player of the entire game - simulating every choice fully
	annoying part of this is having to manually enter the behavior patterns of every enemy
5. make StS into a game like chess where you can share complete game states for analysis
6. enable "duplicate StS" in a fair way, to enable true comparison of skill

Questions
==
1. what is the best single card to add to the starting deck for a fight with nob, cubes, or sleeping guy
2. when cultist has 25hp and 1vuln and you have DS and 1 energy, should you strike or not?
3. try all possible 2,3,N-card decks vs specific enemies and see what's best

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
 - enable safe simulation of actions for pre-display of damage
 - implement an enemy in a nice way - this is complex to reverse engineer, especially at ascension levels
 - display intentions in console mode
 - display simulated damage in console mode
 - fix the whole "copy" requirement and node navigation - feels inefficient
 - reversible play and safe undos rather than copying entire data structures
 - representation of exact game & fight state in a single hash for sharing / replaying
 - in general how to handle random oneoff relics/statuses like frail/BTNodraw?  Really does seem easier to just patch the draw method.
 - handle reshuffle variation randomness => when you reshuffle, create a normal choice node and add a random child with key corresponding to it
 - random testing => means I need to fix the InitialBlock / blockAdjustment concept
 - actually, reshuffle is truly random, not draw. draw is deterministic.
 - need to move the logic for creating randomnodes elsewhere.
 - stop repeatedly visiting the same node?  First generate all actions and just pick one.
 - profiling - guess I spend 99% of the time in .Copy methods.  How to do this better?
 - mc mode with focusing
 - don't repeatedly choose the same exact action.
 - later - identical full fight state evaluation
 - compare MC results to repeatedly playing the real fight to get the same result distributions - the only way to really validate.
  - if you have A with children B and C and are calculating values, if B_v < A_v (by direct HP analysis) there is no reason to completely calculate its value; just return A as bestchild.
 - evolve needs structure adjustments.  ef can modify itself with more work.
 - minimal example of C/RC situation and verify that all paths are considered
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
 - console mode so you can "play" from cmdline. This is necessary to find bugs and generate more tests.
 - split out randomness a bit better
 - basic interactive mode with cmdline options.  normal cards - play card, potion, etc. and special mode addcard etc.
 - visualization improvements
 - fixup sense for statuses
 - unify status parameter - it means either "intensity" for things with fixed lengths like strength (permanent) or flame barrier status(1 round), or length, for things with fixed effects (vulnerable/weaken).
 - There is no reason for this to have two parameters; one which gets mapped is enough.
 - base tests for finding best lines in simple fights
 - how to generally get a narrative account of a fight?  textmode StS
 - I should try out events to handle some of these really rare connections. i.e. no other status except PlatedArmor cares when a player takes attack damage; why should I call ApplyStatus on every status whenever that happens?  And i don't just want to have cutouts directly in the damage code.