const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:24444';

const PROXY_CONFIG = [
  {
    context: ["/graphql"],
    target: "http://connexion.dataio.internal:5001/graphql",
    secure: false,
    changeOrigin: true,
  },
  {
    context: ["/api/graphql"],
    target: "http://127.0.0.1:5005",
    pathRewrite: { "^/api/graphql": "/graphql" },
    secure: false,
    changeOrigin: true,
  }
]

module.exports = PROXY_CONFIG;
