const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');

const conf = {
    entry: {
        main: './src/index.js',
    },
    output: {
        path: path.resolve(__dirname, './dist'),
        filename: 'assets/js/[name].js',
        publicPath: '/',
    },
    devServer: {
        contentBase: path.join(__dirname, './dist'),
        overlay: true,
    },
    devtool: 'eval-sourcemap',
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: 'babel-loader'
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader'
                ],
            },
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: 'assets/css/[name].css'
        }),
        new HtmlWebpackPlugin({
            hash: false,
            template: './src/index.html',
            filename: 'index.html',
        }),
    ],
};

module.exports = () => {
    return conf;
};