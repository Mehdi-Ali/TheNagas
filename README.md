# The Nagas
A Vertical slice of a Mobile RPG (Personal Project) in development with Unity engine.

---------Playable Characters:---------

Characters has 4 abilities and a basic attack:  

* State Machine:
  - Idle
  - Running
  - Basic Attack
  - First Ability / Base Attack
  - Second Ability / CC
  - Third Ability / Dash / Defense / Escape
  - Ultimate
  - Death
   
* The Knight - Semi Done: (messing: effects (like stun and slow), sounds and VFX)
  - Basic Attack / Auto Attack: a 3 melee attack combo with the sword; each attack had its own animation and damage
  - First Ability: Base Attack: He spins his sword while still being able to move around and do AOE (attack over area) damage to nearby enemies.
  - Second Ability: CC (crowd control) Attack: He slashes a slightly charged attack and slows all enemies hit. 
  - Third Ability: Dash/Defense/Escape: He dashes a short distance forward and his next basic attacks does more damage for a few seconds. (While dashing he is immune to all attacks and effects)
  - Ultimate / Ult: Do a jump flip and once hit the ground he deals a big AOE damage around him and stun the enemies.

* The Necromancer - Not Yet Started:
  - Basic Attack / Auto Attack:  A skillshot that deals damage to the first enemy hit (single hit attack means it can only hit one target) // Une boule de flamme mes verte style necromancer.
  - Base Attack: An AOE Attack in a circle zone. // Une zone de degat avec des effet vert similaire au boule
  - CC (crowd control) Attack: A circular skillshot that stuns enemies in it. // des main skelettique qui sort du sol et les attrapent.
  - Dash/Defense/Escape: Enters an Immune mode where he receives no damage or effects and he cannot attack for 1 or 2 seconds, // IL commance a volé (floté un peu sur le sols) et ces yeux s'allume.)
  - Ultimate / Ult: Absorbs the lives of enemies and slows them down for a few seconds // Effect for absorbing the lives of enemies.

---------Enemies (mobs)---------

Enemies roams around randomly and pauses in place from time to time, the vision in front is bigger compared from behind and sides, and if the player runs away, they stop chasing, and always attacking the nearest player. Two basic attacks then a super one that deals more damage and effects.

* State Machine:
  - Idle
  - Roaming
  - Chasing
  - Basic Attack
  - Super Attack
  - Death

* The Ghost - Semi done: (Messing effects, sounds and VFX)
  - Basic Attack: Charges in small duration then project an attack a straight line // ghostly like effects for charging and firing ghostly ball.
  - Supper Attack: Every 2 basic attack he charges an attack then hits in a cone, heroes caught in the cone takes heavy damage. // ghostly like effects for charging and hitting in a cone.
    
* The Small Knight - Not Yet Started:
  - Basic Attack: Swings his sword left and right dealing damage with each swing.
  - Supper Attack: Dashes into the nearest player slowing and damaging players hit.
  
---------Game Design---------

Every aspect of game design is made as scriptable objects for a better and easier experience when setting the game design parameters (like camera, playable characters and enemies' statics (such as ranges, speed, damage and cooldown...)...)

---------Networking---------

The Multiplayer networking is handled with Fish-Networking solution, it is a Server authoritative logic with a client-side prediction, and more secure and better syncing in case of lag.

---------Gameplay---------

  * Movement: Standard left side screen movement: Invisible Gamepad that turns visible and snaps to the player touché position.
  * Aiming:
    - Click and drag to aim the ability Skillshot.
    - If the ability is taped, the aiming is automatic to the nearest enemy.
    - If dragged to the cancel button the ability is candled.
  * Gameplay Footage: https://www.artstation.com/artwork/B3b8rr
  
---------Visuals---------

Work in progress using Unity Shader Graph, Visual Graph, and particle system. (Different branch then main)

---------Builds---------

* Android - apk : https://drive.google.com/file/d/1_nMmMjYdm3oWeAJPzziNGoZzRoKQgfqo/view?usp=share_link
* IOS - ipa : https://drive.google.com/file/d/1QAQNOzF8J3o9m6suOUddLICCmysasYEQ/view?usp=share_link
