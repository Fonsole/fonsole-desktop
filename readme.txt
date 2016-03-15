1
Tree main parts:
	- BigView:
		* desktop application using (Unity / C#)
		* shows the game graphics + contains the game logic
		* folder PartyGamesBigView
		
	- Platform - Server side:
		* Node.js
		* relays messages between controller and bigview using Netgroup (socket.io) + delivers the webpage/controllers (see below)
		* folder pplatform 
		
	- Platform - Client side:
		* HTML5 / Java script
		* Contains the Controller for the games
		* folder pplatform/public (the folder is hosted via node.js app (pplatform.js) which also does the message relay)
		
		
Modules:
	- Netgroup
		* uses socket.io to send messages accross the internet
		* runs on the server, controller and bigview using different libraries to communicate via socket.io
	
	- PPlatform
		* builds a simplified interface to do tasks needed for all games
			e.g. starting a game via controller, opening a new room/getting the game code, sending string messages accross (use json)
		* handles global messages like a new controller registeres, reconnects, disconnects, ...
		* it wraps Netgroup entirely so noone has to deal with the unterlaying message system itself and can just use it via
			a sendMessage method + addMessageListener to receive messages
	
	
	- SayAnything
		* first implemented game using PPlatform
	
		
	- Gamelist: 
		* implemented as the default application that will run after the system started
		* starting point to run a run a game / implement new games
	
	
Structure (important files and folders):
\PartyGamesBigView 									
												the bigview unity project
														
\PartyGamesBigView\Assets\games\gamelist
												the bigview game list + gamelist.unity scene file
												It shows the game code / could show a list of available games
												(not interactive though. games will be started via the controller)

\PartyGamesBigView\Assets\games\sayanything
												SayAnything game folder + start scene sayanything.unity

\PartyGamesBigView\Assets\pplatform
												Contails the platform specific files / reusable code between games
												
\PartyGamesBigView\Assets\pplatform\Platform.cs
												Singleton giving access to the PPlatform module
												including methods to send messages to controllers / registering handler
												to receive messages
												
\PartyGamesBigView\Assets\pplatform\JsonWrapper.cs
												JSON parser to allow unity / js communication (uses Newtonsoft.Json currently)

\PartyGamesBigView\Assets\SocketIO				
												Folder of the used socket.io plugin

\pplatform
												Folder for web side of the applicatin
												
\pplatform\pplatform.js
												Node.js app that will setup the socket.io message system + start the web server to access
												the controller code / mobile page
												
\pplatform\public								
												Public folder.
												Everything in here can be accessed via http://fonsole.com 
												after pplatform.js is started on the server

\pplatform\public\index.html					PPlatform start page. It boots ub the client side PPlatform code and keeps it active
												while all games can run in iframe implemented in this page. This allows to
												have an entry point for shared functionality for all games.

\pplatform\public\notconnected.html
												Default iframe content of index.html. It will show the login screen / allows
												the user to enter the game code. It will be replaced with the gamelist
												or one of the games itself after login was successful.
												
\pplatform\public\game\gamelist\index.html
												Game list for the controller side. Add your new game here to start it.
												
\pplatform\public\game\partypaint				
												(old prototype. currently not running / not supported anymore)
												
\pplatform\public\game\sayanything
												Controller side game folder for the game SayAnything
												
									
												

How to test locally:
	* first you need unity for the bigview part and node.js for the controller and server/network part
	* Unity / bigview:
		* get the personal edition at http://unity3d.com/get-unity
		* open the project directory PartyGamesBigView
		* open the scene PartyGamesBigView\Assets\games\gamelist\gamelist.unity to test games using http://fonsole.com
		* open an indivudal game to test locally e.g. \PartyGamesBigView\Assets\games\sayanything\sayanything.unity
			The game will start trying to connect to a local server until it finally finds one. See the unity console output for details
	* Node.js:
		* download at https://nodejs.org/en/download/
		* after installation use the commant promt to run:
			* "npm install" in folder \pplatform This will download and install all libraries needed for the project
			* "node platform.js" will finally start the server itself
		* Node js will start 2 servers in one:
			* a webserver on port 80 accessable via http://localhost (don't use 127.0.0.1 the controller code checks for "localhost" in the domain name to start local testing mode)
				NOTE: SKYPE MIGHT USE PORT 80! Stop skype and then restart the server if you have problems
			* a socket.io server on port 3001
	* Testing:
		1. start node js server
		2. start unity and select "sayanything.unity"
		3. The command prompt of node.js should show you debug output in the moment unity connects. unity itself will show you a similar output in the console tab
		4. Open http://localhost in your browser -> enter the game code shown in unity or in the debug output
		5. Open multiple windows for multiple users and start testing!
		
	
												
How to add a new game:

	Currently, games can be started via the gamelist on controller side. (PATH: pplatform\public\game\gamelist\index.html)
	The "gamelist" is implemented like a game itself which starts by default. If you want to add a new game simply add a new button
	to the html file. A button click should call the platform method "enter game" giving the GAMENAME. GAMENAME can be any name you
	choose (not visible to the user). It will be simply used as a folder name to locate the game code itself!
	Following paths are assumed for the games:
		* Bigview side: PartyGamesBigView\Assets\games\GAMENAME\GAMENAME.unity (scene file)
		* Controller side: pplatform\public\game\GAMENAME\index.html
	
	After 'enterGame("GAMENAME")' is called all connected controller and the bigview will try to load the game scene / html file
	The controller side will load games in an iFrame and bigview will load the given game as a unity scene
	while keeping the PPlatform module as a singletone gameobject in the scene
	If a game is started directly in the unity editor and not via the game list the PPlatform singleton will initialize
	using "localhost" as domain name instead of fonsole.com allowing to test games locally! The controller side will
	also recognize if it runs on localhost and thus connect to localhost:3001 for socketio!
	
	As the gamelist itself works like a game you can simply leave a game by calling enterGame("gamelist") on the controller side.
	The interface should only show this on hostcontrollers though!
	
