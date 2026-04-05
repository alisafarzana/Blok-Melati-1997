# BLOK MELATI 1997

## Group Members
| Name | Student ID |
|:---|:---:|
| Alisa Farzana Binti Azmi | 24006434 |
| Batrisyia Binti Azhar | 24005624 |
| Lewin Khoo Zhen Xiong | 24004552 |
| Muhammad Zal Hasmi Bin Mat Zaidi | 24005453 |
| Nurliyana Farhanah binti Mohd Zaki | 24006257 |
| Putra Muhammad Adam Muqri bin Nasrun | 24006216 |
| Qaseeh Dania Aisyah Bt Ahmad Shahrel | 24006482 |

## Project Description
Blok Melati 1997 is a pixelated and narrative game where the main character is Yana, a lower-form female student studying in an all girls boarding school. During her studies, she always becomes a target of a senior student. One day, a senior student forced Yana to wash five pieces of her clothes as a punishment during midnight. However, she was disturbed by a vicious ghost called Serena while completing her tasks so we need to help her survive the night. This game is based on the actual Malaysia’s boarding school which set in a long dormitory corridor, toilet and the clothesline posts.

## System Features
| Features | Explanation |
|:---|:---|
| Player Movement | The game system allow user to control the movement of Player using the keyboard input. Each movement has a different animation of the Player |
| Ghost Interaction | The Ghost is the enemy of the game. Ghost will appear in certain time and follow the Player. When the Player hides, the Ghost will roam randomly and exit the area after 12 seconds. Ghost interacts with Player through collision detection, causing health of Player to decrease |
| Health Management | The system has a health mechanism where the Player has only 3 lives in every level. Health will decrease by one if Player collides with the Ghost |
| Collision detection | The system detects collision between Player and Ghost as well as Objects. These collisions have their own events such as health decrease, collecting item, hiding and task activation |
| Inventory Management | The system has inventory where collected items are stored in it | 

## OOP Concepts Used
* Encapsulation
* Inheritance
* Polymorphism
* Abstraction

## How to Run the Program
1. Open the solution in Visual Studio
2. Build the project
3. Run using Start Debugging (F5)
4. Turn on your volume for best experience

## Project Structure
| File | Description |
|:---|:---|
| Character.cs | Base class for Ghost and Player. Controls movement and animation |
| FillBucketTask.cs | Second task in Level 1. Fill up the empty bucket |
| Form1.cs | Level 1 |
| Form2.cs | Level 2. Sequel to Level 1 |
| GameManager.cs | Controls game logic |
| GameMenu.cs | Controls Main Menu logic |
| GameTaskBase.cs | Base class to all tasks class. Controls all tasks logic |
| Ghost.cs | Serena, the enemy in the game |
| HangTask | Level 2 task. Hangs all washed clothes from Level 1 |
| Inventory | Stores all player items |
| Item.cs | Items in the game |
| Level.cs | Manage the transitions of level and stores the obstacles in level |
| PickUpTask.cs | First task in Level 1. Pick up all items on the floor |
| Player.cs | Yana, the playable character |
| Program.cs | Main Program |
| Resizer.cs | Controls window sizing |
| Start.cs | Main Menu |
| StoryManager.cs | Controls the storyline of the game |
| TimedTask.cs | Controls the duration of task progress |
| WashTask.cs | Third task in Level 1. Wash the clothes picked up |
| health.cs | Controls the hearts of Player |

## How to Play
### Objective
#### Level 1
Player needs to collect 5 clothes and wash them at the washing area while avoiding the ghost. Player is given 3 lives. Each encounter with the ghost, Player will lose a life. if Player loses 3 lives, they will lose and have to restart the level.
#### Level 2
Player needs to hang 5 clothes at the clothesline while avoiding the ghost. Player is given 3 lives. Each encounter with the ghost, Player will lose a life. if Player loses 3 lives, they will lose and have to restart the level.

### Keybinds
| Key | Action |
|:---:|:---:|
| W | Move Up |
| A | Move Left |
| S | Move Down |
| D | Move Right |
| E | Hide |
| F | Wash/Hang Clothes |
| 1-6 | Select Item |



