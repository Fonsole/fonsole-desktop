# fonsole

## Project Overview 
Fonsole is a platform that offers a series of multiplayer party games where the smartphone is the controller. Games are similar to board games, party games, card games and other social games that people typically play at gatherings/parties. Games will be mostly turn-based, or asynchronous games. Real time games will not be offered (initially). THE unique feature of this platform, is that players use their phones to control the games that will be displayed on a big screen (laptop or TV screen), via a simple website that they can access through their smart phone's web browser. The platform will provide the tools the infrastructure for devs to easily create additional games. 

[Developer Discord](https://discord.gg/SmjaHGt)

[Related Links](https://discord.gg/TZpJ4S8)

## Frameworks Used (To be filled)
* Vue.js
* Electron
* socket.io

## Build Setup Instructions
* Download Repo Manually or Clone it (Recommended)

* Make sure to have [NPM (node package manager)](https://www.npmjs.com/get-npm) installed on your system and its version greater than or equal to 3.0.0. 

* Open cmd.exe with admin privileges, browse to repo folder, and run the following command

	```npm install npm@latest -g```

* Install dependencies:

  ```npm install```

* Serve with "hot reload" at localhost:8080. Hot reload means that server will automatically update when changes are made to the code.

  ```npm run dev```

* Build for production with minification

  ```npm run build```

* Build for production and view the bundle analyzer report

  ```npm run build --report```
  
* Run ESlint to fix possible style errors

  ```npm run lint-fix```

For detailed explanation on how things work, checkout the [guide](http://vuejs-templates.github.io/webpack/) and [docs for vue-loader](http://vuejs.github.io/vue-loader).

### All Repositories:
#### [Desktop Repo](https://github.com/darklordabc/fonsole-desktop)
Main repo, this is the desktop version of the game (bigscreen), it is the main view that all the players will see for the games, typically on a large monitor/tv, uses electron.

**Dependencies:**
* [Networking Repo](#networking-repo)

#### [API Repo](https://github.com/darklordabc/fonsole-api)
This is the public API that developers can explore to see how to develop games for fonsole.

#### [Server Repo](https://github.com/darklordabc/fonsole-server)
This is the server component of fonsole. It will eventually be made private.

**Dependencies:**
* [Networking Repo](#networking-repo)
* [API Repo](#api-repo)

#### [Networking Repo](https://github.com/darklordabc/fonsole-networking)
This is a socket.io-based component, that inclueds both client and server files. This repository most likely also will be private.
* Server is located in `networking.js` file and is exported by default. Contains everything that is releated to rooms and working with client connections.
* Client part is located in `client` directory. Has everything that can be used for communication with server part. Also has a `export` that returns object with functions, that can be used by [Public API](#api-repo).
