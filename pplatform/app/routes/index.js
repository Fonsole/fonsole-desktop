module.exports = function(app, db) {
  require('./news_routes')(app, db)
};