# TODO: 

### Create capsule with physics
- ✔️Drag mouse from capsule to direction to calculate impulse force vector, applied on release
- ✔️Capsule needs to bounce from walls 
- ✔️Allow multiple characters per player (3 to start)

### Networking
- ️️✔️Host can create session
- ✔️Client can connect to session with IP
- ✔️Both players can apply force ONLY to their own "players"
- Multiple networked players per client

### Game
- Starts when client is connected  
- Command time (players can input command)
- Act time (commands input are applied)
- Goals:
    - Score is counted
    - Players reset position
    - Ball reset
- Game loop:
    - 3 goals reset game
    - Turn limit

### Lobby
- Options:
    - Target score count 
    - Turn limit

### Presentation:
- Graphics for capsule
- Graphics for arena
- UI elements
- Menus
