process.env.BABEL_ENV = 'web';

const path = require('path');
const merge = require('webpack-merge');

const HtmlWebpackPlugin = require('html-webpack-plugin');

const webConfig = {
  entry: {
    web: path.join(__dirname, '../src/renderer/main.js'),
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
      nodeModules: false,
    }),
  ],
  output: {
    path: path.join(__dirname, '../dist/web'),
  },
  target: 'web',
  node: {
    fs: 'empty',
  },
};

module.exports = merge(require('./webpack.renderer.base'), webConfig);
