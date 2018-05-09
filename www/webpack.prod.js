const merge = require('webpack-merge');
const common = require('./webpack.common.js');

const productionConfig = merge([
  parts.clean(PATHS.build),
  parts.minifyJavaScript(),
]);


module.exports = merge(common, {
  mode: 'production'
//  plugins: [
//    new UglifyJSPlugin()
//  ]
});
