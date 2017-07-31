/* eslint-env browser */

require('eventsource-polyfill');
const hotClient = require('webpack-hot-middleware/client?noInfo=true&reload=true'); // eslint-disable-line import/no-unresolved

hotClient.subscribe((event) => {
  if (event.action === 'reload') {
    window.location.reload();
  }
});
