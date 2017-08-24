/**
 * This file is used specifically and only for development. It installs
 * `electron-debug` & `vue-devtools`. There shouldn't be any need to
 *  modify this file, but it can be used to extend your development
 *  environment.
 */
import { app } from 'electron';
import installExtension, { VUEJS_DEVTOOLS } from 'electron-devtools-installer';

require('source-map-support').install();

// Set environment for development
process.env.NODE_ENV = 'development';

// Install `electron-debug` with `devtron`
require('electron-debug')({ showDevTools: true });

// Treat promise rejections as errors
process.on('unhandledRejection', (err) => {
  throw err;
});

// Install `vue-devtools`
app.on('ready', async () => {
  try {
    await installExtension(VUEJS_DEVTOOLS);
  } catch (err) {
    console.log('Unable to install `vue-devtools`: \n', err);
  }
});

// Require `main` process to boot app
require('./index');
