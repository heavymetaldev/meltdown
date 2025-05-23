const path = require("path");
const { experiments } = require("webpack");

const config = {
  mode: "development",
  context: path.resolve(__dirname),
  entry: "./build/cli.js",
  target: "node",
  experiments: {
    outputModule: true
  },
  externalsType: 'module',
  externalsPresets: { node: true },
  module: {
    // rules: [
    //   {
    //     test: /\.ts$/,
    //     use: "ts-loader",
    //     exclude: /node_modules/,
    //   },
    // ],
  },
  // resolve: {
  //   //enforceExtension: true,
  //   extensions: [".ts", ".js", ".tsx", ".jsx"],
  // },
  output: {
    path: path.resolve(__dirname, "./dist"),
    filename: "bundle.mjs",
    module: true,
    libraryTarget: "module"
  },
};

module.exports = config;
