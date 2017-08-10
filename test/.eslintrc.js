module.exports = {
  settings: {
    'import/resolver': {
      webpack: {
        config: 'compile/webpack.renderer.electron.config.js',
      },
    },
  },
  env: {
    mocha: true,
  },
  globals: {
    assert: true,
    expect: true,
    should: true,
    sinon: true,
    __static: true,
  },
  rules: {
    'func-names': 0,
    'prefer-arrow-callback': 0,
  },
};
