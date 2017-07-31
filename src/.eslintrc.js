// http://eslint.org/docs/user-guide/configuring

module.exports = {
  parser: 'babel-eslint',
  env: {
    browser: true,
    node: false,
  },
  // required to lint *.vue files
  plugins: [
    'html',
  ],
  // check if imports actually resolve
  settings: {
    'import/resolver': {
      'webpack': {
        'config': 'build/webpack.base.conf.js',
      },
    },
  },
};
