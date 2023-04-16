# TODO: 

### Create capsule with physics
- ✔️Drag mouse from capsule to direction to calculate impulse force vector, applied on release
- ✔️Capsule needs to bounce from walls 
- ✔️Allow multiple characters per player (3 to start)

### Networking
- ✔️Host can create session
- ✔️Client can connect to session with IP
- ✔️Both players can apply force ONLY to their own "players"
- ✔️Multiple networked players per client

### Lobby
- ✔️Players appear
- ✔️Options:
	- ✔️Target score count 
	- ✔️Turn limit
- ✔️Load game from lobby
- ✔️Get back to lobby 
	
### GAME LOOP
- ✔️Command time (players can input command)
	- ✔️Ready checks
	- ✔️Timers
- ✔️Act time (commands input are applied, not able to add more commands)
- ✔️Goals:
	- ✔️Score is counted
	- ✔️Players reset position
	- ✔️Ball reset
- ✔️Game loop:
	- ✔️3 goals reset game
	- ✔️Turn limit

### Networking:
- ✔️Server not a player
- ✔️Connect with IP
- ✔️Headless server
- ✔️Connect to headless server on digitalocean

### Presentation:
- Menus
- UI elements
- Graphics for capsule (2 teams)
- Graphics for arena



## Notes:
- Run headless host with "Fussball.console.exe --headless host"

## Repos:
- Fussball: Godot game repo (this)
- FussballBuild: Only holds Linux build folder, used by VPS to get latest build
- FussballRunner: Go app to start servers on demand