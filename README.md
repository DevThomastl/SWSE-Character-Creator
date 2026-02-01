# SWSE-Character-Creator

## Description of Application
An application for creating characters in Star Wars Saga Edition.
HTML, CSS, SQL-SERVER, ASP.NET, C#, MVC,
TTRPG

## Wireframe Sketches
![Wireframe Sketch](Wireframe%20Design.png)


## Solution Architecture Diagram
![Solution Architecture Diagram](SolutionArchitectureDiagram.png)


## User Stories
### User Story #1: Create a New Character
As a player, 
I want to create a new character for Star Wars Saga Edition,
So that I can begin playing Star Wars Saga Edition.

### User Story #2: Save a Character
As a player,
I want to save a character,
So that I have access to it in the future.

### User Story #3: Load an Existing Character
As a player,
I want to load an existing character,
So that I can view or edit it.

### User Story #4: Delete a Character
As a player,
I want to delete a character,
So that I can remove any characters that have died.

### User Story #5: Edit a Character
As a player,
I want to edit a character,
So that I can update the character whenever it needs updating.

### User Story #6: Calculate Character Statistics Automatically
As a player,
I want to calculate character statistics automatically,
So that I do not have to perform manual calculations.

### User Story #7: Prevent Illegal Characters
As a player,
I want to prevent illegal characters,
So that I do not mistakenly create something that is unplayable.

## Use-Cases
### Use-Case #1: Create a Character
**Actor**: User\
**Description:** The user creates a new character by selecting the character's species, ability scores, class, feats, skills, and talents. The application will validate the character by ensuring it adheres to the ruleset, and calculates various statistics.
---
### Use-Case #2: Edit a Character
**Actor**: User\
**Description:** The user edits an existing character by changing skills, feats, talents, or ability scores. The application ensures any changes adhere to the ruleset.
---
### Use-Case #3: Save a Character
**Actor**: User\
**Description:** The user saves a character locally. The system validates the character data and makes the data persistent in the event of future retrieval.
---
### Use-Case #4: Load a Character
**Actor**: User\
**Description:** The user loads an existing character from the local database.
---
### Use-Case #5: View a Character
**Actor**: User\
**Description:** The user views the digital character sheet of an existing character. All statistics and information are displayed.
---

## (UML) Use-Case Diagram
![Use Case Diagram](UseCaseDiagram.png)
