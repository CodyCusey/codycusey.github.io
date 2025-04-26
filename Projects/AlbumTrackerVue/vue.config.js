
//This was here
// const { defineConfig } = require('@vue/cli-service')
// module.exports = defineConfig({
//   transpileDependencies: true
// })

//This was copied from the vuedemo file
module.exports = {
  configureWebpack: {
    devtool: 'source-map',
  },
  devServer: {
    proxy: {
      '/api': {
        target: 'http://localhost:8888', // target host
        ws: true, // proxy websockets
        changeOrigin: true, // needed for virtual hosted sites
      },
    },
  },
};