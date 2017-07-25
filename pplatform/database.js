const MongoClient    = require('mongodb').MongoClient;
const bodyParser     = require('body-parser');

const url = "mongodb://tides-dev:sos@ds135049.mlab.com:35049/fonsole-dev";

exports.init = function (app) {
    app.use(bodyParser.urlencoded({ extended: true }));

    MongoClient.connect(url, (err, database) => {
        if (err) return console.log(err)
        require('./app/routes')(app, database);             
    })
}