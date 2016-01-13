
/* 
 * Copyright (C) 2015 Christoph Kutza
 * 
 */
 
var express = require('express');
var app = express();
var http = require('http').Server(app);

//module netgroup. allows us to start the socket.io server
var NetgroupMod = require('./netgroup');

//to allow cross domains / e.g. connects from a unity game hosted on any domain
app.use(function(req, res, next) {
  res.header('Access-Control-Allow-Origin', '*');
  res.header('Access-Control-Allow-Methods', 'GET, OPTIONS');
  res.header('Access-Control-Allow-Headers', 'Content-Type');
  return next();
});

//the app/express module will host all files in the public folder like a good old normal webpage
app.use(express.static('public'));



//setup the netgroup server. we could use 
var netgroup = new NetgroupMod();
netgroup.listen(http);

console.log('netgroup test' + netgroup.test);


//process.env.PORT will be replaced with a pipe by azure if not hosted there the given port is used
var port = process.env.PORT || 3001;

//open the port for clients to connect
http.listen(port, function(){
    
    console.log('listening on *:' + port);
    
});

