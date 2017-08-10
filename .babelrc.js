const plugins = ['transform-runtime'];
const presets = [
  ['env', {
    modules: false,
  }],
  'stage-0',
];

if (process.env.NODE_ENV === 'testing') {
  plugins.push('istanbul');
}

module.exports = {
  presets,
  plugins,
};
