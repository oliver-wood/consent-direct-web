/*
    ./webpack.config.js
*/
const path = require('path');

// Generate HTML automatically
const CleanWebpackPlugin = require('clean-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = {
  entry: './src/index.js',
  output: {
    path: path.resolve('dist'),
    filename: 'app.js',
    publicPath: '/'
  },
  module: {
    rules: [
      {
        test: /\.(css|scss|sass)$/i,
        use: [
          { loader: "style-loader" },
          { loader: "css-loader" },
          { loader: "sass-loader" }
        ]
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: "babel-loader"
      }, {
        test: /\.jsx?$/,
        exclude: /node_modules/,
        use: "babel-loader"
      }
    ]
  },
  plugins: [
    new CleanWebpackPlugin(['dist']),
    new HtmlWebpackPlugin({
      template: 'src/index.html',
      inject: 'body'
    })
  ]
}
