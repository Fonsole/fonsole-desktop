const checkRun = require('./check_run');

// Run npm install on any change in package.json
checkRun(/package\.json$/, 'npm install');
