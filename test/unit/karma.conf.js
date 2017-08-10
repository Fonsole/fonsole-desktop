process.env.NODE_ENV = 'testing';
const webpackConfig = require('../../compile/webpack.renderer.electron.config');

module.exports = (config) => {
  config.set({
    frameworks: ['mocha', 'sinon-chai'],
    files: ['./index.js'],
    preprocessors: {
      './index.js': ['webpack', 'sourcemap'],
    },

    // Coverage
    reporters: ['spec', 'coverage'],
    coverageReporter: {
      dir: './coverage',
      reporters: [
        { type: 'lcov', subdir: '.' },
        { type: 'text-summary' },
      ],
    },

    // Webpack
    webpack: webpackConfig,
    webpackMiddleware: {
      noInfo: true,
    },

    // Electron
    browsers: ['visibleElectron'],
    client: {
      useIframe: false,
    },
    customLaunchers: {
      visibleElectron: {
        base: 'Electron',
        flags: ['--show'],
      },
    },
  });
};
