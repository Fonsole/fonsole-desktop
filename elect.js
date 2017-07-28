const { app, BrowserWindow } = require('electron');

let win = null;

app.on('ready', () => {
  // Initialize the window to our specified dimensions
  win = new BrowserWindow({ width: 1280, height: 720, resizable: false });

  win.setMenu(null);

  // Specify entry point to default entry point of vue.js
  win.loadURL('http://localhost:8080');

  // Remove window once app is closed
  win.on('closed', () => {
    win = null;
  });
});
// create the application window if the window variable is null
app.on('activate', () => {
  if (win === null) {
    createWindow();
  }
});
// quit the app once closed
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
