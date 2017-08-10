/**
 * @file  Base configuration for renderer.
 *        Used for both browser and electron.
 */
process.env.BABEL_ENV = 'renderer';

const path = require('path');
const webpack = require('webpack');

const BabiliWebpackPlugin = require('babili-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

const baseConfig = {
  devtool: '#cheap-module-eval-source-map',
  module: {
    rules: [
      {
        test: /\.(js|vue)$/,
        enforce: 'pre',
        exclude: /node_modules/,
        use: {
          loader: 'eslint-loader',
        },
      },
      {
        test: /\.css$/,
        use: ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use: [
            { loader: 'css-loader', options: { importLoaders: 1 } },
            'postcss-loader',
          ],
        }),
      },
      {
        test: /\.js$/,
        use: 'babel-loader',
        exclude: /node_modules/,
      },
      {
        test: /\.vue$/,
        use: {
          loader: 'vue-loader',
          options: {
            extractCSS: process.env.NODE_ENV === 'production',
            loaders: {
              sass: 'vue-style-loader!css-loader!sass-loader?indentedSyntax=1',
              scss: 'vue-style-loader!css-loader!sass-loader',
            },
          },
        },
      },
      {
        test: /\.(png|jpe?g|gif|svg)(\?.*)?$/,
        use: {
          loader: 'url-loader',
          query: {
            limit: 10000,
            name: 'imgs/[name].[ext]',
          },
        },
      },
      {
        test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/,
        use: {
          loader: 'url-loader',
          query: {
            limit: 10000,
            name: 'fonts/[name].[ext]',
          },
        },
      },
    ],
  },
  node: {
    __dirname: process.env.NODE_ENV !== 'production',
    __filename: process.env.NODE_ENV !== 'production',
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env.IS_WEB': !!process.env.IS_WEB,
    }),
    new webpack.HotModuleReplacementPlugin(),
    new webpack.NoEmitOnErrorsPlugin(),
  ],
  output: {
    filename: '[name].js',
  },
  resolve: {
    alias: {
      '=': path.join(__dirname, '../src/shared'),
      '@': path.join(__dirname, '../src/renderer'),
      vue$: 'vue/dist/vue.esm.js',
    },
    extensions: ['.js', '.vue', '.json', '.css'],
  },
};

switch (process.env.NODE_ENV) {
  case 'production': {
    baseConfig.devtool = '#source-map';

    baseConfig.plugins.push(
      new webpack.DefinePlugin({
        'process.env.NODE_ENV': 'production',
      }),
      new ExtractTextPlugin('style.css'),
      new BabiliWebpackPlugin({
        removeConsole: true,
        removeDebugger: true,
      }),
      new CopyWebpackPlugin([
        {
          from: path.join(__dirname, '../static'),
          to: path.join(__dirname, '../dist/electron/static'),
          ignore: ['.*'],
        },
      ]),
      new webpack.LoaderOptionsPlugin({
        minimize: true,
      }),
      new webpack.BannerPlugin({
        test: /\.js$/,
        exclude: /node_modules/,
        banner: `
███████╗ ██████╗ ███╗   ██╗███████╗ ██████╗ ██╗     ███████╗
██╔════╝██╔═══██╗████╗  ██║██╔════╝██╔═══██╗██║     ██╔════╝
█████╗  ██║   ██║██╔██╗ ██║███████╗██║   ██║██║     █████╗
██╔══╝  ██║   ██║██║╚██╗██║╚════██║██║   ██║██║     ██╔══╝
██║     ╚██████╔╝██║ ╚████║███████║╚██████╔╝███████╗███████╗
╚═╝      ╚═════╝ ╚═╝  ╚═══╝╚══════╝ ╚═════╝ ╚══════╝╚══════╝
` }));
    break;
  }
  case 'testing': {
    // apply vue option to apply isparta-loader on js
    baseConfig.module.rules
      .find(rule => rule.use.loader === 'vue-loader').use.options.loaders.js = 'babel-loader';
    baseConfig.devtool = '#inline-source-map';
    delete baseConfig.entry;
    delete baseConfig.externals;
    delete baseConfig.output.libraryTarget;
    break;
  }
  default: { // development
    baseConfig.plugins.push(new webpack.DefinePlugin({
      __static: `"${path.join(__dirname, '../static').replace(/\\/g, '\\\\')}"`,
    }));
  }
}

module.exports = baseConfig;
