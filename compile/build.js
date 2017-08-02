

process.env.NODE_ENV = 'production';

const chalk = require('chalk');
const del = require('del');
const packager = require('electron-packager');
const webpack = require('webpack');
const Multispinner = require('multispinner');

const buildConfig = require('./build.config');
const mainConfig = require('./webpack.main.config');
const electronConfig = require('./webpack.renderer.electron.config');
const webConfig = require('./webpack.renderer.web.config');

const doneLog = `${chalk.bgGreen.white(' DONE ')} `;
const errorLog = `${chalk.bgRed.white(' ERROR ')} `;
const okayLog = `${chalk.bgBlue.white(' OKAY ')} `;
// const isCI = process.env.CI || false;

function clean() {
  del.sync(['build/*', '!build/icons', '!build/icons/icon.*']);
  console.log(`\n${doneLog}\n`);
  process.exit();
}

function pack(config) {
  return new Promise((resolve, reject) => {
    webpack(config, (compilerErr, stats) => {
      if (compilerErr) {
        reject(compilerErr.stack || compilerErr);
      } else if (stats.hasErrors()) {
        const webpackErr = stats.toString({
          chunks: false,
          colors: true,
        }).split(/\r?\n/).reduce((line, error) => {
          error += `    ${line}\n`;
          return error;
        }, '');

        reject(webpackErr);
      } else {
        resolve(stats.toString({
          chunks: false,
          colors: true,
        }));
      }
    });
  });
}

function bundleApp() {
  packager(buildConfig, (err) => {
    if (err) {
      console.log(`\n${errorLog}${chalk.yellow('`electron-packager`')} says...\n`);
      console.log(`${err}\n`);
    } else {
      console.log(`\n${doneLog}\n`);
    }
  });
}

function web() {
  del.sync(['dist/web/*', '!.gitkeep']);
  webpack(webConfig, (err, stats) => {
    if (err || stats.hasErrors()) console.log(err);

    console.log(stats.toString({
      chunks: false,
      colors: true,
    }));

    process.exit();
  });
}

function build() {
  del.sync(['dist/electron/*', '!.gitkeep']);

  const tasks = ['main', 'renderer'];
  const m = new Multispinner(tasks, {
    preText: 'building',
    postText: 'process',
  });

  let results = '';

  m.on('success', () => {
    process.stdout.write('\x1B[2J\x1B[0f');
    console.log(`\n\n${results}`);
    console.log(`${okayLog}take it away ${chalk.yellow('`electron-packager`')}\n`);
    bundleApp();
  });

  pack(mainConfig).then((result) => {
    results += `${result}\n\n`;
    m.success('main');
  }).catch((err) => {
    m.error('main');
    console.log(`\n  ${errorLog}failed to build main process`);
    console.error(`\n${err}\n`);
    process.exit(1);
  });

  pack(electronConfig).then((result) => {
    results += `${result}\n\n`;
    m.success('renderer');
  }).catch((err) => {
    m.error('renderer');
    console.log(`\n  ${errorLog}failed to build renderer process`);
    console.error(`\n${err}\n`);
    process.exit(1);
  });
}

switch (process.env.BUILD_TARGET) {
  case 'clean':
    clean();
    break;
  case 'web':
    web();
    break;
  default:
    build();
}
