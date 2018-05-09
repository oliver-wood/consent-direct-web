const UglifyWebpackPlugin = require("uglifyjs-webpack-plugin");

exports.minifyJavaScript = () => ({
  optimization: {
    minimizer: [new UglifyWebpackPlugin({ sourceMap: true })],
  },
});
