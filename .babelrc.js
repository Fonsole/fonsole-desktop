module.exports = {
  presets: [
    ["env", {
      "modules": false,
    }],
    "stage-0",
  ],
  plugins: ["transform-runtime"],
  env: {
    test: {
      presets: ["env", "stage-0"],
      plugins: ["istanbul"],
    },
  },
};
