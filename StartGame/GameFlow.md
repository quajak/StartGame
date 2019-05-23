# Game Flow
In this file the gerneral flow of the game is discussed.

Start:

Character Generation -> Mission -> Mission Result -> World map -> Movement -> (City -> Interact with shops and people) -> Mission -> Repeat

this is in the game represented as

MainGameMenu -> (Player Creator) -> Campaign Controller -> Main Game -> Mission Result -> (Level Up) -> World View -> ([City Window]) -> Main Game

Change:

Maybe one removes campaign controller and incorperates it into the map

Player starts on world view and moves before the first mission

New:

MainGameMenu

(PlayerCreator)

World View (Generate map and missions)\
Travel

Mission -> Fight -> Reward\
or\
City -> Trade



TODO:

Build Worldview with a terrain, player and a mission - Done

Allow movement of player - Done

Generate random missions on the map and allow them to be played - Done

Make some missions forced - Done

Add premaded dungeons to map generation

Randomly generate dungeons

Add cities as map features

Add shops to cities, where items can be built

Give cities economic values which affect shops - Done

Add streets between cities - Done

Add forests - As biome

Add rivers?

Add time

Add food and water consumption

Add traders who move between cities

Make cities produce goods

Add goblin tribes to deserts
* Single gobling
* Goblin party
* Goblin tent attack

Add hunts

Add castles - not only cities