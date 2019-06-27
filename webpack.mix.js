const CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin');
const TSConfigPathsPlugin = require('tsconfig-paths-webpack-plugin');

require('laravel-mix')
  .webpackConfig({
    module: {
      rules: [
        {
          test: /\.(ts|tsx)$/,
          loader: require.resolve('tslint-loader'),
          enforce: 'pre',
          include: require('path').resolve('src'),
        },
      ],
    },
    plugins: [
      new CaseSensitivePathsPlugin(),
    ],
    resolve: {
      plugins: [
        new TSConfigPathsPlugin(),
      ],
    },
  })
  .setPublicPath('dist')
  .setResourceRoot('dist')
  .ts('src/app.tsx', 'dist');
