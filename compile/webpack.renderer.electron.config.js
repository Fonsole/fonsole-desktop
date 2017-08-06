process.env.BABEL_ENV = 'renderer';

const path = require('path');
const merge = require('webpack-merge');
const webpack = require('webpack');


const HtmlWebpackPlugin = require('html-webpack-plugin');

/**
 * List of node_modules to include in webpack bundle
 *
 * Required for specific packages like Vue UI libraries
 * that provide pure *.vue files that need compiling
 * https://simulatedgreg.gitbooks.io/electron-vue/content/en/webpack-configurations.html#white-listing-externals
 */
const whiteListedModules = ['vue'];
const { dependencies } = require('../package.json');

const rendererConfig = {
  entry: {
    electron: path.join(__dirname, '../src/renderer/main.js'),
  },
  externals: [
    ...Object.keys(dependencies || {}).filter(d => !whiteListedModules.includes(d)),
    'ws',
  ],
  node: {
    __dirname: process.env.NODE_ENV !== 'production',
    __filename: process.env.NODE_ENV !== 'production',
  },
  plugins: [
    new HtmlWebpackPlugin({
      filename: 'index.html',
      template: path.resolve(__dirname, '../src/index.ejs'),
      minify: {
        collapseWhitespace: true,
        removeAttributeQuotes: true,
        removeComments: true,
      },
      nodeModules: process.env.NODE_ENV !== 'production'
        ? path.resolve(__dirname, '../node_modules')
        : false,
    }),
  ],
  output: {
    libraryTarget: 'commonjs2',
    path: path.join(__dirname, '../dist/electron'),
  },
  target: 'electron-renderer',
};

/**
 * Adjust rendererConfig for development settings
 */
if (process.env.NODE_ENV !== 'production') {
  rendererConfig.plugins.push(new webpack.DefinePlugin({
    __static: `"${path.join(__dirname, '../static').replace(/\\/g, '\\\\')}"`,
  }));
}

module.exports = merge(require('./webpack.renderer.base'), rendererConfig);
