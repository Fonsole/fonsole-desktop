# fonsole

## Project Overview
Fonsole is a platform that offers a series of multiplayer party games where the smartphone is the controller. Games are similar to board games, party games, card games and other social games that people typically play at gatherings/parties. Games will be mostly turn-based, or asynchronous games. Real time games will not be offered (initially). THE unique feature of this platform, is that players use their phones to control the games that will be displayed on a big screen (laptop or TV screen), via a simple website that they can access through their smart phone's web browser. The platform will provide the tools the infrastructure for devs to easily create additional games.

[Developer Discord](https://discord.gg/SmjaHGt)

[Useful Links](https://discord.gg/TZpJ4S8)

## Frameworks Used
* Vue.js
* Electron
* socket.io

## Build Setup Instructions
* Download Repo Manually or Clone it (Recommended)

* Make sure to have [NPM (node package manager)](https://www.npmjs.com/get-npm?utm_source=house&utm_medium=homepage&utm_campaign=free%20orgs&utm_term=Install%20npm) installed on your system and its version greater than or equal to 3.0.0.

* Open cmd.exe with admin privileges, browse to repo folder, and run the following command

	```npm install npm@5.2 -g```

* Install dependencies:

  ```npm install```

* Launch electron with "hot reload" at localhost:8080

	```npm run dev:web```

* Launch webpack-dev-server with "hot reload" at localhost:8080

  ```npm run dev:web```

* Build binaries, checkout ```package.json``` for all possible targets

  ```npm run build:win32```

* Run ESlint to fix possible style errors

  ```npm run lint-fix```

For detailed explanation on how things work, checkout the [guide](http://vuejs-templates.github.io/webpack/) and [docs for vue-loader](http://vuejs.github.io/vue-loader).
# fonsole-playground

### All Repositories:
[Desktop Repo](https://github.com/darklordabc/fonsole-desktop): Main repo, this is the desktop version of the game (bigscreen), it is the main view that all the players will see for the games, typically on a large monitor/tv, uses electron.

[API Repo](https://github.com/darklordabc/fonsole-api): This is the public API that developers can explore to see how to develop games for fonsole.

[Server Repo](https://github.com/darklordabc/fonsole-server): This is the server component of fonsole. It will eventually be made private.
