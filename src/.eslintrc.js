// http://eslint.org/docs/user-guide/configuring

module.exports = {
  parser: 'babel-eslint',
  env: {
    browser: true,
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
