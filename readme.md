# Packages #

Title: Gravitas physics system  
Author : [awtdev](https://itch.io/profile/awtdev)  
URL : [https://awtdev.itch.io/gravitas-physics-system](https://awtdev.itch.io/gravitas-physics-system)  
Lincense: CC0

# Git Branch Naming Scheme #
Branches should be named following this format: 
"branch type"-"prefix"-"Discipline"-"branch_name"

## Branch Types ##
- Dev - Developer branch, for adding new features. The game state in these branches should be incomplete and unpolished
- Master - ONLY FOR POLISHED BUILDS FOR PEER ASSESSMENTS AND DEMO DAYS

## Prefixes ##
- Asset - Asset branch, for when you're just adding assets into the game.
- Bugfix - For branches that are fixing bugs.
- Docs - For documentation themed updates.
- Feature - For new features.
- Hotfix - For quick fixes that happen on the fly.
- Release - For releasing polished builds.
- Playtest - For releasing semi-stable playtesting builds

## Discipline (Fairly self explanatory.) ##
- For releases, there may not be a discipline, so you can just put "N/A"
- Animation - For Animation related work.
- Art - For art related work. (Can include the specific discipline if you'd like.)
- Audio - For audio related work.
- Design - For design, UX and QA related work.
- Production - For production related work.
- Programming - For programming related work.
- Writing - For writing related work.

## Branch name ##
Just for readability sake, make sure words are separated by underscores. Otherwise it can be whatever, just make sure it is clear and obvious as to what has been made. A good guide for this would be to include what is being edited, and what about it is being edited. For example:
AI_3D_Pathfinding.
And not:
Enemybranch. <- this is too vague... what is being done with the enemies in this branch exactly?

## Examples ##
- Dev-Asset-Art-Crane model added.
- Master-Playest-Design-Core Gameplay Loop Testing.

# File Naming Convention #
In engine files should also follow a naming convention. This is most applicable to programmers, however should also be followed by ANYONE putting ANYTHING in engine.
Files should be prefixed with a tag, followed by the name. Words in the name should be separated by underscores.
The general structure of files should be as follows:
"Tag" - "Name1_Name2"

## Animation Files (Probably will get changed as we get more into animation) ##
- ANI - Animations

## Art Files ##
- MAT - Materials
- MOD - Models
- TEX - Textures
- SPR - Sprites
- BAC - Background Art
- UIN - User Interface Art (Things like buttons)

## Audio Files ##
- MUS - Music
- SFX - Sound Effects

## Code Files ##
- SCR - Script

## Prefabs ##
- PRE_AIR - Airship Prefabs
- PRE_CIT - City Prefabs
- PRE_ENE - Enemy Prefabs
- PRE_NET - Network Object Prefabs
- PRE_PLA - Player Prefabs
- PRE_PPR - Post Processing Prefabs
- PRE_SHA - Ship Asset Prefabs (Things like the engine, wheel, pressure gauge etc.)
- PRE_TER - Terrain Prefabs
- PRE_UIN - User Interface Prefabs

## Scenes ##
- SCN - 2D Scenes
- SCN - 3D Scenes

## Examples ##

- ART_MAT-Airship_Material
- PRE_CIT-City_Canvas

a