var path = require('path')
module.exports = {
  configureWebpack: {
    mode: 'production',
    resolve: {
      alias: {
        src: path.resolve(__dirname, 'src'),
        components: path.resolve(__dirname, 'src/components')
      }
    },
  }
}